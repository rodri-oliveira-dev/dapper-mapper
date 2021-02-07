using DapperMapper.Attributes;
using System;
using System.Reflection;

namespace DapperMapper.Mapper
{
    internal sealed class MappedEntityProperty<T>
    {
        public MappedEntityProperty(PropertyInfo property, DapperColumn dapperColumn)
        {
            Property = property;
            DapperColumn = dapperColumn;
            Setter = FasterInvoker.BuildUntypedSetter<T>(property);
            Getter = FasterInvoker.BuildUntypedGetter<T>(property);
        }

        public PropertyInfo Property { get; }
        public DapperColumn DapperColumn { get; }
        public Action<T, object> Setter { get; }
        public Func<T, object> Getter { get; }
    }
}
