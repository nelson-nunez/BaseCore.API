using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading;
using Microsoft.Extensions.Hosting;

namespace BaseRest.Core.API.Common
{
    public interface IJwtAuthManager
    {
        IImmutableDictionary<string, RefreshToken> UsersRefreshTokensReadOnlyDictionary { get; }
        JwtAuthResult GenerateTokens(string username, Claim[] claims, DateTime now);
        JwtAuthResult GenerateTokens(string username, Claim[] claims, DateTime now, int accessTokenExpiration);
        JwtAuthResult Refresh(string refreshToken, string accessToken, DateTime now);

        void RemoveExpiredRefreshTokens(DateTime now);
        void RemoveRefreshTokenByUserName(string userName);
        (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token);
        (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token, bool validateLifetime);
    }

    public class JwtAuthManager : IJwtAuthManager
    {
        public IImmutableDictionary<string, RefreshToken> UsersRefreshTokensReadOnlyDictionary => usersRefreshTokens.ToImmutableDictionary();
        private readonly ConcurrentDictionary<string, RefreshToken> usersRefreshTokens;  // can store in a database or a distributed cache
        private readonly JwtTokenConfig jwtTokenConfig;
        private readonly byte[] secret;

        public JwtAuthManager()
        {

            jwtTokenConfig = AppConfiguration.GetConfigurationSection<JwtTokenConfig>("JwtTokenConfig");
            usersRefreshTokens = new ConcurrentDictionary<string, RefreshToken>();
            secret = Encoding.ASCII.GetBytes(jwtTokenConfig.Secret);
        }

        // optional: clean up expired refresh tokens
        public void RemoveExpiredRefreshTokens(DateTime now)
        {
            var expiredTokens = usersRefreshTokens.Where(x => x.Value.ExpireAt < now).ToList();
            foreach (var expiredToken in expiredTokens)
            {
                usersRefreshTokens.TryRemove(expiredToken.Key, out _);
            }
        }

        // can be more specific to ip, user agent, device name, etc.
        public void RemoveRefreshTokenByUserName(string userName)
        {
            var refreshTokens = usersRefreshTokens.Where(x => x.Value.UserName == userName).ToList();
            foreach (var refreshToken in refreshTokens)
            {
                usersRefreshTokens.TryRemove(refreshToken.Key, out _);
            }
        }

        public void RemoveRefreshTokenByOld(string oldRefreshToken)
        {
            RefreshToken rt = null;
            var deleted = usersRefreshTokens.TryRemove(oldRefreshToken, out rt);

        }

        public JwtAuthResult GenerateTokens(string username, Claim[] claims, DateTime now)
        {
            return GenerateTokens(username, claims, now, jwtTokenConfig.AccessTokenExpiration);
        }

        public JwtAuthResult GenerateTokens(string username, Claim[] claims, DateTime now, int accessTokenExpiration)
        {
            var claimList = new List<Claim>(claims);
            //claimList.Add(new Claim("AccessTokenExpiration", accessTokenExpiration.ToString()));
            var shouldAddAudienceClaim = string.IsNullOrWhiteSpace(claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value);
            var jwtToken = new JwtSecurityToken(
                jwtTokenConfig.Issuer,
                shouldAddAudienceClaim ? jwtTokenConfig.Audience : string.Empty,
                claimList.ToArray(),
                expires: now.AddMinutes(accessTokenExpiration),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature));
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            var refreshToken = new RefreshToken
            {
                UserName = username,
                TokenString = GenerateRefreshTokenString(),
                ExpireAt = now.AddMinutes(jwtTokenConfig.RefreshTokenExpiration)
            };
            usersRefreshTokens.AddOrUpdate(refreshToken.TokenString, refreshToken, (s, t) => refreshToken);

            return new JwtAuthResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public JwtAuthResult RenewAccessToken(RefreshToken refreshToken, Claim[] claims, DateTime now, int accessTokenExpiration)
        {
            var claimList = new List<Claim>(claims);
            //claimList.Add(new Claim("AccessTokenExpiration", accessTokenExpiration.ToString()));
            var shouldAddAudienceClaim = string.IsNullOrWhiteSpace(claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value);
            var jwtToken = new JwtSecurityToken(
                jwtTokenConfig.Issuer,
                shouldAddAudienceClaim ? jwtTokenConfig.Audience : string.Empty,
                claimList.ToArray(),
                expires: now.AddMinutes(accessTokenExpiration),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature));
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return new JwtAuthResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        public JwtAuthResult Refresh(string refreshToken, string accessToken, DateTime now)
        {
            var (principal, jwtToken) = DecodeJwtToken(accessToken, false);
            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
            {
                throw new SecurityTokenException("Invalid token");
            }

            var userName = principal.Identity.Name;
            if (!usersRefreshTokens.TryGetValue(refreshToken, out var existingRefreshToken))
            {
                throw new SecurityTokenException("Invalid token");
            }
            if (existingRefreshToken.UserName != userName || existingRefreshToken.ExpireAt < now)
            {
                throw new SecurityTokenException("Invalid token");
            }

            return RenewAccessToken(existingRefreshToken, principal.Claims.ToArray(), now, jwtTokenConfig.AccessTokenExpiration);

            //RemoveRefreshTokenByOld(refreshToken);
            //return GenerateTokens(userName, principal.Claims.ToArray(), now, jwtTokenConfig.AccessTokenExpiration); // need to recover the original claims
        }

        public (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token, bool validateLifetime)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new SecurityTokenException("Invalid token");
            }
            var principal = new JwtSecurityTokenHandler()
                .ValidateToken(token,
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtTokenConfig.Issuer,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(secret),
                        ValidAudience = jwtTokenConfig.Audience,
                        ValidateAudience = true,
                        ValidateLifetime = validateLifetime,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    },
                    out var validatedToken);
            return (principal, validatedToken as JwtSecurityToken);
        }

        public (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token)
        {
            return DecodeJwtToken(token, true);
        }

        private static string GenerateRefreshTokenString()
        {
            var randomNumber = new byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public class JwtRefreshTokenCache : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IJwtAuthManager _jwtAuthManager;

        public JwtRefreshTokenCache(IJwtAuthManager jwtAuthManager)
        {
            _jwtAuthManager = jwtAuthManager;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            // remove expired refresh tokens from cache every minute
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _jwtAuthManager.RemoveExpiredRefreshTokens(DateTime.Now);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
