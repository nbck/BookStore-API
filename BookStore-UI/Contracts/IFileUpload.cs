using System.IO;
using System.Threading.Tasks;

namespace BookStore_UI.Contracts
{
    public interface IFileUpload
    {
        public Task UploadFile(Stream msFile, string picName);
        public void RemoveFile(string picName);
    }
}