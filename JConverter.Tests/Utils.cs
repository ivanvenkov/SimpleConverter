using FakeItEasy;
using JConverter.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace JConverter.Tests
{
    public static class Utils
    {
        public static DbContextOptions<ConverterDbContext> GetOptions(string dbName)
        {
            return new DbContextOptionsBuilder<ConverterDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }

        public static IConfiguration GetFakeConfiguration(string tempDirectoryPath)
        {
            var fakeConfiguration = A.Fake<IConfiguration>();
            var fakeConfigurationSection = A.Fake<IConfigurationSection>();
            A.CallTo(() => fakeConfiguration.GetSection("FilePathSettings")).Returns(fakeConfigurationSection);
            A.CallTo(() => fakeConfigurationSection["Path"]).Returns(tempDirectoryPath);
            return fakeConfiguration;
        }

        public static IFormFile CreateMockIFormFile(string content)
        {
            byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(content);
            return new FormFile(new MemoryStream(fileBytes), 0, fileBytes.Length, "file", "testfile.xml");
        }
    }
}
