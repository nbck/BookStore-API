using System;
using System.IO;
using System.Threading.Tasks;
using BookStore_UI.Contracts;
using Microsoft.AspNetCore.Hosting;

namespace BookStore_UI.Services
{
    public class FileUpload : IFileUpload
    {
        private readonly IWebHostEnvironment _environment;

        public FileUpload(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task UploadFile(Stream msFile, string picName)
        {
            try
            {
                string path = $"{_environment.WebRootPath}\\uploads\\{picName}";
                var buffer = new byte[4 * 1096];
                int totalRead = 0;
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    int bytesRead;
                    while ((bytesRead = await msFile.ReadAsync(buffer)) != 0)
                    {
                        totalRead += bytesRead;
                        await fs.WriteAsync(buffer);
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void RemoveFile(string picName)
        {
            string path = $"{_environment.WebRootPath}\\uploads\\{picName}";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}