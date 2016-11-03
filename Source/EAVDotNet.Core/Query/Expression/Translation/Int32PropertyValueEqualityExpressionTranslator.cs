namespace JanHafner.EAVDotNet.Query.Expression.Translation
{
    using System;
    using System.ComponentModel.Composition;
    using JanHafner.EAVDotNet.Model.Values;

    [Export(typeof(ExpressionTranslator))]
    internal sealed class Int32PropertyValueEqualityExpressionTranslator :
        PropertyValueEqualityExpressionTranslator<Int32PropertyValue, Int32>
    {
    }
}