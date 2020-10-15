using HandiCrafts.Core.Domain.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HandiCrafts.Web.Infrastructure.Security
{
    public class JwtTokenService
    {
        private UserManager userManager;
        private RoleManager<Role> roleManager;
        private SymmetricSecurityKey signingKey;
        public JwtTokenService(UserManager userManager, RoleManager<Role> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public async Task<ClaimsIdentity> GetUserClaims(User user)
        {
            var claims = new List<Claim>();
            var userClaims = await userManager.GetClaimsAsync(user);
            var userRoles = await userManager.GetRolesAsync(user);
            claims.AddRange(userClaims);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await roleManager.FindByNameAsync(userRole);
                if (role != null)
                {
                    claims.AddRange(await roleManager.GetClaimsAsync(role));
                }
            }
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim(ClaimTypes.Surname, user.FullName));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            //claims.Add(new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(new UserScoreSummary()
            //{ Score = user.Score, ConsumedScore = user.ConsumedScore })));
            // claims.Add(new Claim("ConsumedScore" ,user.ConsumedScore.ToString()));

            var claimIdentity = new ClaimsIdentity(claims.ToArray(), JwtBearerDefaults.AuthenticationScheme);
            return claimIdentity;
        }

        public async Task<JwtToken> GetJwtToken(ClaimsIdentity claimIdentity, TokenProviderOptions options)
        {
            var now = DateTime.UtcNow;
            var claims = claimIdentity.Claims.ToList();
            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, claimIdentity.GetUserName()),
                new Claim(JwtRegisteredClaimNames.Jti, await options.NonceGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(now).ToString(), ClaimValueTypes.Integer64)
            });

            var jwt = new JwtSecurityToken(
                issuer: options.Issuer,
                audience: options.Audience,
                claims: claims.ToArray(),
                notBefore: now,
                expires: now.Add(options.Expiration),
                signingCredentials: options.SigningCredentials);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var roles = string.Join(",", claims.Where(x => x.Type == ClaimTypes.Role).Select(m => m.Value));
            var response = new JwtToken()
            {
                AccessToken = encodedJwt,
                ExpiredIn = (int)options.Expiration.TotalSeconds,
                UserName = claimIdentity.GetUserName(),
                Name = IdentityExtensions.GetUserFullName(claimIdentity),
                // ScoreSummary = JsonConvert.DeserializeObject<UserScoreSummary>((claimIdentity.GetUserData())),
                // ConsumedScore= Convert.ToInt32((claimIdentity.GetCustomData("ConsumedScore")))
            };
            return response;

        }
        public static long ToUnixEpochDate(DateTime date) => new DateTimeOffset(date).ToUniversalTime().ToUnixTimeSeconds();
    }
}
