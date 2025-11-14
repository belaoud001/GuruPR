using GuruPR.Application.Common.Exceptions;

namespace GuruPR.Application.Features.Users.Exceptions;

public class UserNotFoundException(string message) : NotFoundException(message);
