namespace DapperMapper.Results
{
    public sealed class QueryResult
    {
        public QueryResult(bool success, long affectedRows, SqlInfoError error = null)
        {
            Success = success;
            AffectedRows = affectedRows;
            Error = error;
        }

        public bool Success { get; }

        public long AffectedRows { get; }

        public SqlInfoError Error { get; }
    }
}
