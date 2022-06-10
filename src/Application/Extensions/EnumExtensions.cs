using System;
using System.ComponentModel;

namespace Dreamrosia.Koin.Application.Extensions
{
    public static class EnumExtensions
    {
        public static string ToDescriptionString(this Enum val)
        {
            if (val is null)
            {
                return string.Empty;
            }
            else
            {
                var attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);

                return attributes.Length > 0 ?
                       attributes[0].Description :
                       val.ToString();
            }
        }


        public static T GetValueFromDescription<T>(string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                    {
                        return (T)field.GetValue(null);
                    }
                }
                else
                {
                    if (field.Name == description)
                    {
                        return (T)field.GetValue(null);
                    }
                }
            }

            //throw new ArgumentException("Not found.", nameof(description));
            return default(T);
        }
    }
}