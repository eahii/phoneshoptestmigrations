// File: UsedPhonesWebShop/frontend/Services/ILoginService.cs
using System.Threading.Tasks;

namespace frontend.Services
{
    public interface ILoginService
    {
        Task<bool> Login(string email, string password);
        Task Logout();
    }
}