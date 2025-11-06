namespace GuruPR.Domain.Errors;

public class ValidationError
{
    public string Field { get; set; } = null!;

    public string Message { get; set; } = null!;
}
