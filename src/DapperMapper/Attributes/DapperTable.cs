using System;

namespace DapperMapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class DapperTable : Attribute
    {
        public DapperTable(string tableName)
        {
            TableName = tableName;
        }
        public string TableName { get; }
    }
}
