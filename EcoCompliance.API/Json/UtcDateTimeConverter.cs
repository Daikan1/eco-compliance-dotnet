using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EcoCompliance.API.Json;

/// <summary>
/// Datas sempre em UTC no formato RFC 3339 (ex.: 2026-09-24T00:00:00.0000000Z).
/// O Oracle devolve DateTime sem fuso (Kind = Unspecified); sem este conversor
/// a API emitiria datas ambíguas, sem o sufixo de fuso horário.
/// </summary>
public sealed class UtcDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        ToUtc(reader.GetDateTime());

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
        writer.WriteStringValue(ToUtc(value).ToString("O", CultureInfo.InvariantCulture));

    private static DateTime ToUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc   => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _                  => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };
}
