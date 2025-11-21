using GuruPR.Domain.Enums;

namespace GuruPR.Application.Common.Interfaces.Application;

public interface IUserRoleService
{
    Task AssignRoleAsync(string userId, UserRole userRole);

    Task RemoveRoleAsync(string userId, UserRole userRole);
}
