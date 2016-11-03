namespace JanHafner.EAVDotNet.Context
{
    using System.Data.Entity;
    using JanHafner.EAVDotNet.Model;
    using JanHafner.EAVDotNet.Model.Collections;
    using JanHafner.EAVDotNet.Model.Values;
    using JetBrains.Annotations;

    /// <summary>
    /// Should be simply your one and only needed access point to the dynamic type system.
    /// </summary>
    internal sealed class DynamicDbContext : DbContext, IDynamicDbContext
    {
        static DynamicDbContext()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<DynamicDbContext>());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDbContext"/> class.
        /// </summary>
        public DynamicDbContext([NotNull] DynamicDbContextConfiguration dynamicDbContextConfiguration)
            : base(dynamicDbContextConfiguration.NameOrConnectionString)
        {
            //this.Database.Log += s => Debug.WriteLine(s);
        }

        /// <summary>
        /// Gets the <see cref="IDbSet{InstanceDescriptor}"/> which provides access to the type system.
        /// </summary>s
        public DbSet<InstanceDescriptor> Instances { get; set; }

        /// <inheritdoc />
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InstanceDescriptor>().HasMany(id => id.Aliases).WithRequired().WillCascadeOnDelete();
            modelBuilder.Entity<InstanceDescriptor>().HasMany(id => id.PropertyValues).WithRequired(m => m.AssociatedInstance).WillCascadeOnDelete();
            modelBuilder.Entity<TypeAlias>();

            this.RegisterCollectionItemTypes(modelBuilder);
            this.RegisterPropertyValueTypes(modelBuilder);
        }

        /// <summary>
        /// Registeres collection item types.
        /// </summary>
        /// <param name="modelBuilder">The <see cref="DbModelBuilder"/>.</param>
        private void RegisterCollectionItemTypes(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CollectionItem>();

            modelBuilder.Entity<BooleanCollectionItem>();
            modelBuilder.Entity<ByteCollectionItem>();
            modelBuilder.Entity<Int16CollectionItem>();
            modelBuilder.Entity<Int32CollectionItem>();
            modelBuilder.Entity<Int64CollectionItem>();
            modelBuilder.Entity<StringCollectionItem>();
            modelBuilder.Entity<DateTimeCollectionItem>();
            modelBuilder.Entity<GuidCollectionItem>();
            modelBuilder.Entity<ByteArrayCollectionItem>();

            modelBuilder.Entity<InstanceCollectionItemAssociation>();
        }

        /// <summary>
        /// Registeres property value types.
        /// </summary>
        /// <param name="modelBuilder">The <see cref="DbModelBuilder"/>.</param>
        private void RegisterPropertyValueTypes(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PropertyValue>();
            //modelBuilder.Entity<PropertyValue>().HasKey(m => new { m.Id, m.AssociatedInstanceId });

            modelBuilder.Entity<BooleanPropertyValue>();
            modelBuilder.Entity<BytePropertyValue>();
            modelBuilder.Entity<Int16PropertyValue>();
            modelBuilder.Entity<Int32PropertyValue>();
            modelBuilder.Entity<Int64PropertyValue>();
            modelBuilder.Entity<StringPropertyValue>();
            modelBuilder.Entity<DateTimePropertyValue>();
            modelBuilder.Entity<GuidPropertyValue>();
            modelBuilder.Entity<ByteArrayPropertyValue>();

            modelBuilder.Entity<InstancePropertyAssociation>();
            modelBuilder.Entity<CollectionPropertyAssociation>();
        }
    }
}