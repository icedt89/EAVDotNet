namespace JanHafner.EAVDotNet
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using JanHafner.Toolkit.Common.Reflection;
    using JetBrains.Annotations;

    /// <summary>
    /// Provides basic access to some properties of a <see cref="Type"/> by walking over them and calling methods which can be overridden in a derived class.
    /// </summary>
    internal abstract class TypeWalker
    {
        [NotNull]
        private readonly IPropertyReflector propertyReflector;

        /// <summary>
        /// Access to the <see cref="Type"/> with which this instance was initialized.
        /// This field must be set in a derived class in order to function correctly.
        /// </summary>
        [CanBeNull]
        protected Type InitializedWithType;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeWalker"/> class.
        /// </summary>
        /// <param name="propertyReflector">The <see cref="IPropertyReflector"/> for property retrieval.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyReflector"/>' cannot be null.</exception>
        protected TypeWalker([NotNull] IPropertyReflector propertyReflector)
        {
            if (propertyReflector == null)
            {
                throw new ArgumentNullException(nameof(propertyReflector));
            }

            this.propertyReflector = propertyReflector;
            this.ReflectProperties = true;
        }

        /// <summary>
        /// Gets a value indicating whether the walker should walk on interfaces.
        /// </summary>
        protected Boolean ReflectInterfaces { get; set; }

        /// <summary>
        /// Gets a value indicating whether the walker should walk on properties.
        /// </summary>
        protected Boolean ReflectProperties { get; set; }

        /// <summary>
        /// Gets a value indicating whether the walker should walk on base types.
        /// </summary>
        protected Boolean ReflectBaseTypes { get; set; }

        /// <summary>
        /// Reflects the propertis of the <see cref="Type"/> described by the <see cref="InitializedWithType"/> field.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<PropertyInfo> ReflectTypeProperties()
        {
            return this.propertyReflector.ReflectProperties(this.InitializedWithType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<Type> ReflectTypeInterfaces()
        {
            return this.InitializedWithType.GetInterfaces();
        }

        /// <summary>
        /// Begins the walk process.
        /// Because the walk process changes the state of the current instance it id best practice to reset the fields after your work is done.
        /// </summary>
        /// <exception cref="InvalidOperationException">The <see cref="InitializedWithType"/> is not correctly initialized.</exception>
        protected virtual void Walk()
        {
            if (this.InitializedWithType == null)
            {
                throw new InvalidOperationException();
            }

            this.WalkInterfaces();
            this.WalkProperties();
            this.WalkBaseTypes();
        }

        /// <summary>
        /// Begins walking on all interfaces.
        /// </summary>
        private void WalkInterfaces()
        {
            if (!this.ReflectInterfaces)
            {
                return;
            }

            foreach (var interfaceType in this.ReflectTypeInterfaces())
            {
                this.StepOnInterface(interfaceType);
            }
        }

        /// <summary>
        /// Begins walking on all properties.
        /// </summary>
        private void WalkProperties()
        {
            if (!this.ReflectProperties)
            {
                return;
            }

            foreach (var propertyInfo in this.ReflectTypeProperties())
            {
                this.StepOnPropertyInfo(propertyInfo);
            }
        }

        /// <summary>
        /// Begins walking on all base types.
        /// </summary>
        private void WalkBaseTypes()
        {
            if (!this.ReflectBaseTypes)
            {
                return;
            }

            var currentBaseType = this.InitializedWithType.BaseType;
            while (currentBaseType != null)
            {
                this.StepOnBaseType(currentBaseType);

                currentBaseType = currentBaseType.BaseType;
            }
        }

        /// <summary>
        /// The <see cref="TypeWalker"/> is stepped on a base type.
        /// </summary>
        /// <param name="baseType">The <see cref="Type"/> of the base type.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="baseType"/>' cannot be null.</exception>
        protected virtual void StepOnBaseType([NotNull] Type baseType)
        {
        }

        /// <summary>
        /// The <see cref="TypeWalker"/> is stepped on an interface implementation.
        /// </summary>
        /// <param name="interfaceType">The <see cref="Type"/> of the interface.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="interfaceType"/>' cannot be null.</exception>
        protected virtual void StepOnInterface([NotNull] Type interfaceType)
        {
        }

        /// <summary>
        /// The <see cref="TypeWalker"/> is stepped on a property.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> of the property.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyInfo"/>' cannot be null.</exception>
        protected virtual void StepOnPropertyInfo([NotNull] PropertyInfo propertyInfo)
        {
        }
    }
}
