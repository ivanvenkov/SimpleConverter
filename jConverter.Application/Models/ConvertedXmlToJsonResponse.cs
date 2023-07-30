namespace jConverter.Application.Models
{
    public class ConvertedXmlToJsonResponse : IConvertedResponse
    {
        public  string FileData { get; set; }
        public  string Extension { get => "json"; }
    }
}
