// Turns the Ajax call parameters into a DataTableParameter object
// Permission to use this code for any purpose and without fee is hereby granted.
// No warrantles.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using HandiCrafts.Core.Infrastructure;
using Newtonsoft.Json.Linq;

namespace DataTables
{

    public class DataTableParameters
    {
        public Dictionary<int, DataTableColumn> Columns;
        public int Draw;
        public int Length;
        public Dictionary<int, DataTableOrder> Order;
        public bool SearchRegex;
        public string SearchValue;
        public int Start;

        private DataTableParameters()
        {
        }

        /// <summary>
        /// Retrieve DataTable parameters from WebMethod parameter, sanitized against parameter spoofing
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DataTableParameters Get(JsonElement input)
        {
            return Get(JObject.Parse(input.GetRawText()));
        }

        /// <summary>
        /// Retrieve DataTable parameters from JSON, sanitized against parameter spoofing
        /// </summary>
        /// <param name="input">JToken object</param>
        /// <returns>parameters</returns>
        public static DataTableParameters Get(JObject input)
        {
            return new DataTableParameters
            {
                Columns = DataTableColumn.Get(input),
                Order = DataTableOrder.Get(input),
                Draw = (int)input["draw"],
                Start = (int)input["start"],
                Length = (int)input["length"],
                SearchValue =
                    new string(
                        ((string)input["search"]["value"]).Where(
                            c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-').ToArray()),
                SearchRegex = (bool)input["search"]["regex"]
            };
        }

        public IQueryable<T> ApplyFiltersAndOrders<T>(IQueryable<T> query)
        {
            var filterColumns = Columns.Where(x => x.Value.Searchable && !string.IsNullOrEmpty(x.Value.SearchValue)).ToList();
            foreach (var item in filterColumns)
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var propertyExp = Expression.Property(parameter, typeof(T).GetProperty(item.Value.Name));
                var someValue = Expression.Constant(Convert.ChangeType(item.Value.SearchValue, typeof(T).GetProperty(item.Value.Name).PropertyType));
                Expression comparison = null;
                if (typeof(T).GetProperty(item.Value.Name).PropertyType == typeof(string))
                {
                    MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    comparison = Expression.Call(propertyExp, method, someValue);
                }                    
                else
                    comparison = Expression.Equal(propertyExp, someValue);

                Func<T, bool> filterExpression = Expression.Lambda<Func<T, bool>>(comparison, parameter).Compile();

                query = query.Where(filterExpression).AsQueryable<T>();
            }
            return query;
        }
        public IQueryable<T> ApplyPaging<T>(IQueryable<T> query)
        {
            return query.Skip(Start).Take(Length);
        }
    }

    public class DataTableColumn
    {
        public int Data;
        public string Name;
        public bool Orderable;
        public bool Searchable;
        public bool SearchRegex;
        public string SearchValue;

        private DataTableColumn()
        {
        }

        /// <summary>
        /// Retrieve the DataTables Columns dictionary from a JSON parameter list
        /// </summary>
        /// <param name="input">JToken object</param>
        /// <returns>Dictionary of Column elements</returns>
        public static Dictionary<int, DataTableColumn> Get(JToken input)
        {
            return ((JArray)input["columns"])
                .Select(col => new DataTableColumn
                {
                    Data = (int)col["data"],
                    Name =
                        new string(
                            ((string)col["name"]).Where(
                                c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-').ToArray()),
                    Searchable = (bool)col["searchable"],
                    Orderable = (bool)col["orderable"],
                    SearchValue =
                        new string(
                            ((string)col["search"]["value"]).Where(
                                c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-').ToArray()),
                    SearchRegex = (bool)col["search"]["regex"]
                })
                .ToDictionary(c => c.Data);
        }
    }

    public class DataTableOrder
    {
        public int Column;
        public string Direction;

        private DataTableOrder()
        {
        }

        /// <summary>
        /// Retrieve the DataTables order dictionary from a JSON parameter list
        /// </summary>
        /// <param name="input">JToken object</param>
        /// <returns>Dictionary of Order elements</returns>
        public static Dictionary<int, DataTableOrder> Get(JToken input)
        {
            return (
                (JArray)input["order"])
                .Select(col => new DataTableOrder
                {
                    Column = (int)col["column"],
                    Direction =
                        ((string)col["dir"]).StartsWith("desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC"
                })
                .ToDictionary(c => c.Column);
        }
    }
}