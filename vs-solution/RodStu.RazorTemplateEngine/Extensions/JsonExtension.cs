using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Json = Newtonsoft.Json;

namespace System
{
    public static class JsonExtension
    {
        public static string ToJson(this object obj, bool formatted = false, Dictionary<Type, string[]> ignoreTypes = null)
        {
            Json.Formatting formatting = Json.Formatting.None;
            Json.JsonSerializerSettings settings = new Json.JsonSerializerSettings();
            settings.NullValueHandling = Json.NullValueHandling.Ignore;
            settings.Converters.Add(new CustomDateTimeConverter());

            if (ignoreTypes != null && ignoreTypes.Count > 0)
            {
                IgnorableSerializerContractResolver ignore = new IgnorableSerializerContractResolver();
                foreach (var igT in ignoreTypes)
                {
                    if (igT.Value != null && igT.Value.Count() > 0)
                        ignore.Ignore(igT.Key, igT.Value.ToArray());
                    else
                        ignore.Ignore(igT.Key);
                }
                settings.ContractResolver = ignore;
            }


            if (formatted)
                formatting = Json.Formatting.Indented;


            return Json.JsonConvert.SerializeObject(obj, formatting, settings);
        }
    }


    #region Json.NET Extended
    /// <summary>
    /// Special JsonConvert resolver that allows you to ignore properties.  See http://stackoverflow.com/a/13588192/1037948
    /// For this implementation, see http://stackoverflow.com/a/14510134/524063
    /// </summary>
    public class IgnorableSerializerContractResolver : Json.Serialization.DefaultContractResolver
    {
        protected readonly Dictionary<Type, HashSet<string>> Ignores;

        /// <summary>
        /// CTOR
        /// </summary>
        public IgnorableSerializerContractResolver()
        {
            this.Ignores = new Dictionary<Type, HashSet<string>>();
        }

        /// <summary>
        /// Explicitly ignore the given property(s) for the given type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName">one or more properties to ignore.  Leave empty to ignore the type entirely.</param>
        public void Ignore(Type type, params string[] propertyName)
        {
            // start bucket if DNE
            if (!this.Ignores.ContainsKey(type)) this.Ignores[type] = new HashSet<string>();

            foreach (var prop in propertyName)
            {
                this.Ignores[type].Add(prop);
            }
        }

        /// <summary>
        /// Is the given property for the given type ignored?
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool IsIgnored(Type type, string propertyName)
        {
            if (!this.Ignores.ContainsKey(type)) return false;

            // if no properties provided, ignore the type entirely
            if (this.Ignores[type].Count == 0) return true;

            return this.Ignores[type].Contains(propertyName);
        }

        /// <summary>
        /// The decision logic goes here
        /// </summary>
        /// <param name="member"></param>
        /// <param name="memberSerialization"></param>
        /// <returns></returns>
        protected override Json.Serialization.JsonProperty CreateProperty(MemberInfo member, Json.MemberSerialization memberSerialization)
        {
            Json.Serialization.JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (this.IsIgnored(property.DeclaringType, property.PropertyName)
            // need to check basetype as well for EF -- @per comment by user576838
            || this.IsIgnored(property.DeclaringType.BaseType, property.PropertyName))
            {
                property.ShouldSerialize = instance => { return false; };
            }

            return property;
        }
    }

    class CustomDateTimeConverter : Newtonsoft.Json.Converters.IsoDateTimeConverter
    {
        public CustomDateTimeConverter()
        {
            base.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        }
    }
    #endregion
}
