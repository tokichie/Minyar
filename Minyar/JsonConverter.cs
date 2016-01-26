using Newtonsoft.Json;

namespace Minyar {
    public static class JsonConverter {
        public static T Deserialize<T>(string json) {
            var resolver = new PrivateSetterContractResolver();
            var serializeSettings = new JsonSerializerSettings {ContractResolver = resolver};
            return JsonConvert.DeserializeObject<T>(json, serializeSettings);
        }

        public static string Serialize(object obj) {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
