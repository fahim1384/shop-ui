
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace HandiCrafts.Core.Infrastructure
{
    public static class CommonHelper
    {
        public static string NormalizeString(params string[] strings)
        {
            strings = strings?.Select(x => x?.ToUpper().Trim())
                              .Where(x => !string.IsNullOrWhiteSpace(x))
                              .ToArray();

            if (strings?.Length > 0)
            {
                return string.Join(" ", strings);
            }

            return string.Empty;
        }

        public static string RemoveNewLine(this string value)
        {
          return  Regex.Replace(value, @"\t|\n|\r", "");
        }

        public static T ParseEnum<T>(string value, T defaultValue) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            foreach (T item in Enum.GetValues(typeof(T)))
            {
                if (item.ToString().ToLower().Equals(value.Trim().ToLower())) return item;
            }

            return defaultValue;
        }
        public static string ToDescription(this Enum value)
        {
            var attributes = (DescriptionAttribute[])value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static IEnumerable<SelectListItem> ToSelectList(this System.Enum enumValue, IStringLocalizer localizer = null, string localizerPrefix = null, int? selected = null)
        {
            return (from System.Enum e in System.Enum.GetValues(enumValue.GetType())
                    let t = e.GetType()
                    select new SelectListItem
                    {
                        Selected = (Convert.ToInt16(e) == selected),
                        Text = localizer == null ? e.ToDescription() : localizer[localizerPrefix + "." + e],
                        Value = Convert.ToInt16(e).ToString()
                    }).ToList();
        }

        /// <summary>
        /// convert items in struct to SelectList
        /// </summary>
        public static List<SelectListItem> ToSelectList(this Type type, IStringLocalizer localizer, string localizerPrefix)
        {
            List<string> values = type.GetAllPublicConstantValues<string>();
            List<SelectListItem> selectList = new List<SelectListItem>();
            foreach (var i in values)
            {
                selectList.Add(new SelectListItem
                {
                    Text = localizer[localizerPrefix + "." + i].Value,
                    Value = i
                });
            }

            return selectList;
        }
        public static bool AreCharsNumber(string str, params char[] exceptions)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            exceptions = exceptions ?? new char[] { };

            foreach (var c in str)
            {
                if (!char.IsDigit(c) && !exceptions.Any(x => x == c))
                {
                    return false;
                }
            }

            return true;
        }


        public static string GetRandomAlphanumericString(int length)
        {
            const string alphanumericCharacters =
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                "abcdefghijklmnopqrstuvwxyz" +
                "0123456789";
            return GetRandomString(length, alphanumericCharacters);
        }

        public static string GetRandomNumericString(int length)
        {
            const string alphanumericCharacters =
                "0123456789";
            return GetRandomString(length, alphanumericCharacters);
        }

        public static string GetRandomString(int length, IEnumerable<char> characterSet)
        {
            if (length < 0)
                throw new ArgumentException("length must not be negative", "length");
            if (length > int.MaxValue / 8) // 250 million chars ought to be enough for anybody
                throw new ArgumentException("length is too big", "length");
            if (characterSet == null)
                throw new ArgumentNullException("characterSet");
            var characterArray = characterSet.Distinct().ToArray();
            if (characterArray.Length == 0)
                throw new ArgumentException("characterSet must not be empty", "characterSet");

            var bytes = new byte[length * 8];
            new RNGCryptoServiceProvider().GetBytes(bytes);
            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                ulong value = BitConverter.ToUInt64(bytes, i * 8);
                result[i] = characterArray[value % (uint)characterArray.Length];
            }
            return new string(result);
        }

        public static List<T> GetAllPublicConstantValues<T>(this Type type)
        {
            return type
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(T))
                .Select(x => (T)x.GetRawConstantValue())
                .ToList();
        }
        
        public static string Commafy3Digit(this decimal value)
        {
            return value.ToString("#,##0").Replace(" ", ",");
        }

        public static string Sha256(string input)
        {
            var crypt = new SHA256Managed();
            var hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(input));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        public static string ToBase64Encode(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string FromBase64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public static string TryFromBase64Decode(this string base64EncodedData)
        {
            try
            {
                var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch 
            {
                return "";
            }            
        }
        public static bool IsMD5(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return false;
            }

            return Regex.IsMatch(input, "^[0-9a-fA-F]{32}$", RegexOptions.Compiled);
        }
    }
}
