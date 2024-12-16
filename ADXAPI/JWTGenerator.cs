using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ADXAPI
{
    public class JWTGenerator : IJWTGenerator
    {
        private readonly IConfiguration config;
        public JWTGenerator(IConfiguration configuration)
        {
            config = configuration;
        }
        public string GenerateToken(string email, bool adxAccess)
        {

            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()),

            };

            if (adxAccess)
            {
                claims.Add(new Claim(ClaimTypes.Role, "ADXUser"));
            }
      
            var key = Encoding.UTF8.GetBytes(config["JwtSettings:Key"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(1),
                Issuer = "https://localhost:44397/",
                Audience = "https://localhost:44397/",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
