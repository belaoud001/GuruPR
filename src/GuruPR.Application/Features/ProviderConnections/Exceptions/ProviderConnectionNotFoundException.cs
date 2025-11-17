using GuruPR.Application.Common.Exceptions;

namespace GuruPR.Application.Features.ProviderConnections.Exceptions;

public class ProviderConnectionNotFoundException(string message) : NotFoundException(message);
