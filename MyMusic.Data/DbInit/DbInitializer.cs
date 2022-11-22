using Microsoft.AspNetCore.Identity;
using MyMusic.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMusic.Data.DbInit
{
    public static class DbInitializer
    {
        public static async Task Initialize(UserManager<User> userManager)
        {
            if(!userManager.Users.Any())
            {
                var user = new User
                {
                    UserName = "bob",
                    Email = "bob@test.com"
                };

                await userManager.CreateAsync(user,"1234Aa..");
                await userManager.AddToRoleAsync(user, "Member");

                var admin = new User
                {
                    UserName = "admin",
                    Email = "admin@test.com"
                };

                await userManager.CreateAsync(admin, "1234Aa..");
                await userManager.AddToRoleAsync(admin, "Admin");

            }
        }

    }
}