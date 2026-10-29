using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Store.APIs.Middlewares;
using Store.Core.Entities.Identity;
using Store.Repository.Data;
using Store.Repository.Data.Contexts;
using Store.Repository.Identity;
using Store.Repository.Identity.Contexts;
using System.Threading.Tasks;

namespace Store.APIs.Helper
{
    public static class ConfigureMiddleware
    {
        public static async Task<WebApplication> ConfigureMiddlewareAsync(this WebApplication app)
        {

            using var scope = app.Services.CreateScope();

            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<StoreDbContext>();
            var identityContext = services.GetRequiredService<StoreIdentityDbContext>();
            var userManager= services.GetRequiredService<UserManager<AppUser>>();

            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            try
            { 
                await context.Database.MigrateAsync();
                await StoreDbContextSeed.SeedAsync(context);
                await identityContext.Database.MigrateAsync();
                await StoreIdentityDbContextSeed.SeedAppUserAsync(userManager);
            }

            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, " there are problems during apply migrations! ");
            }


            app.UseMiddleware<ExceptionMiddleware>();

            
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStatusCodePagesWithReExecute("/error/{0}");
            app.UseStaticFiles();
            app.UseCors("FrontendPolicy");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            

            return app;
        }
    }
}
