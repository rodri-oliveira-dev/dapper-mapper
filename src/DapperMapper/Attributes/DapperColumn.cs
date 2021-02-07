using DapperMapper.Enums;
using System;
using System.Runtime.CompilerServices;

namespace DapperMapper.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DapperColumn : Attribute
    {
        public DapperColumn([CallerMemberName] string columnName = "", bool primaryKey = false)
        {
            ColumnName = columnName;
            PrimaryKey = primaryKey;
        }

        public string ColumnName { get; set; }

        public AutoSync Sync { get; set; }
        public bool PrimaryKey { get; set; }
    }
}
