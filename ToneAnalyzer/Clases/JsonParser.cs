using Newtonsoft.Json;
using System.Collections.Generic;

namespace ToneAnalyzer.Clases
{
    /// <summary>
    /// Representa una respuesta en JSON de Tone Analizer.
    /// </summary>
    public partial class JsonToneResponse {
        [JsonProperty("document_tone")]
        public DocumentTone DocumentTone { get; set; }

        [JsonProperty("sentences_tone")]
        public List<SentencesTone> SentencesTone { get; set; }

        public JsonToneResponse(string response) {
            var deserializedResponse = JsonConvert.DeserializeObject<JsonToneResponse>(response);

            DocumentTone = deserializedResponse.DocumentTone;
            SentencesTone = deserializedResponse.SentencesTone;
        }
    }

    public partial class DocumentTone {
        [JsonProperty("tone_categories")]
        public List<ToneCategory> ToneCategories { get; set; }
    }

    public partial class ToneCategory {
        [JsonProperty("tones")]
        public List<Tone> Tones { get; set; }

        [JsonProperty("category_id")]
        public string CategoryId { get; set; }

        [JsonProperty("category_name")]
        public string CategoryName { get; set; }
    }

    public partial class Tone {
        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("tone_id")]
        public string ToneId { get; set; }

        [JsonProperty("tone_name")]
        public string ToneName { get; set; }
    }

    public partial class SentencesTone {
        [JsonProperty("sentence_id")]
        public long SentenceId { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("tone_categories")]
        public List<ToneCategory> ToneCategories { get; set; }

        [JsonProperty("input_from")]
        public long InputFrom { get; set; }

        [JsonProperty("input_to")]
        public long InputTo { get; set; }
    }

    public partial class JsonToneResponse {
        public static JsonToneResponse FromJson(string json) {
            return JsonConvert.DeserializeObject<JsonToneResponse>(json, Converter.Settings);
        }
    }

    public static class Serialize {
        public static string ToJson(this JsonToneResponse self) {
            return JsonConvert.SerializeObject(self, Converter.Settings);
        }
    }

    public class Converter {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}
