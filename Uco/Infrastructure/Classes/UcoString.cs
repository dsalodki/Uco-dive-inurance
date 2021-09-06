using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Uco.Infrastructure
{
    public static class UcoString
    {
        public static List<string> String2List(string value)
        {
            List<string> Items = new List<string>();
            if (!string.IsNullOrEmpty(value))
            {
                foreach (string item in value.Split('|').ToList())
                {
                    if (string.IsNullOrEmpty(item)) continue;
                    Items.Add(item);
                }
            }
            return Items;
        }
        public static string List2String(List<string> Items)
        {
            if (Items.Count == 0) return string.Empty;
            return string.Format("|{0}|", String.Join("|", Items.Select(r => r).ToArray()));
        }

        public static string RemoveHTMLTagsFromString(string Text)
        {
            string noHTML = Regex.Replace(Text, @"<[^>]+>|&nbsp;", "").Trim();
            return Regex.Replace(noHTML, @"\s{2,}", " ");
        }

        public static string ReplaceBrByDot(string Text)
        {
            return Text.Replace("<br />", ". ").Replace("<br/>", ". ").Replace("<br>", ". ");
        }

        public static string ClearForCSV(string Text)
        {
            if (string.IsNullOrEmpty(Text)) return string.Empty;
            return Text.Replace("\"", "“").Replace(",", ".");
        }

        public static string GetUtf8String(string notUtf8String)
        {
            string OutValue = "";
            byte[] encodedBytes = System.Text.Encoding.Default.GetBytes(notUtf8String);
            string notUtf8StringEncoded = System.Text.Encoding.UTF8.GetString(encodedBytes);

            if (notUtf8StringEncoded.Contains("�")) OutValue = notUtf8String;
            else notUtf8String = notUtf8StringEncoded;

            return notUtf8String;
        }

        public static string GetDescription<T>(this T enumerationValue)
          where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();

        }

        public static class EnumHelper<T>
        {
            public static IList<T> GetValues(Enum value)
            {
                var enumValues = new List<T>();

                foreach (FieldInfo fi in value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public))
                {
                    enumValues.Add((T)Enum.Parse(value.GetType(), fi.Name, false));
                }
                return enumValues;
            }

            public static T Parse(string value)
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }

            public static IList<string> GetNames(Enum value)
            {
                return value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToList();
            }

            public static IList<string> GetDisplayValues(Enum value)
            {
                return GetNames(value).Select(obj => GetDisplayValue(Parse(obj))).ToList();
            }

            public static string GetDisplayValue(T value)
            {
                var fieldInfo = value.GetType().GetField(value.ToString());

                var descriptionAttributes = fieldInfo.GetCustomAttributes(
                    typeof(DisplayAttribute), false) as DisplayAttribute[];

                if (descriptionAttributes == null) return string.Empty;
                return (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString();
            }
        }
    }

}