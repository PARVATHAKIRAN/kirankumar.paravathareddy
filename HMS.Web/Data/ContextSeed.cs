using HMS.Web.Data.Entities;
using HMS.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static HMS.Web.Enums.Enum;

namespace HMS.Web.Data
{
    public static class ContextSeed
    {
        public static void SeedAdminUserAsync(IApplicationBuilder app)
        {
            //Seed User
            
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<HMSDbContext>();
                context.Database.EnsureCreated();

                var _userManager = serviceScope.ServiceProvider.GetService<UserManager<IdentityUser>>();
                var _roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();


                if (!context.Users.Any(usr => usr.UserName == "Admin@test.com"))
                {
                    var user = new IdentityUser()
                    {
                        UserName = "Admin@test.com",
                        Email = "Admin@test.com",
                        EmailConfirmed = false,
                    };

                    var userResult = _userManager.CreateAsync(user, "Test@123").Result;
                }

                if (!_roleManager.RoleExistsAsync(Enums.Enum.Roles.Admin.ToString()).Result ||
                    !_roleManager.RoleExistsAsync(Enums.Enum.Roles.Doctor.ToString()).Result ||
                    !_roleManager.RoleExistsAsync(Enums.Enum.Roles.Patient.ToString()).Result)
                {
                    var Adminrole = _roleManager.CreateAsync(new IdentityRole { Name = Roles.Admin.ToString() }).Result;
                    var Doctorrole = _roleManager.CreateAsync(new IdentityRole { Name = Enums.Enum.Roles.Doctor.ToString() }).Result;
                    var Patientrole = _roleManager.CreateAsync(new IdentityRole { Name = Enums.Enum.Roles.Patient.ToString() }).Result;

                }

                var adminUser = _userManager.FindByNameAsync("Admin@test.com").Result;
                var userRole = _userManager.AddToRolesAsync(adminUser, new string[] { Enums.Enum.Roles.Admin.ToString() }).Result;

				
				context.SaveChanges();
            }
        }
    }
}
