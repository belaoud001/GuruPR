using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Domain.Entities.OAuth;
using GuruPR.Persistence.Contexts;

namespace GuruPR.Persistence.Repositories;

public class ProviderRepository : GenericRepository<Provider>, IProviderRepository
{
    private readonly GuruDBContext _guruDBContext;

    public ProviderRepository(GuruDBContext guruDBContext) : base(guruDBContext)
    {
        _guruDBContext = guruDBContext;
    }
}
