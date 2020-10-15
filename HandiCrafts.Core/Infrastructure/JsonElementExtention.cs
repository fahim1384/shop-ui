using System;
using System.Globalization;
using System.Text.Json;

namespace HandiCrafts.Core.Infrastructure
{
    public static class JsonElementExtentions
    {
        public static T ToObject<T>(this JsonElement element)
        {
            var json = element.GetRawText();
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
        public static T ToObject<T>(this JsonDocument document)
        {
            var json = document.RootElement.GetRawText();
            return JsonSerializer.Deserialize<T>(json);
        }

    }
}
