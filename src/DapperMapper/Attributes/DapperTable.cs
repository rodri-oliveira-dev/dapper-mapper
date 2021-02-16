using System;
using System.Text.RegularExpressions;

namespace DapperMapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class DapperTable : Attribute
    {
        public DapperTable(string tableName)
        {
            TableName = !string.IsNullOrEmpty(tableName)
                ? Regex.Replace(tableName, @"[^A-Za-z0-9_[\].]+", "")
                : string.Empty;
        }
        public string TableName { get; }
    }
}
