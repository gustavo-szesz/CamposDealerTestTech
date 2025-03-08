using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Models
{
    public class MicrosoftDateConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string value = reader.GetString() ?? string.Empty;
                if (!string.IsNullOrEmpty(value))
                {
                    // Tenta converter usando o formato de data da Microsoft (/Date(milliseconds)/)
                    if (value.StartsWith("/Date(") && value.EndsWith(")/"))
                    {
                        string milliseconds = value.Substring(6, value.Length - 8);
                        if (long.TryParse(milliseconds, out long ticks))
                        {
                            return DateTimeOffset.FromUnixTimeMilliseconds(ticks).DateTime;
                        }
                    }
                    
                    // Tenta converter outros formatos de data
                    if (DateTime.TryParse(value, out DateTime result))
                    {
                        return result;
                    }
                }
            }
            
            return DateTime.Now; // Valor padrão se não conseguir converter
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("O", CultureInfo.InvariantCulture));
        }
    }
}