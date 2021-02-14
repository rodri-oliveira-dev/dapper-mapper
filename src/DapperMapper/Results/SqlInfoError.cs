namespace DapperMapper.Results
{
    public sealed class SqlInfoError
    {
        public SqlInfoError(int errorNumber, int errorSeverity, int errorState, string errorProcedure, int errorLine, string errorMessage)
        {
            ErrorNumber = errorNumber;
            ErrorSeverity = errorSeverity;
            ErrorState = errorState;
            ErrorProcedure = errorProcedure;
            ErrorLine = errorLine;
            ErrorMessage = errorMessage;
        }

        public int ErrorNumber { get; }

        public int ErrorSeverity { get; }

        public int ErrorState { get; }

        public string ErrorProcedure { get; }

        public int ErrorLine { get; }

        public string ErrorMessage { get; }

    }
}
