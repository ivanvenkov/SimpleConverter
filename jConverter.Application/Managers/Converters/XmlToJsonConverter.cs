using jConverter.Application.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace jConverter.Application.Managers.Converters
{
    public class XmlToJsonConverter : IConverter
    {
        public async Task<IConvertedResponse> Convert(IFormFile file)
        {
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                string xmlData = await reader.ReadToEndAsync();
                XDocument xDocument = XDocument.Parse(xmlData);

                // Implement XML to JSON conversion using Newtonsoft.Json's SerializeXNode method
                string jsonData = JsonConvert.SerializeXNode(xDocument);
                return new ConvertedXmlToJsonResponse
                {
                    FileData = jsonData
                };
            }
        }       
    }
}
