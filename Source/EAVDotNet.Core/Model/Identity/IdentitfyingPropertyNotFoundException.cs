namespace JanHafner.EAVDotNet.Model.Identity
{
    using System;
    using System.Runtime.Serialization;
    using JanHafner.EAVDotNet.Properties;
    using JetBrains.Annotations;

    /// <summary>
    /// This <see cref="Exception"/> is thrown if the supplied <see cref="Type"/> has not specified a [Key] property.
    /// </summary>
    public sealed class IdentitfyingPropertyNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentitfyingPropertyNotFoundException"/> class.
        /// </summary>
        /// <param name="type">The <see cref="Type"/>.</param>
        public IdentitfyingPropertyNotFoundException([NotNull] Type type)
            : base(String.Format(ExceptionMessages.IdentitfyingPropertyNotFoundExceptionMessage, type.Name))
        {
        }

        private IdentitfyingPropertyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}