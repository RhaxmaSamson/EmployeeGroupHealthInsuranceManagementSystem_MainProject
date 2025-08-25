using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EmployeeHealthInsurance.Data;
using System.Threading.Tasks;

public static class DbInitializer
{
    public static async Task Initialize(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        context.Database.Migrate();

        string hrManagerRole = "HRManager";
        string adminRole = "Admin";
        string employeeRole = "Employee";

        if (await roleManager.FindByNameAsync(hrManagerRole) == null)
            await roleManager.CreateAsync(new IdentityRole(hrManagerRole));
        if (await roleManager.FindByNameAsync(adminRole) == null)
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        if (await roleManager.FindByNameAsync(employeeRole) == null)
            await roleManager.CreateAsync(new IdentityRole(employeeRole));

        // HR 1
        string adminUserEmail = "hr.manager@gmail.com";
        var hr1 = await userManager.FindByNameAsync(adminUserEmail);
        if (hr1 == null)
        {
            hr1 = new IdentityUser { UserName = adminUserEmail, Email = adminUserEmail, EmailConfirmed = true };
            var result = await userManager.CreateAsync(hr1, "SecurePassword123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(hr1, hrManagerRole);
                await userManager.AddToRoleAsync(hr1, adminRole);
            }
        }
        else
        {
            if (!hr1.EmailConfirmed)
            {
                hr1.EmailConfirmed = true;
                await userManager.UpdateAsync(hr1);
            }
            if (!await userManager.IsInRoleAsync(hr1, hrManagerRole))
                await userManager.AddToRoleAsync(hr1, hrManagerRole);
        }

        // HR 2 (confirmed)
        string anotherHrEmail = "hr.cts@cognizant.com";
        var hr2 = await userManager.FindByNameAsync(anotherHrEmail);
        if (hr2 == null)
        {
            hr2 = new IdentityUser { UserName = anotherHrEmail, Email = anotherHrEmail, EmailConfirmed = true };
            var result = await userManager.CreateAsync(hr2, "CtsHR12!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(hr2, hrManagerRole);
            }
        }

        // Admin
        string adminEmail = "admin@company.com";
        var admin = await userManager.FindByNameAsync(adminEmail);
        if (admin == null)
        {
            admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            var result = await userManager.CreateAsync(admin, "Admin#12345");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, adminRole);
            }
        }

        // Employee
        string employeeEmail = "employee@company.com";
        var emp = await userManager.FindByNameAsync(employeeEmail);
        if (emp == null)
        {
            emp = new IdentityUser { UserName = employeeEmail, Email = employeeEmail, EmailConfirmed = true };
            var result = await userManager.CreateAsync(emp, "Employee#12345");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(emp, employeeRole);
            }
        }
    }
}