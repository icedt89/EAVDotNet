namespace JanHafner.EAVDotNet.Context
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using JanHafner.EAVDotNet.Model;
    using JetBrains.Annotations;

    /// <summary>
    /// Defines the interface for the <see cref="DynamicDbContext"/> without relying on the <see cref="DbContext"/>.
    /// </summary>
    public interface IDynamicDbContext : IDisposable
    {
        /// <summary>
        /// Gets the <see cref="DbSet{InstanceDescriptor}"/> which provides access to the instance system.
        /// </summary>
        [NotNull]
        DbSet<InstanceDescriptor> Instances { get; }

        /// <summary>
        /// Saves the changes and returns the count of affected rows, asynchronously.
        /// </summary>
        /// <returns>The count of affected rows.</returns>
        Task<Int32> SaveChangesAsync();
    }
}