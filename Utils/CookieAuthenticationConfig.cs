using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TCU.English.Controllers;

namespace TCU.English.Utils
{
    public static class CookieAuthenticationConfig
    {
        public static readonly string ReturnUrlParameter = "RequestPath";
        public static void CustomCookieAuthentication(this IServiceCollection services)
        {

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Account/Access");
                options.Cookie = new CookieBuilder
                {
                    //Domain = "",
                    HttpOnly = true,
                    Name = ".TCU.English.Security.Cookie",
                    Path = "/",
                    SameSite = SameSiteMode.Lax,
                    SecurePolicy = CookieSecurePolicy.SameAsRequest,
                };
                options.ExpireTimeSpan = TimeSpan.FromMinutes(Config.MAX_COOKIE_LIFE_MINUTES); // Thời hạn đăng nhập là 10 năm
                options.Events = new CookieAuthenticationEvents
                {
                    OnSignedIn = context =>
                    {
                        //Console.WriteLine("{0} - {1}: {2}", DateTime.Now,
                        //    "OnSignedIn", context.Principal.Identity.Name);
                        return Task.CompletedTask;
                    },
                    OnSigningOut = context =>
                    {
                        //Console.WriteLine("{0} - {1}: {2}", DateTime.Now,
                        //    "OnSigningOut", context.HttpContext.User.Identity.Name);
                        return Task.CompletedTask;
                    },
                    OnValidatePrincipal = context =>
                    {
                        //Console.WriteLine("{0} - {1}: {2}", DateTime.Now,
                        //    "OnValidatePrincipal", context.Principal.Identity.Name);
                        return Task.CompletedTask;
                    }
                };
                // Thời gian giới hạn cho một phiên đăng nhập
                //options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
                options.LoginPath = new PathString($"/{NameUtils.ControllerName<AuthenticationController>()}");
                options.ReturnUrlParameter = ReturnUrlParameter;
                options.SlidingExpiration = true;
            });
        }
    }
}
