using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace KsWare.AppVeyorClient.Shared.Json {

	public class OnlyDeclaredMembersSerializerContractResolver : DefaultContractResolver {

		private readonly Dictionary<Type, HashSet<string>> _includes;

		public OnlyDeclaredMembersSerializerContractResolver() {
			_includes = new Dictionary<Type, HashSet<string>>();
		}

		public void Include<T>() {
			var type = typeof(T);
			if (!_includes.ContainsKey(type)) {
				_includes[type] = new HashSet<string>();
				var jsonPropertyNames = typeof(T)
					.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Where(IsSerializable)
					.Select(p => p.Name).ToArray();

				foreach (var prop in jsonPropertyNames) _includes[type].Add(prop);
			}
		}

		private static bool IsSerializable(PropertyInfo p) { return p.GetCustomAttribute<JsonIgnoreAttribute>() == null; }

		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {
			var property = base.CreateProperty(member, memberSerialization);

			if (IsIncluded(property.DeclaringType, property.PropertyName)) 
				property.ShouldSerialize  = i => true;
			else property.ShouldSerialize = i => false;

			return property;
		}

		private bool IsIncluded(Type type, string jsonPropertyName) {
			if (!_includes.ContainsKey(type)) return false; 
			return _includes[type].Contains(jsonPropertyName);
		}
	}

}