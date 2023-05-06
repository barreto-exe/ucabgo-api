using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using UcabGo.Core.Data;

namespace UcabGo.Application.Utils
{
    public static class ObjectExtensions
    {
        public static readonly JsonSerializerSettings JsonSerializerSettings = new()
        {
            DateFormatString = "yyyy-MM-dd hh:mm:ss",
            Culture = System.Globalization.CultureInfo.GetCultureInfo("en-US")
        };

        public static T ToObject<T>(this IEnumerable<KeyValuePair<string, StringValues>> source)
            where T : class, new()
        {
            var items = new Dictionary<string, string>();

            foreach (var item in source)
            {
                items.Add(item.Key, item.Value);
            }

            var json = JsonConvert.SerializeObject(items, JsonSerializerSettings);

            var someObject = JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);

            return someObject;
        }


        public static T ToObject<T>(this Stream source)
            where T : class, new()
        {
            var json = new StreamReader(source).ReadToEnd();

            var someObject = JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);

            return someObject;
        }

        public static async Task<T> ToObjectAsync<T>(this Stream source)
            where T : class, new()
        {
            var json = await new StreamReader(source).ReadToEndAsync();

            var someObject = JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);

            return someObject;
        }

        public static bool IsValid(this BaseRequest model, out List<ValidationResult> validationResults)
        {
            return Validator.TryValidateObject(model, new ValidationContext(model), validationResults = new(), true);
        }

        public static string GetDescription(this Enum e)
        {
            var attribute =
                e.GetType()
                    .GetTypeInfo()
                    .GetMember(e.ToString())
                    .FirstOrDefault(member => member.MemberType == MemberTypes.Field)
                    .GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .SingleOrDefault()
                    as DescriptionAttribute;

            return attribute?.Description ?? e.ToString();
        }
    }
}
