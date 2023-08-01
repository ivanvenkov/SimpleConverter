using FakeItEasy;
using jConverter.Application.Exceptions;
using jConverter.Application.Filters;
using jConverter.Application.Managers;
using jConverter.Application.Models;
using JConverter.Controllers;
using JConverter.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyTested.AspNetCore.Mvc;
using System.Text.Json;

namespace JConverter.Tests
{
    public class FileManagerTests
    {
        private string tempDirectoryPath;

        public FileManagerTests()
        {
            tempDirectoryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectoryPath);
        }

        [Fact]
        public async Task UploadFile_ValidRequest_SuccessfullyUploadsToDirecotry()
        {
            // Arrange
            IConfiguration fakeConfiguration = Utils.GetFakeConfiguration(tempDirectoryPath);

            var options = Utils.GetOptions(nameof(UploadFile_ValidRequest_SuccessfullyUploadsToDirecotry));
            using var dbContext = new ConverterDbContext(options);
            var fileManager = new FileManager(dbContext, fakeConfiguration);

            var xmlContent = @"<note> <to>Tove</to> <from>Jani</from> <heading>Reminder</heading> <body>Don't forget me this weekend!</body> </note> ";
            var request = new InputRequestModel
            {
                File = Utils.CreateMockIFormFile(xmlContent),
                FileName = "testfile.xml",
                ConverterType = "XmlToJsonConverter"
            };

            // Act
            var result = await fileManager.Upload(request);

            // Assert
            var savedFile = await dbContext.Files.FirstOrDefaultAsync();
            var filePath = Path.Combine(tempDirectoryPath, savedFile.ConvertedFileName);
            Assert.NotNull(filePath);
            Assert.True(File.Exists(filePath));
            var fileContent = await File.ReadAllTextAsync(filePath);
            Assert.Equal("{\"note\":{\"to\":\"Tove\",\"from\":\"Jani\",\"heading\":\"Reminder\",\"body\":\"Don't forget me this weekend!\"}}", fileContent);
        }
     
        [Fact]
        public async Task UploadFile_ExistingFile_ThrowsFileExistsException()
        {
            // Arrange
            IConfiguration fakeConfiguration = Utils.GetFakeConfiguration(tempDirectoryPath);

            var options = Utils.GetOptions(nameof(UploadFile_ExistingFile_ThrowsFileExistsException));
            using var dbContext = new ConverterDbContext(options);
            var fileManager = new FileManager(dbContext, fakeConfiguration);

            var xmlContent = @"<note> <to>Tove</to> <from>Jani</from> <heading>Reminder</heading> <body>Don't forget me this weekend!</body> </note> ";
            var request = new InputRequestModel
            {
                File = Utils.CreateMockIFormFile(xmlContent),
                FileName = "testfile.xml",
                ConverterType = "XmlToJsonConverter"
            };

            var existingFilePath = Path.Combine(tempDirectoryPath, "testfile.json");
            await File.WriteAllTextAsync(existingFilePath, "Sample content");

            // Act and Assert
            await Assert.ThrowsAsync<FileExistsException>(async () => await fileManager.Upload(request));

            File.Delete(existingFilePath);
        }


        [Fact]
        public async Task UploadFile_ValidRequest_ReturnsCreated()
        {
            IConfiguration fakeConfiguration = Utils.GetFakeConfiguration(tempDirectoryPath);

            var xmlContent = @"<note> <to>Tove</to> <from>Jani</from> <heading>Reminder</heading> <body>Don't forget me this weekend!</body> </note> ";
            var request = new InputRequestModel
            {
                File = Utils.CreateMockIFormFile(xmlContent),
                FileName = "testfile.xml",
                ConverterType = "XmlToJsonConverter"
            };

            var options = Utils.GetOptions(nameof(UploadFile_ValidRequest_ReturnsCreated));
            using var dbContext = new ConverterDbContext(options);
            var fileManager = new FileManager(dbContext, fakeConfiguration);

            MyController<ConverterApiController>.Instance(instance => instance.WithDependencies(fileManager))
                .Calling(c => c.UploadFile(With.Value<InputRequestModel>(request)))
                .ShouldHave()
                .ActionAttributes(attr => attr
                    .RestrictingForHttpMethod(MyTested.AspNetCore.Mvc.HttpMethod.Post))
                .AndAlso()
                .ShouldReturn()
                .StatusCode(201);
        }

        [Fact]
        public async Task OnActionExecutionAsync_InvalidRequest_ReturnsBadRequestWithErrorModelPayload()
        {
            // Arrange
            var loggerMock = A.Fake<ILogger<ValidationFilter>>();
            var filter = new ValidationFilter(loggerMock);
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();
            var actionDescriptor = new ControllerActionDescriptor();
            var routeData = new RouteData();
            var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);
            var context = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                A.Fake<Controller>());

            context.ModelState.AddModelError("ConverterType", "'Converter Type' must not be empty.");

            // Act
            await filter.OnActionExecutionAsync(context, NextActionExecutionDelegate);

            // Assert

            Assert.Equal(400, context.HttpContext.Response.StatusCode);

            var responseStream = httpContext.Response.Body as MemoryStream;
            responseStream.Position = 0;
            using var reader = new StreamReader(responseStream);
            var responseBody = await reader.ReadToEndAsync();
            var responseModel = JsonSerializer.Deserialize<ErrorModel>(responseBody, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            Assert.NotNull(responseModel);
            Assert.Equal("'Converter Type' must not be empty.", responseModel.ErrorMsg);
        }

        private Task<ActionExecutedContext> NextActionExecutionDelegate()
        {
            var httpContext = new DefaultHttpContext();
            var actionDescriptor = new ControllerActionDescriptor();
            var routeData = new RouteData();
            var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);
            return Task.FromResult(new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), A.Fake<Controller>()));
        }
    }
}