using EmployeeHealthInsurance.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Reflection.Emit;



namespace EmployeeHealthInsurance.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Policy> Policies { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Claim> Claims { get; set; }

        public DbSet<EnrollmentDependent> EnrollmentDependents { get; set; }    
 


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Required for Identity setup

            // ✅ Enum-to-string conversions for better readability in SQL
            //modelBuilder.Entity<Policy>()
            //    .Property(p => p.PolicyType)
            //    .HasConversion<string>();

            modelBuilder.Entity<Policy>()
                .Property(p => p.PolicyType)
                .HasConversion(new EnumToNumberConverter<PolicyType, int>());


            modelBuilder.Entity<Claim>()
                .Property(c => c.ClaimStatus)
                .HasConversion<string>();

            modelBuilder.Entity<Enrollment>()
                .Property(e => e.Status)
                .HasConversion<string>();

        }
    }


}