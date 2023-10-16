using HMS.Web.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reflection.Emit;
using HMS.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace HMS.Web.Data
{
    public class HMSDbContext : IdentityDbContext<IdentityUser>
    {
        public HMSDbContext(DbContextOptions<HMSDbContext> options)
       : base(options)
        {
        }

        //Identity
        //public override DbSet<IdentityUser> ApplicationUsers { get; set; }
        //public override DbSet<Role> Roles { get; set; }
        //public override DbSet<UserClaim> UserClaims { get; set; }
        //public override DbSet<UserRole> UserRoles { get; set; }
        //public override DbSet<UserLogin> UserLogins { get; set; }
        //public override DbSet<RoleClaim> RoleClaims { get; set; }
        //public override DbSet<UserToken> UserTokens { get; set; }

        //Doctor
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Schedule> Schedules { get; set; }

        //Patient
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.Entity<User>().ToTable("Users");
            //builder.Entity<Role>().ToTable("Roles");
            //builder.Entity<RoleClaim>().ToTable("RoleClaims");
            //builder.Entity<UserClaim>().ToTable("UserClaims");
            //builder.Entity<UserLogin>().ToTable("UserLogins");
            //builder.Entity<UserRole>().ToTable("UserRoles");
            //builder.Entity<UserToken>().ToTable("UserTokens");
        }
                
    }
}
