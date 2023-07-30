using jConverter.Application.Exceptions;
using jConverter.Application.Managers.Converters;
using jConverter.Application.Models;
using JConverter.Data;
using JConverter.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace jConverter.Application.Managers
{
    public class FileManager : IFileManager
    {
        private readonly ConverterDbContext dbContext;
        private readonly IConfiguration configuration;

        public FileManager(ConverterDbContext dbContext, IConfiguration configuration) 
            => (this.dbContext, this.configuration) = (dbContext, configuration);

        public async Task<string> Upload(InputRequestModel request)
        {
            var path = this.configuration.GetSection("FilePathSettings")["Path"];

            if (request is null)
                   throw new Exception("The received InputRequestModel is null.");

            var convertedResult = await this.ConvertFile(request);
                var convertedFileName = this.GetFileNameWithExtension(request.FileName, convertedResult.Extension);

                if (this.FileNameExists(path, convertedFileName))
                    throw new FileExistsException($"File with name \"{convertedFileName}\" already exists on the server");

                this.dbContext.Files.Add(new FileEntity
                {
                    OriginalFile = await this.ConvertIFormFileToString(request.File),
                    OriginalFileName = request.FileName.ToLower(),
                    CreatedOn = DateTime.Now,
                    ConvertedFile = convertedResult.FileData,
                    ConvertedFileName = convertedFileName
                });

                await this.dbContext.SaveChangesAsync();

                await this.SaveJsonToFileAsync(convertedResult.FileData, convertedFileName, path);

                return convertedResult.FileData;
        }

        private bool FileNameExists(string path, string fileName) => File.Exists(Path.Combine(path, fileName));

        private string GetFileNameWithExtension(string filename, string extension) => Path.ChangeExtension(filename, extension);

        private async Task SaveJsonToFileAsync(string jsonData, string filename, string path)
        {
            string filePath = Path.Combine(path, filename);
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                await writer.WriteAsync(jsonData);
            }
        }

        private async Task<IConvertedResponse> ConvertFile(InputRequestModel request)
        {
            var type = request.ConverterType;
            var converterStrategy = Assembly
                            .GetExecutingAssembly()
                            .GetTypes()
                            .Where(t => typeof(IConverter).IsAssignableFrom(t)
                            && t.IsClass
                            && t.Name.Contains(type)).FirstOrDefault();

            if (converterStrategy == null)
                throw new ArgumentException($"There is no '{converterStrategy}' currently in this version of the File Converter.");

            var converter = (IConverter)Activator.CreateInstance(converterStrategy);
            var convertedResult = await converter.Convert(request.File);
            return convertedResult;
        }

        private async Task<string> ConvertIFormFileToString(IFormFile formFile)
        {
            using (StreamReader reader = new StreamReader(formFile.OpenReadStream()))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}