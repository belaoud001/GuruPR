namespace GuruPR.Domain.Interfaces.Markers;

public interface IOwnedEntity : IHasEntityId
{
    string UserId { get; set; }
}
