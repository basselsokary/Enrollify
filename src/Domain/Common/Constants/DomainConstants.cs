namespace Domain.Common.Constants;

public static class DomainConstants
{
    public static class Paging
    {
        public const int DefaultPageSize = 20;
        public const int MaxPageSize = 100;
    }

    public static class User
    {
        public const int MaxUserNameLength = 128;
        public const int MaxFirstNameLength = 128;
        public const int MaxLastNameLength = 128;
        public const int MaxEmailLength = 256;
        public const int MaxFullNameLength = 512;
        public const int MaxPasswordLength = 64;
        public const int MinPasswordLength = 8;
        public const int MaxPasswordHashLength = 512;
        public const int MaxRefreshTokenLength = 512;
    }
}