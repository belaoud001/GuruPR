using GuruPR.Application.Common.Exceptions;

namespace GuruPR.Application.Features.Users.Exceptions;

public class UserRoleOperationFailedException(string message) : OperationFailedException(message);
