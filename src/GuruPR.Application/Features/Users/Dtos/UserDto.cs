namespace GuruPR.Application.Features.Users.Dtos;

public class UserDto
{
    public Guid Id { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public string? Email { get; set; }

    public string? UserName { get; set; }

    public string FullName { get; set; } = string.Empty;
}