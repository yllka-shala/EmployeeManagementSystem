using EMSBaseLibrary.DTOs;
using EMSBaseLibrary.Entities;
using EMSBaseLibrary.Responses;
using EMSServer.Data;
using EMSServer.Helpers;
using EMSServer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EMSServer.Repositories.Implementations
{
    public class UserRepository(AppDbContext appDbContext,  IFileRepository fileService) : IUserRepository
    {

        public async Task<GeneralResponse> UpdateUser(User user)
        {
            var existingUser = await appDbContext.Users.FindAsync(user.Id);
            if (existingUser == null)
            {
                return new GeneralResponse(false, "User not found!");
            }

            var exist = await appDbContext.Users.AnyAsync(c => c.Email.ToLower() == user.Email.ToLower() && c.Id != user.Id);
            if (exist)
            {
                return new GeneralResponse(false, "Email already exists!");
            }

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.RoleId = user.RoleId;
            existingUser.IsActive = user.IsActive;

            await appDbContext.SaveChangesAsync();
            return new GeneralResponse(true, "User role updated successfully!");
        }

        public async Task<GeneralResponse> DeleteUser(int id)
        {
            var user = await appDbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            appDbContext.Users.Remove(user!);
            await appDbContext.SaveChangesAsync();
            return new GeneralResponse(true,"User successfully deleted!");
        }
        public async Task<List<User>> GetUsers() => await appDbContext.Users.AsNoTracking().Include(u => u.Role).ToListAsync();

        public async Task<string> GetUserImage(int id) 
        {
            var result = (await GetUsers()).FirstOrDefault(u => u.Id == id)!.ProfilePicture;
            return result; 
        }

        public async Task<GeneralResponse> UpdateProfile(UserProfile userProfile)
        {
            var user = await appDbContext.Users.FirstOrDefaultAsync(u => u.Id == userProfile.Id);
            if (user == null)
            {
                return new GeneralResponse(false, "User not found!");
            }

            var exist = await appDbContext.Users.AnyAsync(c => c.Email.ToLower() == userProfile.Email.ToLower() && c.Id != userProfile.Id);
            if (exist)
            {
                return new GeneralResponse(false, "Email already exists!");
            }

            if (!string.IsNullOrEmpty(userProfile.Base64ImageData))
            {
                var fileName = await SaveFile(userProfile.Base64ImageData, userProfile.ProfilePictureFileName);

                //Delete the old picture from folder
                if (!string.IsNullOrEmpty(user.ProfilePicture))
                {
                    fileService.DeleteFile(user.ProfilePicture);
                }

                user.ProfilePicture = fileName;
            }

            user.Email = userProfile.Email;

            await appDbContext.SaveChangesAsync();
            return new GeneralResponse(true, "Profile updated successfully!");
        }

        public async Task<GeneralResponse> DeleteProfilePicture(int id)
        {
            if (id == 0)
                return new GeneralResponse(false, "Invalid Id!");

            var existingUser = await appDbContext.Users.FindAsync(id);
            if (existingUser == null)
            {
                return new GeneralResponse(false, "User not found!");
            }

            if (string.IsNullOrEmpty(existingUser.ProfilePicture))
            {
                return new GeneralResponse(false, "Profile picture is already null.");
            }

            try
            {
                fileService.DeleteFile(existingUser.ProfilePicture);
                existingUser.ProfilePicture = null;

                await appDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new GeneralResponse(false,"Failed to update user profile due to a database error.");
            }

            return new GeneralResponse(true, "Profile picture deleted successfully!");
        }

        private async Task<string> SaveFile(string base64ImageData, string userFileName)
        {
            string base64Data = base64ImageData;
            if (base64Data.Contains(","))
            {
                base64Data = base64Data.Split(',')[1];
            }

            // Convert Base64 string to byte array
            var bytes = Convert.FromBase64String(base64Data);

            // Generate a safe, unique filename
            var extension = !string.IsNullOrEmpty(userFileName)
                            ? Path.GetExtension(userFileName)
                            : ".jpg";

            var fileName = $"{Guid.NewGuid()}{extension}";

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            var filePath = Path.Combine(folderPath, fileName);

            // Ensure the directory exists on the server
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Write the bytes to the physical file
            await File.WriteAllBytesAsync(filePath, bytes);

            return fileName;
        }
    }
}
