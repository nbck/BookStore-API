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

        public async Task UploadFile(MemoryStream msFile, string picName)
        {
            try
            {
                string path = $"{_environment.WebRootPath}\\uploads\\{picName}";

                using (var fs = new FileStream(path, FileMode.Create))
                {
                    msFile.WriteTo(fs);
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