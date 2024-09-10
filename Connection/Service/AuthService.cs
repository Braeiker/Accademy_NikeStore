using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NikeStore.Models;
using NikeStore.Models.Dto;
using NikeStore.Models.Settings;

namespace NikeStore.Connection.Service
{
    public class AuthService
    {
        private readonly Identity _identitySettings;
        private readonly Jwt _jwtSettings;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthService(IOptions<Identity> identityOptions, IOptions<Jwt> jwtOptions, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _identitySettings = identityOptions.Value;
            _jwtSettings = jwtOptions.Value;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<TokenResponse> GenerateJwtAsync(string userName)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                {
                    throw new ArgumentException("User not found.");
                }

                var roles = await _userManager.GetRolesAsync(user);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id) // Adding user ID for uniqueness
                };

                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var expiry = DateTime.UtcNow.AddDays(_jwtSettings.ExpiryInDays);

                var token = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: expiry,
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return new TokenResponse { Token = tokenString, Expires = expiry };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while generating the JWT.", ex);
            }
        }
    }
}
