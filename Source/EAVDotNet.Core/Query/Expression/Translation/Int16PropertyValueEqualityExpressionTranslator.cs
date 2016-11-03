namespace JanHafner.EAVDotNet.Query.Expression.Translation
{
    using System;
    using System.ComponentModel.Composition;
    using JanHafner.EAVDotNet.Model.Values;

    [Export(typeof(ExpressionTranslator))]
    internal sealed class Int16PropertyValueEqualityExpressionTranslator :
        PropertyValueEqualityExpressionTranslator<Int16PropertyValue, Int16>
    {
    }
}