using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ICMarkets.Blockcypher.Application.DataObjects.Helpers
{
    public class JsonResolvers
    {
        public class IgnoreBaseContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

                // Filter to keep only properties declared directly on the target type
                return properties.Where(p => p.DeclaringType == type).ToList();
            }
        }
    }
}
