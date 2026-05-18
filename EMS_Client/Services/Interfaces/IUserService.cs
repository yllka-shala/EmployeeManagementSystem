using EMS_Client.Helpers;
using EMSBaseLibrary.DTOs;
using EMSBaseLibrary.Entities;
using EMSBaseLibrary.Responses;
using Microsoft.AspNetCore.Components.Forms;

namespace EMS_Client.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetUsers();
        Task<GeneralResponse> UpdateUser(User user);
        Task<GeneralResponse> DeleteUser(int id);
        Task<GeneralResponse> CreateAsync(Register user);
        Task<LoginResponse> SignInAsync(Login user);
        Task<LoginResponse> RefreshTokenAsync(RefreshToken token);
        Task<GeneralResponse> UpdateProfile(UserProfile userProfile);
    }
}
