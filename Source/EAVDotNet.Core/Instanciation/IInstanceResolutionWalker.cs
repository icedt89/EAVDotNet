namespace JanHafner.EAVDotNet.Instanciation
{
    using System;
    using JetBrains.Annotations;

    /// <summary>
    /// The implementation is responsible for creating an instance from the <see cref="InstanceResolutionContext"/>.
    /// </summary>
    public interface IInstanceResolutionWalker
    {
        /// <summary>
        /// Creates an object from the supplied <see cref="InstanceResolutionContext"/>.
        /// </summary>
        /// <param name="instanceResolutionContext">The <see cref="InstanceResolutionContext"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="instanceResolutionContext"/>' cannot be null.</exception>
        /// <returns>The created object.</returns>
        Object ResolveInstance([NotNull] InstanceResolutionContext instanceResolutionContext);
    }
}