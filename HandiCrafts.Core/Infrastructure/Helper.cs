

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace HandiCrafts.Core
{
    public static class Helper
    {

        /// <summary>
        /// تعیین معتبر بودن کد ملی
        /// </summary>
        /// <param name="nationalCode">کد ملی وارد شده</param>
        /// <returns>
        /// در صورتی که کد ملی صحیح باشد خروجی <c>true</c> و در صورتی که کد ملی اشتباه باشد خروجی <c>false</c> خواهد بود
        /// </returns>
        /// <exception cref="System.Exception"></exception>
        public static Boolean IsValidNationalCode(this String nationalCode)
        {
            //در صورتی که کد ملی وارد شده تهی باشد

            if (String.IsNullOrEmpty(nationalCode))
                throw new Exception("لطفا کد ملی را صحیح وارد نمایید");


            //در صورتی که کد ملی وارد شده طولش کمتر از 10 رقم باشد
            if (nationalCode.Length != 10)
                throw new Exception("طول کد ملی باید ده کاراکتر باشد");

            //در صورتی که کد ملی ده رقم عددی نباشد
            var regex = new Regex(@"\d{10}");
            if (!regex.IsMatch(nationalCode))
                throw new Exception("کد ملی تشکیل شده از ده رقم عددی می‌باشد؛ لطفا کد ملی را صحیح وارد نمایید");

            //در صورتی که رقم‌های کد ملی وارد شده یکسان باشد
            var allDigitEqual = new[] { "0000000000", "1111111111", "2222222222", "3333333333", "4444444444", "5555555555", "6666666666", "7777777777", "8888888888", "9999999999" };
            if (allDigitEqual.Contains(nationalCode)) return false;


            //عملیات شرح داده شده در بالا
            var chArray = nationalCode.ToCharArray();
            var num0 = Convert.ToInt32(chArray[0].ToString()) * 10;
            var num2 = Convert.ToInt32(chArray[1].ToString()) * 9;
            var num3 = Convert.ToInt32(chArray[2].ToString()) * 8;
            var num4 = Convert.ToInt32(chArray[3].ToString()) * 7;
            var num5 = Convert.ToInt32(chArray[4].ToString()) * 6;
            var num6 = Convert.ToInt32(chArray[5].ToString()) * 5;
            var num7 = Convert.ToInt32(chArray[6].ToString()) * 4;
            var num8 = Convert.ToInt32(chArray[7].ToString()) * 3;
            var num9 = Convert.ToInt32(chArray[8].ToString()) * 2;
            var a = Convert.ToInt32(chArray[9].ToString());

            var b = (((((((num0 + num2) + num3) + num4) + num5) + num6) + num7) + num8) + num9;
            var c = b % 11;

            return (((c < 2) && (a == c)) || ((c >= 2) && ((11 - c) == a)));
        }

        public static string SerializeObject(object obj)
        {
            Dictionary<string, object> FinalProperties = new Dictionary<string, object>();

            try
            {
                JsonSerializerSettings objJSS = new JsonSerializerSettings();
                objJSS.Converters.Add(new StringEnumConverter { CamelCaseText = false });
                objJSS.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                objJSS.DateFormatString = "yyyy/MM/dd";

                string SerializeString = JsonConvert.SerializeObject(obj, Formatting.None, objJSS);

                return SerializeString;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }


        //public static void SetSession<T>(this ISession session, string key, T value)
        //{
        //    session.SetString(key, JsonConvert.SerializeObject(value));
        //}

        //public static T GetSession<T>(this ISession session, string key)
        //{
        //    var value = session.GetString(key);

        //    return value == null ? default(T) :
        //        JsonConvert.DeserializeObject<T>(value);
        //}

        public static DataTable ConvertListToDataTable<T>(IEnumerable<T> collection)
        {
            DataTable newDataTable = new DataTable();
            Type impliedType = typeof(T);
            PropertyInfo[] _propInfo = impliedType.GetProperties();
            foreach (PropertyInfo pi in _propInfo)
                newDataTable.Columns.Add(pi.Name, pi.PropertyType);

            foreach (T item in collection)
            {
                DataRow newDataRow = newDataTable.NewRow();
                newDataRow.BeginEdit();
                foreach (PropertyInfo pi in _propInfo)
                    newDataRow[pi.Name] = pi.GetValue(item, null);
                newDataRow.EndEdit();
                newDataTable.Rows.Add(newDataRow);
            }
            return newDataTable;
        }

        public static List<T> DeSerializeObjectArray<T>(string JsonArray)
        {
            List<T> ObjectArray = new List<T>();

            try
            {
                JArray JArr = JArray.Parse(JsonArray);

                foreach (JObject Js in JArr)
                {
                    T Obj = DeSerializeObject<T>(Js.ToString());

                    ObjectArray.Add(Obj);
                }

                return ObjectArray;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public static T DeserializeFromJson<T>(string json)
        {
            T deserializedProduct = JsonConvert.DeserializeObject<T>(json);
            return deserializedProduct;
        }

        public static T DeSerializeObject<T>(string JsonValue)
        {
            try
            {
                JsonSerializerSettings objJSS = new JsonSerializerSettings();
                objJSS.Converters.Add(new StringEnumConverter { CamelCaseText = false });
                objJSS.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                objJSS.DateFormatString = "yyyy/MM/dd";
                T DeSerializeObject = JsonConvert.DeserializeObject<T>(JsonValue, objJSS);

                return DeSerializeObject;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
    }

    public struct SelectBoxStruct
    {
        public string key;
        public string value;
    }

    [Serializable()]
    public struct DataTableStruct<T>
    {
        public int draw;
        public long recordsTotal;
        public long recordsFiltered;
        public List<T> data;
    }

    public struct UserInfo
    {
        public string name;
        public string family;
        public string nationalCode;
        public string mobile;
        public string memberValidationCode;
    }
}
