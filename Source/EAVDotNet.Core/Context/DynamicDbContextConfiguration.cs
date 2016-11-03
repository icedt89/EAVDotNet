namespace JanHafner.EAVDotNet.Context
{
    using System;
    using JetBrains.Annotations;

    /// <summary>
    /// Contains configuration for the <see cref="IDynamicDbContext"/> and <see cref="DynamicDbContextAdapter" />.
    /// </summary>
    public sealed class DynamicDbContextConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDbContextConfiguration"/> class.
        /// </summary>
        /// <param name="nameOrConnectionString">The connection string or the name of the connection string inside in the configuration file which will be used.</param>
        public DynamicDbContextConfiguration([NotNull] String nameOrConnectionString)
        {
            if (String.IsNullOrEmpty(nameOrConnectionString))
            {
                throw new ArgumentNullException(nameof(nameOrConnectionString));
            }

            this.NameOrConnectionString = nameOrConnectionString;
        }

        /// <summary>
        /// Gets the connection string or the name of the connection string which will be used.
        /// </summary>
        [NotNull]
        public String NameOrConnectionString { get; private set; }
    }
}
