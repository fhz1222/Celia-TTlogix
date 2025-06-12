using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Net.Http.Headers;
using ServiceResult;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TT.Services.Services.Utilities;

namespace TT.Controllers.Controllers
{
    public abstract class TTLogixControllerBase : ControllerBase
    {
        public override ActionResult ValidationProblem([ActionResultObjectValue] ModelStateDictionary modelState)
        {
            if (!modelState.IsValid)
            {
                foreach (var key in modelState.Keys)
                {
                    var errors = modelState[key].Errors.Select(e => e.ErrorMessage).ToList();
                    modelState.Remove(key);
                    foreach (var error in errors)
                    {
                        modelState.AddModelError(key, new JsonResultError(error).ToJson());
                    }
                }
            }
            return base.ValidationProblem(modelState);
        }

        public ActionResult FromResult<T>(Result<T> result)
            => result.ResultType switch
            {
                ResultType.Ok => Ok(result.Data),
                ResultType.NotFound => NotFound(result.Errors),
                ResultType.Invalid => BadRequest((result.GetType() == typeof(ComplexInvalidResult<T>)) ? ((ComplexInvalidResult<T>)result).ComplexErrors : result.Errors),
                ResultType.Unexpected => BadRequest(result.Errors),
                ResultType.Unauthorized => Unauthorized(),
                _ => throw new Exception("An unhandled result has occurred as a result of a service call."),
            };

        public ActionResult FromStreamResult<T>(Result<T> result, string fileName, string fileType) where T : Stream
        {
            return result.ResultType switch
            {
                ResultType.Ok => new FileStreamResult(result.Data, fileType) { FileDownloadName = fileName },
                ResultType.NotFound => NotFound(result.Errors[0]),
                ResultType.Invalid => BadRequest((result.GetType() == typeof(ComplexInvalidResult<T>)) ? ((ComplexInvalidResult<T>)result).ComplexErrors[0] : result.Errors[0]),
                ResultType.Unexpected => BadRequest(result.Errors[0]),
                ResultType.Unauthorized => Unauthorized(),
                _ => throw new Exception("An unhandled result has occurred as a result of a service call."),
            };
        }

        protected FileStreamResult ToTextFileStreamResult(IEnumerable<string> data, string fileName)
        {
            var stream = new MemoryStream();
            var outputFile = new StreamWriter(stream);

            foreach (var line in data)
                outputFile.WriteLine(line);

            outputFile.Flush();
            stream.Position = 0;

            var result = new FileStreamResult(stream, MediaTypeHeaderValue.Parse("text/plain"))
            {
                FileDownloadName = fileName
            };
            return result;
        }

        protected FileStreamResult ToCSVFileStreamResult<T>(IEnumerable<T> data, string fileName)
        {
            var stream = new MemoryStream();
            var outputFile = new StreamWriter(stream);


            var csv = new CsvWriter(outputFile, CultureInfo.InvariantCulture);

            csv.WriteRecords(data);

            outputFile.Flush();
            stream.Position = 0;

            var result = new FileStreamResult(stream, MediaTypeHeaderValue.Parse("text/plain"))
            {
                FileDownloadName = fileName
            };
            return result;
        }
    }
}
