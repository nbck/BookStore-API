using System.IO;
using System.Threading.Tasks;

namespace BookStore_UI.WASM.Contracts
{
    public interface IFileUpload
    {
        public Task UploadFile(MemoryStream msFile, string picName);
        public void RemoveFile(string picName);
    }
}