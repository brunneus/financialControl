namespace FinanceControl.Infra
{
    public static class ValidationErrors
    {
        public static class Account
        {
            public static readonly ErrorMessage AccountDoesNotExists = new("ACCOUNT_DOES_NOT_EXISTS", "A conta informada não existe");
        }

        public static class Category
        {
            public static readonly ErrorMessage CategoryDoesNotExists = new("CATEGORY_DOES_NOT_EXISTS", "A categoria informada não existe");
        }

        public static class General
        {
            public static ErrorMessage UnknownError(string message) => new("UNKNOWN_ERROR", message);
        }
    }
}
