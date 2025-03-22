using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Common
{
    public class AuthenticationConfiguration
    {
        public string AccessTokenSecret { get; set; } = "eGZ5RlduZXlalksdjf234Z2lqV1QzZkN1TU5sQnasdffad";
        public int AccessTokenExpirationSeconds { get; set; } = 600;
        public int RefreshTokenExpirationHours { get; set; } = 24;
        public string Issuer { get; set; } = "InternetBanking";
        public string Audience { get; set; } = "InternetBanking/api";
    }

    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services)
        {
            AuthenticationConfiguration tokenGeneratorConfiguration = new AuthenticationConfiguration();
            services.AddSingleton(tokenGeneratorConfiguration);

            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenGeneratorConfiguration.AccessTokenSecret)),
                        ValidIssuer = tokenGeneratorConfiguration.Issuer,
                        ValidAudience = tokenGeneratorConfiguration.Audience,
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true
                    };
                });

            return services;
        }
    }
}
