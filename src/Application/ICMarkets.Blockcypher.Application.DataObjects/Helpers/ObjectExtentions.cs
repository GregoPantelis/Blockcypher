using Newtonsoft.Json.Linq;

namespace ICMarkets.Blockcypher.Application.DataObjects.Helpers
{
    public static class ObjectExtentions
    {
        public static JObject ToJObject(this object obj)
        {
            return JObject.FromObject(obj);
        }
    }
}
