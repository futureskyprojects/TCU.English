using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TCU.English.Data;
using TCU.English.Models;
using TCU.English.Models.Repository;
using TCU.English.Utils;

namespace TCU.English
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-3.1&tabs=visual-studio
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Thiết lập thêm CROS (https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-3.1)
            services.AddCors();

            // Thiết lập ngữ cảnh đến CSDL
            services.AddDbContext<SystemDatabaseContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            // Thêm các scope CSDL
            services.AddDataRepositoryScope();

            // Thiết lập cho phép mô hình MVC
            services.AddControllersWithViews();

            // Thiết lập chữ thường cho URL routing
            services.AddRouting(options => options.LowercaseUrls = true);

            // Bình thướng hóa url
            services.AddRouting(option =>
            {
                option.ConstraintMap["slugify"] = typeof(SlugifyParameterTransformer);
            });

            #region Cookie Authentication Configure
            // https://viblo.asia/p/su-dung-cookie-authentication-trong-aspnet-core-djeZ1VG8lWz
            services.CustomCookieAuthentication();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SystemDatabaseContext db)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCookiePolicy();
            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            // Khởi tạo CSDL
            DatabaseInitializer.Initialize(db);

            app.UseAuthentication(); // Bật xác thực
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: "AdminAreaRoute",
                    areaName: nameof(Areas.Admin),
                    pattern: nameof(Areas.Admin) + "/{controller:slugify=Login}/{action:slugify=Index}/{id:slugify?}");

                endpoints.MapAreaControllerRoute(
                    name: "default",
                    areaName: nameof(Areas.Guest),
                    pattern: "{controller:slugify}/{action:slugify}/{id:slugify?}",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}


#region Refs link
// https://tienganhmoingay.com/thong-tin-toeic/cau-truc-de-thi-toeic/#listening
// https://medium.com/@balramchavan/setup-entity-framework-core-for-mysql-in-asp-net-core-2-5b40a5a3af94
// https://adrientorris.github.io/aspnet-core/identity/extend-user-model.html
// https://stackoverflow.com/questions/51004516/net-core-2-1-identity-get-all-users-with-their-associated-roles
// https://docs.microsoft.com/en-us/ef/core/querying/related-data#including-multiple-levels
// https://adrientorris.github.io/aspnet-core/identity/extend-user-model.html
// https://stackoverflow.com/questions/10411529/package-manager-console-enable-migrations-commandnotfoundexception-only-in-a-spe
#endregion