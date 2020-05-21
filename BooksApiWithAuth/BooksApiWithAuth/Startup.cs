using BooksApiWithAuth.Models;
using BooksApiWithAuth.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BooksApiWithAuth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region register Singletons

            //get key
            var jwtKey = Configuration.GetSection("BookstoreApplicationSettings:JwtTokenSecret").Value;

            //application settings from appsettings
            services.Configure<BookstoreApplicationSettings>(
                Configuration.GetSection(nameof(BookstoreApplicationSettings)));
            services.AddSingleton<IBookstoreApplicationSettings>(sp =>
                sp.GetRequiredService<IOptions<BookstoreApplicationSettings>>().Value);

            //db settings from appsettings
            services.Configure<BookstoreDatabaseSettings>(
                Configuration.GetSection(nameof(BookstoreDatabaseSettings)));
            services.AddSingleton<IBookstoreDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<BookstoreDatabaseSettings>>().Value);

            //users
            //services.AddSingleton<IApiUserService>(x =>
            //{
            //    var dbSettings = x.GetRequiredService<IBookstoreDatabaseSettings>();
            //    var appSettings = x.GetRequiredService<IBookstoreApplicationSettings>();
            //    return new ApiUserService(dbSettings, appSettings);
            //});
            services.AddSingleton<ApiUserService>();

            //books
            services.AddSingleton<BookService>();

            #endregion register Singletons

            #region configure misc

            services.AddControllers()
                .AddNewtonsoftJson(options => options.UseMemberCasing());

            #endregion configure misc

            #region authentication

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(y =>
               {
                   y.Events = new JwtBearerEvents
                   {
                       OnTokenValidated = context =>
                       {
                           //get username
                           var usernameFromToken = context.Principal.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;                           
                           var userService = context.HttpContext.RequestServices.GetRequiredService<ApiUserService>();
                           var user = userService.GetByUsername(usernameFromToken);
                           if (user == null)
                           {
                               // return unauthenticated if user no longer exists
                               context.Fail("Unauthenticated request");
                           }

                           //can also get role if required
                           //var roleFromToken = context.Principal.Claims.Where(x => x.Type == ClaimTypes.Role).FirstOrDefault().Value;

                           return Task.CompletedTask;
                       }
                   };
                   y.RequireHttpsMetadata = false;
                   y.SaveToken = true;
                   y.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey)),
                       ValidateIssuer = false,
                       ValidateAudience = false
                   };
               });

            #endregion authentication

            #region authorization

            services.AddAuthorization(options =>
            {
                //add a policy for the library team
                options.AddPolicy("LibraryTeam",
                     policy => policy.RequireRole("LibraryAdmin", "LibraryStaff"));
            });

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //to load the index.html page
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "StaticPages"))
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}