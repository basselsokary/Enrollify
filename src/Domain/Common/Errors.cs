using Domain.Common.Shared;

namespace Domain.Common;

public static class UserErrors
{
    public static Error NotFound(string key) =>
        Error.NotFound("User.NotFound", $"User with '{key}' was not found.");
    public static Error FullNameRequired =>
        Error.Validation("User.FullNameRequired", "Full name is required.");
    public static Error FullNameExceededLength =>
        Error.Validation("User.FullNameExceededLength", "Full name exceeds the maximum allowed length.");
    
    public static Error EmailRequired =>
        Error.Validation("Email.Required", "Email is required.");
    public static Error InvalidEmailFormat(string email) =>
        Error.Validation("Email.InvalidFormat", $"The provided email '{email}' is not in a valid format.");
    public static Error EmailTooLong =>
        Error.Validation("Email.TooLong", "Email exceeds the maximum allowed length.");
    
    public static Error PasswordRequired =>
        Error.Validation("User.PasswordRequired", "Password is required.");
    public static Error PasswordTooShort =>
        Error.Validation("User.PasswordTooShort", "Password does not meet the minimum required length.");
    public static Error PasswordTooLong =>
        Error.Validation("User.PasswordTooLong", "Password exceeds the maximum allowed length.");
}

public static class ValidationErrors
{
    public static Error ValueRequired =>
        Error.Validation("Validation.ValueRequired", "Value is required.");

    public static Error InvalidEnumValue =>
        Error.Validation("Validation.InvalidEnumValue", "Invalid value was provided.");

    public static Error NumberMustBeGreaterThanZero =>
        Error.Validation("Validation.NumberMustBeGreaterThanZero", "Value must be greater than zero.");

    public static Error MaximumLengthExceeded =>
        Error.Validation("Validation.MaximumLengthExceeded", "Value exceeds the maximum allowed length.");

    public static Error RangeInvalid(int min, int max) =>
        Error.Validation("Validation.RangeInvalid", $"The provided range is invalid. Please provide a value between {min} and {max}.");
}