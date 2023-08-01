# SimpleConverter
A simple file converter application.
A .NET7 application for converting and uploading files (currently only one conversion pair supported: XmlToJson). The applicaiton exposes an endpoint at "api/ConverterApi/upload-file" [POST] which expects to receive the following data:
file: the file to be converted;
fileName : the name of the file (string)
converterType : the type of conversion (string)
With successful response the API should return StatusCode 201 together with the converted object itself.
In case of failure a 400 BadRequest and the respective error information should be expected.
