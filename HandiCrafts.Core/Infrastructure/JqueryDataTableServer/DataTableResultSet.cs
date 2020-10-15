using HandiCrafts.Core.Domain.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
/// <summary>
///     Resultset to be JSON stringified and set back to client.
/// </summary>
namespace DataTables
{
    public class DataTableResultSet
    {
        /// <summary>Array of records. Each element of the array is itself an array of columns</summary>
        public List<List<string>> data { get; set; } = new List<List<string>>();

        /// <summary>value of draw parameter sent by client</summary>
        public int draw { get; set; }

        /// <summary>filtered record count</summary>
        public int recordsFiltered { get; set; }

        /// <summary>total record count in resultset</summary>
        public int recordsTotal { get; set; }

        public void Load<TQuery>(DataTableParameters dataTableParameters, IQueryable<TQuery> queryable, out List<TQuery> list)
        {
            draw = dataTableParameters.Draw;
            recordsTotal = queryable.Count();
            queryable = dataTableParameters.ApplyFiltersAndOrders(queryable);
            recordsFiltered = queryable.Count();
            queryable = dataTableParameters.ApplyPaging(queryable);
            list = queryable.ToList();
        }
    }

    public class DataTableResultError : DataTableResultSet
    {
        public string error;
    }

}