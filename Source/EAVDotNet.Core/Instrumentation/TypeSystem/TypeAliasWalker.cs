namespace JanHafner.EAVDotNet.Instrumentation.TypeSystem
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using JanHafner.EAVDotNet;
    using JanHafner.EAVDotNet.Model;
    using JanHafner.Toolkit.Common.ExtensionMethods;
    using JanHafner.Toolkit.Common.Reflection;

    /// <summary>
    /// Walks over a <see cref="Type"/> and creates <see cref="TypeAlias"/> instances.
    /// Instanciation via MEF is transient.
    /// </summary>
    [Export(typeof (ITypeAliasWalker))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal sealed class TypeAliasWalker : TypeWalker, ITypeAliasWalker
    {
        private InstanceDescriptor instanceDescriptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeAliasWalker"/> class.
        /// </summary>
        /// <param name="propertyReflector">The <see cref="IPropertyReflector"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyReflector"/>' cannot be null.</exception>
        [ImportingConstructor]
        public TypeAliasWalker(IPropertyReflector propertyReflector)
            : base(propertyReflector)
        {
            this.ReflectBaseTypes = true;
            this.ReflectInterfaces = true;
            this.ReflectProperties = false;
        }

        /// <summary>
        /// Creates <see cref="TypeAlias"/> instances for the supplied <see cref="InstanceDescriptor"/>.
        /// </summary>
        /// <param name="instanceDescriptor">The <see cref="InstanceDescriptor"/>.</param>
        /// <param name="type">The <see cref="Type"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="instanceDescriptor"/>' and '<paramref name="type"/>' cannot be null.</exception>
        public void CreateAliases(InstanceDescriptor instanceDescriptor, Type type)
        {
            if (instanceDescriptor == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptor));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            this.instanceDescriptor = instanceDescriptor;
            this.InitializedWithType = type;

            this.Walk();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<Type> ReflectTypeInterfaces()
        {
            return base.ReflectTypeInterfaces().Where(it => !it.HasAttribute<NoTypeDescriptorAliasAttribute>());
        }

        /// <summary>
        /// The <see cref="TypeWalker"/> is stepped on an interface implementation.
        /// </summary>
        /// <param name="interfaceType">The <see cref="Type"/> of the interface.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="interfaceType"/>' cannot be null.</exception>
        protected override void StepOnInterface(Type interfaceType)
        {
            if (interfaceType == null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }

            this.instanceDescriptor.Aliases.Add(new TypeAlias(interfaceType));
        }

        /// <summary>
        /// The <see cref="TypeWalker"/> is stepped on a base type.
        /// </summary>
        /// <param name="baseType">The <see cref="Type"/> of the base type.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="baseType"/>' cannot be null.</exception>
        protected override void StepOnBaseType(Type baseType)
        {
            if (baseType == null)
            {
                throw new ArgumentNullException(nameof(baseType));
            }

            if (baseType.HasAttribute<NoTypeDescriptorAliasAttribute>())
            {
                return;
            }

            this.instanceDescriptor.Aliases.Add(new TypeAlias(baseType));
        }
    }

}