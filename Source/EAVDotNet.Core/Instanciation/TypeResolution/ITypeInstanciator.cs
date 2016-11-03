namespace JanHafner.EAVDotNet.Instanciation.TypeResolution
{
    using System;
    using JetBrains.Annotations;

    /// <summary>
    /// The implementor is responsible for creating instances of <see cref="Type"/> objects.
    /// </summary>
    public interface ITypeInstanciator
    {
        /// <summary>
        /// Creates an instance of the supplied <see cref="Type"/>.
        /// </summary>
        /// <param name="typeInstanciationContext">The <see cref="ITypeInstanciationContext"/> from which to create an instance.</param>
        /// <returns>The created instance.</returns>
        [NotNull]
        Object CreateInstance([NotNull] ITypeInstanciationContext typeInstanciationContext);
    }
}
