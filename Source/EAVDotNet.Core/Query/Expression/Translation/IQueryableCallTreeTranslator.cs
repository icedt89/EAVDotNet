namespace JanHafner.EAVDotNet.Query.Expression.Translation
{
    using System.Linq;
    using System.Linq.Expressions;
    using JanHafner.EAVDotNet.Model;
    using JetBrains.Annotations;

    public interface IQueryableCallTreeTranslator
    {
        Expression Translate([NotNull] IQueryable<InstanceDescriptor> instances, Expression expression);
    }
}