namespace JanHafner.EAVDotNet.Context
{
    using System;
    using System.ComponentModel.Composition;
    using JetBrains.Annotations;

    [Export(typeof(IDynamicDbContextFactory))]
    internal sealed class DynamicDbContextFactory : IDynamicDbContextFactory
    {
        public IDynamicDbContext CreateDynamicDbContext(
            [NotNull] DynamicDbContextConfiguration dynamicDbContextConfiguration)
        {
            if (dynamicDbContextConfiguration == null)
            {
                throw new ArgumentNullException(nameof(dynamicDbContextConfiguration));
            }

            return new DynamicDbContext(dynamicDbContextConfiguration);
        }
    }
}