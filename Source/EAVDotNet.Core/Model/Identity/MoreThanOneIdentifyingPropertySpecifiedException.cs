namespace JanHafner.EAVDotNet.Model.Identity
{
    using System;
    using System.Runtime.Serialization;
    using JanHafner.EAVDotNet.Properties;
    using JetBrains.Annotations;

    /// <summary>
    /// This <see cref="Exception"/> is thrown if there are more than one property specified as [Key].
    /// </summary>
    public sealed class MoreThanOneIdentifyingPropertySpecifiedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentitfyingPropertyNotFoundException"/> class.
        /// </summary>
        /// <param name="type">The <see cref="Type"/>.</param>
        public MoreThanOneIdentifyingPropertySpecifiedException([NotNull] Type type)
            : base(String.Format(ExceptionMessages.MoreThanOneIdentifyingPropertySpecifiedExceptionMessage, type.Name))
        {
        }

        private MoreThanOneIdentifyingPropertySpecifiedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}