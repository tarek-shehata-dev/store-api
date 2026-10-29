using Microsoft.AspNetCore.Identity;
using Store.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Repository.Identity
{
    public static class StoreIdentityDbContextSeed
    {
        public async static Task SeedAppUserAsync(UserManager<AppUser> _userManager)
        {
            if(_userManager.Users.Count() == 0)   
            {
                var user = new AppUser()
                {
                    Email = "admin@store.com",
                    DisplayName = "Admin User",
                    UserName = "admin",
                    PhoneNumber = "01000000000",
                    Address = new Address { FName = "Admin", LName = "User", City = "Cairo", Country = "Egypt", Street = "Main St" }
                };
                await _userManager.CreateAsync(user, "P@ssW0rd");
            }

        }
    }
}
