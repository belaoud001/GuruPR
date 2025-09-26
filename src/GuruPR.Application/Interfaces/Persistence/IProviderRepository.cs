using GuruPR.Domain.Entities.OAuth;

namespace GuruPR.Application.Interfaces.Persistence;

public interface IProviderRepository : IGenericRepository<Provider>
{
    // TODO: Think of eager and lazy loading here.
}
