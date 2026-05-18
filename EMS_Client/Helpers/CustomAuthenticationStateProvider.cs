using EMSBaseLibrary.DTOs;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EMS_Client.Helpers
{
    public class CustomAuthenticationStateProvider(SessionService sessionService) : AuthenticationStateProvider
    {
        private readonly ClaimsPrincipal anonymous = new(new ClaimsIdentity());
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var stringToken = await sessionService.GetToken();
            if (string.IsNullOrEmpty(stringToken))
                return await Task.FromResult(new AuthenticationState(anonymous));

            var deserializeToken = JsonHelper.DeserializeJsonString<UserSession>(stringToken);
            if (deserializeToken == null)
                return await Task.FromResult(new AuthenticationState(anonymous));

            var getUserClaims = DecryptToken(deserializeToken.Token);
            if (getUserClaims == null)
                return await Task.FromResult(new AuthenticationState(anonymous));

            var claimsPrincipal = SetClaimsPrincipal(getUserClaims);

            return await Task.FromResult(new AuthenticationState(claimsPrincipal));
        }

        private static CustomUserClaims DecryptToken(string jwtToken)
        {
            if (string.IsNullOrEmpty(jwtToken))
                return new CustomUserClaims();

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);
            var userId = token.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier);
            var name = token.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Name);
            var email = token.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Email);
            var role = token.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Role);

            return new CustomUserClaims(userId!.Value!, name!.Value, email!.Value, role!.Value);
        }

        public static ClaimsPrincipal SetClaimsPrincipal(CustomUserClaims claims)
        {
            if (claims.Email is null)
                return new ClaimsPrincipal();

            return new ClaimsPrincipal(new ClaimsIdentity(
            new List<Claim>
            {
                new(ClaimTypes.NameIdentifier,claims.Id),
                new(ClaimTypes.Name,claims.Name),
                new(ClaimTypes.Email,claims.Email),
                new(ClaimTypes.Role,claims.Role),

            }, "JwtAuth"));
        }

        public async Task UpdateAuthenticationState(UserSession userSession)
        {
            var claimsPrincipal = new ClaimsPrincipal();
            if (userSession.Token != null || userSession.RefreshToken != null)
            {
                var serializeSession = JsonHelper.SerializeObj(userSession);
                await sessionService.SetToken(serializeSession);
                var getUserClaims = DecryptToken(userSession.Token!);
                claimsPrincipal = SetClaimsPrincipal(getUserClaims);
            }
            else
            {
                await sessionService.RemoveToken();
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }
    }
}
