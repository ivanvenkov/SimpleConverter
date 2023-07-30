using jConverter.Application.Models;
using Microsoft.AspNetCore.Http;

namespace jConverter.Application.Managers.Converters
{
    public interface IConverter
    {
        Task<IConvertedResponse> Convert(IFormFile file);
    }
}
