namespace JanHafner.EAVDotNet.Query.Expression.Translation
{
    using System;
    using System.ComponentModel.Composition;
    using JanHafner.EAVDotNet.Model.Values;

    [Export(typeof(ExpressionTranslator))]
    internal sealed class GuidPropertyValueEqualityExpressionTranslator :
        PropertyValueEqualityExpressionTranslator<GuidPropertyValue, Guid>
    {
    }
}