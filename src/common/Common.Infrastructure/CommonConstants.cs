namespace Common.Infrastructure;

internal static class CommonConstants
{
    internal const string ParentModuleName = "Parent";
    internal const string ParentModuleSchema = "parent";
    internal const string SystemConnectionString = "Server=102.211.206.231,1433;Database=Web-System3;User Id=sa;Password=25122000SK;Encrypt=False;Trusted_Connection=True;TrustServerCertificate=True;Integrated Security=False";
    internal const string ParentConnectionString = "Host=102.211.206.231;Port=5432;Database=Customer1-HO;Username=sa;Password=25122000SK;Pooling=true;MinPoolSize=10;MaxPoolSize=100;Include Error Detail=true";
    internal const string ChildConnectionString = "Host=102.211.206.231;Port=5432;Database=Customer1-ST;Username=sa;Password=25122000SK;Pooling=true;MinPoolSize=10;MaxPoolSize=100;Include Error Detail=true";
}
