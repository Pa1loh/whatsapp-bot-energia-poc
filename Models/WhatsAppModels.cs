using System.Text.Json.Serialization;

namespace WhatsAppBot.Models;
public record WhatsAppWebhookPayload(
    [property: JsonPropertyName("entry")] List<Entry> Entry
);
public record Entry(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("changes")] List<Change> Changes
);
public record Change(
    [property: JsonPropertyName("value")] Value Value,
    [property: JsonPropertyName("field")] string Field
);
public record Value(
    [property: JsonPropertyName("messaging_product")] string MessagingProduct,
    [property: JsonPropertyName("metadata")] Metadata Metadata,
    [property: JsonPropertyName("messages")] List<Message>? Messages
);
public record Metadata(
    [property: JsonPropertyName("display_phone_number")] string DisplayPhoneNumber,
    [property: JsonPropertyName("phone_number_id")] string PhoneNumberId
);
public record Message(
    [property: JsonPropertyName("from")] string From,
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("timestamp")] string Timestamp,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("text")] Text? Text,
    [property: JsonPropertyName("interactive")] Interactive? Interactive
);
public record Text(
    [property: JsonPropertyName("body")] string Body
);
public record Interactive(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("button_reply")] ButtonReply? ButtonReply
);
public record ButtonReply(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("title")] string Title
);
public record BotaoResposta(string Id, string Titulo);