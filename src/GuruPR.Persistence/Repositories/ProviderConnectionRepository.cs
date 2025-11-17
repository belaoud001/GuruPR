using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Domain.Entities.OAuth;
using GuruPR.Persistence.Contexts;

namespace GuruPR.Persistence.Repositories;

public class ProviderConnectionRepository : GenericRepository<ProviderConnection>, IProviderConnectionRepository
{
    public ProviderConnectionRepository(GuruDbContext guruDBContext) : base(guruDBContext)
    {
    }
}
