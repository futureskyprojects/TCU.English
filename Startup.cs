using GleamTech.AspNet.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TCU.English.Data;
using TCU.English.Hubs;
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
            // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?tabs=aspnetcore2x&view=aspnetcore-3.1#iis-configuration
            services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });

            // Out-of-process hosting model
            services.Configure<IISOptions>(options =>
            {
                options.ForwardClientCertificate = false;
            });

            // Thiết lập thêm CROS (https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-3.1)
            services.AddCors();

            // https://stackoverflow.com/questions/40523565/asp-net-core-x-frame-options-strange-behavior
            services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);

            // Thiết lập ngữ cảnh đến CSDL
            services.AddDbContext<SystemDatabaseContext>(options =>
                options
                .UseLazyLoadingProxies() // https://stackoverflow.com/questions/52024996/net-core-model-virtual-property-not-loaded
                .UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            // Thêm các scope CSDL
            services.AddDataRepositoryScope();

            // Thiết lập cho phép mô hình MVC
            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            // Thiết lập chữ thường cho URL routing
            services.AddRouting(options => options.LowercaseUrls = true);

            // Thêm SignalR phục vụ REALTIME
            services.AddSignalR();

            // Bình thướng hóa url
            services.AddRouting(option =>
            {
                option.ConstraintMap["slugify"] = typeof(SlugifyParameterTransformer);
            });

            #region Cookie Authentication Configure
            // https://viblo.asia/p/su-dung-cookie-authentication-trong-aspnet-core-djeZ1VG8lWz
            services.CustomCookieAuthentication();
            #endregion

            //Add GleamTech to the ASP.NET Core services container.
            //----------------------
            services.AddGleamTech();
            //----------------------
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SystemDatabaseContext db)
        {
            if (env.IsDevelopment())
            {
                //DatabaseInitializer.Initialize(db);
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //Register GleamTech to the ASP.NET Core HTTP request pipeline.
            //----------------------
            app.UseGleamTech();
            //----------------------

            //app.UseSession();
            app.UseCookiePolicy();
            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // Bật xác thực
            app.UseAuthorization();

            // Khởi tạo CSDL

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chathub");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller:slugify=Home}/{action:slugify=Index}/{id:slugify?}"
                    );
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