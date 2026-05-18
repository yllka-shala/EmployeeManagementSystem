using EMS_Client.Helpers;
using EMS_Client.Services.Interfaces;
using EMSBaseLibrary.DTOs;
using EMSBaseLibrary.Entities;
using EMSBaseLibrary.Responses;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace EMS_Client.Services.Implementations
{
    public class UserService(GetHttpClient getHttpClient) : IUserService
    {
        public const string AuthUrl = "api/authentication";
        public const string UsersUrl = "api/user";
        public async Task<GeneralResponse> CreateAsync(Register user)
        {
            var httpClient = getHttpClient.GetPublicHttpClient();
            var result = await httpClient.PostAsJsonAsync($"{AuthUrl}/register", user);
            if (!result.IsSuccessStatusCode)
                return new GeneralResponse(false, "Error Occured!");

            return await result.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<LoginResponse> SignInAsync(Login user)
        {
            var httpClient = getHttpClient.GetPublicHttpClient();
            var result = await httpClient.PostAsJsonAsync($"{AuthUrl}/login", user);
            if (!result.IsSuccessStatusCode)
                return new LoginResponse(false, "Error Occured!");

            return await result.Content.ReadFromJsonAsync<LoginResponse>();

        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshToken token)
        {
            var httpClient = getHttpClient.GetPublicHttpClient();
            var result = await httpClient.PostAsJsonAsync($"{AuthUrl}/refresh-token", token);
            if (!result.IsSuccessStatusCode)
                return new LoginResponse(false, "Error Occured!");

            return await result.Content.ReadFromJsonAsync<LoginResponse>();
        }

        public async Task<List<User>> GetUsers()
        {
            var httpClient = await getHttpClient.GetPrivateHttpClient();
            var result = await httpClient.GetFromJsonAsync<List<User>>($"{UsersUrl}/users");
            return result;
        }

        public async Task<GeneralResponse> UpdateUser(User user)
        {
            var httpClient = getHttpClient.GetPublicHttpClient();
            var result = await httpClient.PutAsJsonAsync($"{UsersUrl}/update-user", user);
            if (!result.IsSuccessStatusCode)
                return new GeneralResponse(false, "Error Occurred");

            return await result.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse> DeleteUser(int id)
        {
            var httpClient = await getHttpClient.GetPrivateHttpClient();
            var response = await httpClient.DeleteAsync($"{UsersUrl}/delete-user/{id}");
            if (!response.IsSuccessStatusCode)
                return new GeneralResponse(false, "Error Occurred!");

            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse> UpdateProfile(UserProfile userProfile)
        {
            var httpClient = await getHttpClient.GetPrivateHttpClient();
            var result = await httpClient.PutAsJsonAsync($"{UsersUrl}/update-profile", userProfile);
            if (!result.IsSuccessStatusCode)
                return new GeneralResponse(false, "Error Occurred");

            return await result.Content.ReadFromJsonAsync<GeneralResponse>();
        }
    }
}
