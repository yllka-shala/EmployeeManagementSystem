using EMSBaseLibrary.DTOs;
using EMSBaseLibrary.Entities;
using EMSBaseLibrary.Responses;
using EMSServer.Data;
using EMSServer.Helpers;
using EMSServer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EMSServer.Repositories.Implementations
{
    public class AccountRepository(IOptions<JwtSection> config, AppDbContext appDbContext) : IAccountRepository
    {
        public async Task<GeneralResponse> CreateAsync(Register user)
        {
            if (user == null)
                return new GeneralResponse(false, "Model is empty!");

            var checkUser = await FindUserByEmail(user.Email!);
            if (checkUser != null)
                return new GeneralResponse(false, "User is registered already!");

            var roleId = await GetRoleId();
            if (roleId == 0)
            {
                return new GeneralResponse(false, "No role was found!");
            }

            //Save User
            var applicationUser = await AddToDatabase(new User()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                RoleId = roleId,
                IsActive = true,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password)
            });

            return new GeneralResponse(true, "Account Created!");
        }

        public async Task<LoginResponse> SignInAsync(Login user)
        {
            if (user is null)
                return new LoginResponse(false, "Model is empty!");

            var applicationUser = await FindUserByEmail(user.Email!);
            if (applicationUser is null)
                return new LoginResponse(false, "User not found!");

            //Verify password
            if (!BCrypt.Net.BCrypt.Verify(user.Password, applicationUser.Password))
                return new LoginResponse(false, "Email/Password not valid!");

            var getUserRole = await FindUserRole(applicationUser.Id);
            if (getUserRole is null)
                return new LoginResponse(false, "User Role not found!");

            var getRoleName = await FindRoleName(getUserRole.Id);
            if (getRoleName is null)
                return new LoginResponse(false, "User Role not found!");

            string jwtToken = GenerateToken(applicationUser, getRoleName!.Name);
            string refreshToken = GenerateRefreshToken();

            //Save the refresh token to the database
            var findUser = await appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(a => a.UserId == applicationUser.Id);
            if (findUser is not null)
            {
                findUser!.Token = refreshToken;
                await appDbContext.SaveChangesAsync();
            }
            else
            {
                await AddToDatabase(new RefreshTokenInfo()
                {
                    Token = refreshToken,
                    UserId = applicationUser.Id
                });
            }

            return new LoginResponse(true, "Login successfully", jwtToken, refreshToken);
        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshToken token)
        {
            if (token is null)
                return new LoginResponse(false, "Model is empty!");

            var findToken = await appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(a => a.Token!.Equals(token.Token));
            if (findToken is null)
                return new LoginResponse(false, "Refresh token is required!");

            //get user details
            var user = await appDbContext.Users.FirstOrDefaultAsync(a => a.Id == findToken.UserId);
            if (user is null)
                return new LoginResponse(false, "Refresh token could not be generated because user not found!");

            var userRole = await FindUserRole(user.Id);
            var roleName = await FindRoleName(userRole.Id);
            string jwtToken = GenerateToken(user, roleName.Name);
            string refreshToken = GenerateRefreshToken();

            var updateRefreshToken = await appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(a => a.UserId == user.Id);
            if (updateRefreshToken is null)
                return new LoginResponse(false, "Refresh token could not be generated because user has not signed in!");

            updateRefreshToken.Token = refreshToken;
            await appDbContext.SaveChangesAsync();

            return new LoginResponse(true, "Token refreshed successfully!", jwtToken, refreshToken);
        }

        private string GenerateToken(User user, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Value.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: config.Value.Issuer,
                audience: config.Value.Audience,
                claims: userClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        private async Task<Roles> FindUserRole(int userId) => await appDbContext.Roles.FirstOrDefaultAsync(a => a.Id == userId);

        private async Task<Roles> FindRoleName(int roleId) => await appDbContext.Roles.FirstOrDefaultAsync(a => a.Id == roleId);
        private async Task<int> GetRoleId()
        {
            bool adminExists = await appDbContext.Users.AnyAsync(r => r.Role.Name == Constants.Admin.ToString());

            if (!adminExists)
            {
                // If no admin exists, assign the new user as an admin
                await AddToDatabase(new Roles() { Name = Constants.Admin.ToString() });
            }
            else
            {
                // If an admin already exists, ensure the user role is present in the database
                bool userRoleExists = await appDbContext.Roles.AnyAsync(r => r.Name == Constants.User.ToString());
                if (!userRoleExists)
                {
                    await AddToDatabase(new Roles() { Name = Constants.User.ToString() });
                }
            }

            string roleName = adminExists ? Constants.User.ToString() : Constants.Admin.ToString();

            int roleId = await appDbContext.Roles.Where(r => r.Name == roleName).Select(r => r.Id).FirstOrDefaultAsync();

            return roleId;
        }

        private async Task<User> FindUserByEmail(string email)
        {
            return await appDbContext.Users.FirstOrDefaultAsync(a => a.Email!.ToLower()!.Equals(email!.ToLower()));
        }

        private async Task<T> AddToDatabase<T>(T model)
        {
            var result = appDbContext.Add(model);
            await appDbContext.SaveChangesAsync();
            return (T)result.Entity;
        }
    }
}
