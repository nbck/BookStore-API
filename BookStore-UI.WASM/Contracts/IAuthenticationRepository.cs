using System.Threading.Tasks;
using BookStore_UI.WASM.Models;

namespace BookStore_UI.WASM.Contracts
{
    public interface IAuthenticationRepository
    {
         Task<bool> Register(RegistrationModel user);
         Task<bool> Login(LoginModel user);
         Task Logout();
    }
}