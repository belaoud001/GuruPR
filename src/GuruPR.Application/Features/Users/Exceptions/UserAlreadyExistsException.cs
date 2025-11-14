namespace GuruPR.Application.Features.Users.Exceptions;

public class UserAlreadyExistsException(string message) : Exception(message);
