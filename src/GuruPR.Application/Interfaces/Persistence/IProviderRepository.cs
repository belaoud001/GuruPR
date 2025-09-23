using GuruPR.Domain.Entities.OAuth;

namespace GuruPR.Application.Interfaces.Persistence;

public interface IProviderRepository
{
    Task SaveAsync(Provider provider);
}
