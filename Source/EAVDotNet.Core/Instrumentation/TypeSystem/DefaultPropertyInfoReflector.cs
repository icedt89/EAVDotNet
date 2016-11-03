namespace JanHafner.EAVDotNet.Instrumentation.TypeSystem
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Reflection;
    using JanHafner.Toolkit.Common.Reflection;
    using Toolkit.Common.ExtensionMethods;

    /// <summary>
    /// Default implementation of the <see cref="IPropertyReflector"/>.
    /// Reflects <see cref="PropertyInfo"/> instances which passes the following predicate:
    /// (Instance | Public | NonPublic) && Readable && Writable && NoIndexParameter && NoNotMappedAttribute
    /// </summary>
    [Export(typeof(IPropertyReflector))]
    internal sealed class DefaultPropertyInfoReflector : PropertyReflector
    {
        protected override IEnumerable<PropertyInfo> ReflectPropertiesCore(Type type)
        {
            return
              base.ReflectPropertiesCore(type)
                  .Where(
                      pi =>
                          pi.CanWrite && pi.CanRead && pi.GetIndexParameters().Length == 0 &&
                          !pi.HasAttribute<NotMappedAttribute>()).ToList();
        }
    }
}