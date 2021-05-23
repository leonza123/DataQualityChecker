using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;
using System.Threading.Tasks;

namespace DataQualityChecker.Helpers
{
    public class FileHelper
    {
        public static bool SaveFile(IFormFile file, string targetFolder, string fileName)
        {
            if (file == null || file.Length == 0 || string.IsNullOrEmpty(targetFolder))
                return false;

            CreateDirectory(targetFolder);
            var path = Path.Combine(targetFolder, fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return true;
        }

        public static bool SaveFile(IFormFile file, string fullPath)
        {
            if (file == null || file.Length == 0 || string.IsNullOrEmpty(fullPath))
                return false;

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return true;
        }

        public static async Task<MemoryStream> DownloadFile(string fileName, string directory)
        {
            var path = Path.Combine(directory, fileName);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;
            return memory;
        }

        public static string GetContentType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(fileName, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }

        public static void CreateDirectory(string directoryPath) 
        {
            if (!Directory.Exists(directoryPath)) 
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        //CSV helper - to get files with encoding
        public class Utf8ForExcelCsvResult : IActionResult
        {
            public string Content { get; set; }
            public string ContentType { get; set; }
            public string FileName { get; set; }
            public Task ExecuteResultAsync(ActionContext context)
            {
                var Response = context.HttpContext.Response;
                Response.Headers["Content-Type"] = this.ContentType;
                Response.Headers["Content-Disposition"] = $"attachment; filename={this.FileName}; filename*=UTF-8''{this.FileName}";
                using (var sw = new StreamWriter(Response.Body, System.Text.Encoding.UTF8))
                {
                    sw.Write(Content);
                }
                return Task.CompletedTask;
            }
        }
    }
}
