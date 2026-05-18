using EMSBaseLibrary.DTOs;
using EMSBaseLibrary.Entities;
using EMSBaseLibrary.Responses;
using EMSServer.Helpers;

namespace EMSServer.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetUsers();
        Task<GeneralResponse> UpdateUser(User user);
        Task<GeneralResponse> DeleteUser(int id);
        Task<string> GetUserImage(int id);
        Task<GeneralResponse> UpdateProfile(UserProfile profile);
        Task<GeneralResponse> DeleteProfilePicture(int id);
    }
}
