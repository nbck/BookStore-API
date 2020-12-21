using System;
using System.IO;
using System.Threading.Tasks;
using BookStore_UI.WASM.Contracts;

namespace BookStore_UI.WASM.Services
{
    //public class FileUpload : IFileUpload
    //{
    //    private readonly IWebHostEnvironment _environment;

    //    public FileUpload(IWebHostEnvironment environment)
    //    {
    //        _environment = environment;
    //    }

    //    public async Task UploadFile(MemoryStream msFile, string picName)
    //    {
    //        try
    //        {
    //            string path = $"{_environment.WebRootPath}\\uploads\\{picName}";

    //            using (var fs = new FileStream(path, FileMode.Create))
    //            {
    //                msFile.WriteTo(fs);
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            throw;
    //        }
    //    }

    //    public void RemoveFile(string picName)
    //    {
    //        string path = $"{_environment.WebRootPath}\\uploads\\{picName}";
    //        if (ImageData.Exists(path))
    //        {
    //            ImageData.Delete(path);
    //        }
    //    }
    //}
}