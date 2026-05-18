using EMSBaseLibrary.DTOs;
using EMSBaseLibrary.Responses;

namespace EMSServer.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<GeneralResponse> CreateAsync(Register user);
        Task<LoginResponse> SignInAsync(Login user);
        Task<LoginResponse> RefreshTokenAsync(RefreshToken token);
    }
}
