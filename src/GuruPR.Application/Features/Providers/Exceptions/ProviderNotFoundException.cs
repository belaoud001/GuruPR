using GuruPR.Application.Common.Exceptions;

namespace GuruPR.Application.Features.Providers.Exceptions;

public class ProviderNotFoundException(string message) : NotFoundException(message);
