namespace JanHafner.EAVDotNet.Query
{
    using System;
    using JanHafner.EAVDotNet.Properties;

    /// <summary>
    /// This <see cref="Exception"/> is thrown if a query based on IQueryable{T} instead of IQueryable{InstanceDescriptor} is executed and resulted in an <see cref="Exception"/>.
    /// </summary>
    public sealed class INeedYourHelpToImproveException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INeedYourHelpToImproveException"/> class.
        /// </summary>
        /// <param name="innerException">The underlying <see cref="Exception"/>.</param>
        public INeedYourHelpToImproveException(Exception innerException)
            : base(ExceptionMessages.INeedYourHelpToImproveExceptionMessage, innerException)
        {
        }
    }
}
