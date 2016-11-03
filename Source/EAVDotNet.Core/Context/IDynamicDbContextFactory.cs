namespace JanHafner.EAVDotNet.Context
{
    public interface IDynamicDbContextFactory
    {
        IDynamicDbContext CreateDynamicDbContext(DynamicDbContextConfiguration dynamicDbContextConfiguration);
    }
}
