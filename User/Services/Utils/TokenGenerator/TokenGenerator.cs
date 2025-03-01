using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserApi.Data.Models;

namespace UserApi.Services.Utils.TokenGenerator
{
    public class TokenGenerator
    {
        private readonly TokenGeneratorConfiguration _configuration;
        public TokenGenerator(TokenGeneratorConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateAccessToken(Guid UserId, Role Role)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim("Id", UserId.ToString()),
                new Claim("TokenType", "Access"),
                new Claim(ClaimTypes.Role, Role.ToString())
            };

            DateTime expires = DateTime.UtcNow.AddSeconds(_configuration.AccessTokenExpirationSeconds);

            SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.AccessTokenSecret));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                _configuration.Issuer,
                _configuration.Audience,
                claims,
                DateTime.UtcNow,
                expires: expires,
                credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken(Guid UserId)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim("Id", UserId.ToString()),
                new Claim("TokenType", "Refresh"),
            };

            DateTime expires = DateTime.UtcNow.AddSeconds(_configuration.RefreshTokenExpirationHours);

            SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.AccessTokenSecret));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                _configuration.Issuer,
                _configuration.Audience,
                claims,
                DateTime.UtcNow,
                expires: expires,
                credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
