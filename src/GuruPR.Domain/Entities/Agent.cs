using GuruPR.Domain.Entities.Configurations;
using GuruPR.Domain.Entities.Configurations.Enums;
using GuruPR.Domain.Errors;

namespace GuruPR.Domain.Entities;

public class Agent
{
    // Basic Info
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = null!;
    public string AvatarUrl { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Instrunctions { get; set; } = null!;

    // Tools
    public IList<string> Tools { get; set; } = null!;

    // Model Configuration
    public ModelConfiguration ModelConfiguration { get; set; } = null!;

    // Memory Settings
    public MemoryConfiguration MemoryConfiguration { get; set; } = null!;

    // Metadata
    public string CreatedByUserId { get; set; } = null!;
    public AgentStatus Status { get; set; } = AgentStatus.Inactive;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsValid() => !Validate().Any();

    // Validation Method
    public IEnumerable<ValidationError> Validate()
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(Id) || !Guid.TryParse(Id, out _))
        {
            errors.Add(new ValidationError
            {
                Field = nameof(Id),
                Message = "Invalid or missing Id."
            });
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            errors.Add(new ValidationError
            {
                Field = nameof(Name),
                Message = "Name is required."
            });
        }

        if (string.IsNullOrWhiteSpace(AvatarUrl) || !Uri.IsWellFormedUriString(AvatarUrl, UriKind.Absolute))
        {
            errors.Add(new ValidationError
            {
                Field = nameof(AvatarUrl),
                Message = "AvatarUrl must be a valid absolute URL."
            });
        }

        if (string.IsNullOrWhiteSpace(Description) || Description.Length < 5)
        {
            errors.Add(new ValidationError
            {
                Field = nameof(Description),
                Message = "Description is required and must be at least 5 characters."
            });
        }

        if (string.IsNullOrWhiteSpace(Instrunctions))
        {
            errors.Add(new ValidationError
            {
                Field = nameof(Instrunctions),
                Message = "Instructions are required."
            });
        }

        if (Tools == null || Tools.Count == 0)
        {
            errors.Add(new ValidationError
            {
                Field = nameof(Tools),
                Message = "At least one tool is required."
            });
        }

        if (ModelConfiguration == null)
        {
            errors.Add(new ValidationError
            {
                Field = nameof(ModelConfiguration),
                Message = "Model configuration is required."
            });
        }
        else
        {
            errors.AddRange(ModelConfiguration.Validate());
        }

        // Memory configuration is optional
        if (MemoryConfiguration != null)
        {
            errors.AddRange(MemoryConfiguration.Validate());
        }

        if (string.IsNullOrWhiteSpace(CreatedByUserId))
        {
            errors.Add(new ValidationError
            {
                Field = nameof(CreatedByUserId),
                Message = "CreatedBy user ID is required."
            });
        }

        return errors;
    }
}
