namespace Common.Domain.Abstractions;

public abstract class AggregateRoot : BaseEntity, IAuditable
{
    public string LastModBy { get; set; } = string.Empty;
    public DateTime LastModDt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDt { get; set; }
}