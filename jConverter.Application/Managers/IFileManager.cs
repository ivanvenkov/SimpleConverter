using jConverter.Application.Models;

namespace jConverter.Application.Managers
{
    public interface IFileManager
    {
        Task<string> Upload(InputRequestModel request);
    }
}