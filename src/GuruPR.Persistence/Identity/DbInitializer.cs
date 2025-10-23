using GuruPR.Application.Exceptions;
using GuruPR.Application.Exceptions.Account;
using GuruPR.Application.Settings.Database;
using GuruPR.Domain.Entities;
using GuruPR.Domain.Enums;
using GuruPR.Domain.Extensions.User;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuruPR.Persistence.Identity;

public static class DbInitializer
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        await SeedRolesAsync(serviceProvider);
        await SeedAdminAsync(serviceProvider, configuration);
    }

    private static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        var roleNames = UserRoleExtensions.AllRoleNames();

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
        }
    }
    private static async Task SeedAdminAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        var admin = configuration.GetSection(DefaultAdminSettings.SectionName)
                                 .Get<DefaultAdminSettings>();

        if (admin == null)
        {
            throw new OperationFailedException("DefaultAdmin section is missing in configuration.");
        }

        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        var adminUser = await userManager.FindByEmailAsync(admin.Email);
        if (adminUser == null)
        {
            var user = new User
            {
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                UserName = admin.Email,
                Email = admin.Email,
                EmailConfirmed = true
            };

            var userCreationResult = await userManager.CreateAsync(user, admin.Password);
            if (userCreationResult.Succeeded)
            {
                IEnumerable<string> adminRoles = [UserRole.Admin.ToName(), UserRole.User.ToName()];
                var roleAssignementResult = await userManager.AddToRolesAsync(user, adminRoles);

                if (!roleAssignementResult.Succeeded)
                {
                    throw new UserRoleOperationFailedException(
                        "Failed to assign admin role when seeding: " + string.Join(", ", roleAssignementResult.Errors.Select(e => e.Description))
                    );
                }
            }
            else
            {
                throw new RegistrationFailedException("Failed to register admin user when seeding: " + string.Join(", ", userCreationResult.Errors.Select(e => e.Description)));
            }
        }
    }
}
