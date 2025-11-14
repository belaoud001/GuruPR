using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Domain.Entities.Tool;
using GuruPR.Persistence.Contexts;

namespace GuruPR.Persistence.Repositories;

public class ToolRepository : GenericRepository<Tool>, IToolRepository
{
    public ToolRepository(GuruDbContext dbContext) : base(dbContext)
    {
    }
}
