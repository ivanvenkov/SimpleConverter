using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace jConverter.Application.Models
{
    public class InputRequestModel
    {
        [JsonPropertyName("file")]
        public IFormFile File { get; set; }

        [JsonPropertyName("fileName")]
        public string FileName { get; set; }

        [JsonPropertyName("converterType")]
        public string ConverterType { get; set; }        
    }
}