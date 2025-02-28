namespace Store.Domain;
public class StoreM
{
    public int StoreId { get; }
    public string StoreName { get; private set; } = string.Empty;
}
