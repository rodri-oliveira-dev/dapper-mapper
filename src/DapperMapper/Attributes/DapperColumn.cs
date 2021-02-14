using DapperMapper.Enums;
using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace DapperMapper.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DapperColumn : Attribute
    {
        private AutoSync _sync;
        private bool _primaryKey;
        private string _columnName;

        public DapperColumn([CallerMemberName] string columnName = "", bool primaryKey = false, AutoSync sync = AutoSync.Ever)
        {
            ColumnName = columnName;
            PrimaryKey = primaryKey;
            Sync = sync;
        }

        public string ColumnName
        {
            get => _columnName;
            set => _columnName = !string.IsNullOrEmpty(value) ? Regex.Replace(value, @"[^A-Za-z0-9_[\]]+", "") : string.Empty;
        }

        public string ParameterName
        {
            get
            {
                if (!string.IsNullOrEmpty(ColumnName))
                {
                    var sanitizedValue = Regex.Replace(ColumnName, @"[^A-Za-z0-9_]+", "", RegexOptions.CultureInvariant);
                    return sanitizedValue;
                }

                return string.Empty;
            }
        }

        public AutoSync Sync
        {
            get => _sync;
            set => _sync = value;
        }

        public bool PrimaryKey
        {
            get => _primaryKey;
            set => _primaryKey = value;
        }
    }
}
