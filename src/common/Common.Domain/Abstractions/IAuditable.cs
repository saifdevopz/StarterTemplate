namespace Common.Domain.Abstractions;

public interface IAuditable
{
    public DateTime CreatedDt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime LastModDt { get; set; }
    public string LastModBy { get; set; }
}
