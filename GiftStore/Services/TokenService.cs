using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GiftStore.Services
{
    //public class TokenService
    //{
    //    private readonly JwtConfig _jwtConfig;

    //    public TokenService(IOptions<JwtConfig> jwtConfig)
    //    {
    //        _jwtConfig = jwtConfig.Value;
    //    }

    //    public string GenerateToken(string username)
    //    {
    //        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));
    //        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    //        var claims = new[]
    //        {
    //            new Claim(ClaimTypes.Name, username),
    //            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    //        };

    //        var token = new JwtSecurityToken(
    //            issuer: _jwtConfig.Issuer,
    //            audience: _jwtConfig.Issuer,
    //            claims: claims,
    //            expires: DateTime.Now.AddMinutes(60), // Token expiry time (e.g., 60 minutes)
    //            signingCredentials: credentials
    //        );

    //        return new JwtSecurityTokenHandler().WriteToken(token);
    //    }
    //}
}
