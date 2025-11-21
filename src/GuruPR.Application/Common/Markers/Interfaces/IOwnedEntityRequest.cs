using GuruPR.Domain.Interfaces.Markers;

namespace GuruPR.Application.Common.Markers.Interfaces;

public interface IOwnedEntityRequest<TEntity> : IUserContextCommand where TEntity : IOwnedEntity
{
    string Id { get; }
}
