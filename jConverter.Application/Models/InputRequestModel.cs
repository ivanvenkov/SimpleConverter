using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace jConverter.Application.Models
{
    public class InputRequestModel
    {
        [JsonPropertyName("file")]
        public IFormFile File { get; set; }

        [JsonPropertyName("filename")]
        public string FileName { get; set; }

        [JsonPropertyName("convertertype")]
        public string ConverterType { get; set; }        
    }
}