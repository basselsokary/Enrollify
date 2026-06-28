using Domain.Common.Shared;

namespace Domain.Common;

public static class UserErrors
{
    public static Error Unauthorized(string? userId) =>
        Error.Unauthorized("User.Unauthorized", $"The user with ID '{userId}' is unauthorized.");

    public static Error NotFound(string key) =>
        Error.NotFound("User.NotFound", $"User with '{key}' was not found.");

    public static Error IdRequired =>
        Error.Validation("User.IdRequired", "User ID is required.");

    public static Error FullNameRequired =>
        Error.Validation("User.FullNameRequired", "Full name is required.");

    public static Error UserNameRequired =>
        Error.Validation("User.UserNameRequired", "Username is required.");

    public static Error UserNameExceededLength =>
        Error.Validation("User.UserNameExceededLength", "Username exceeds the maximum allowed length.");

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

    public static Error CollectionRequired =>
        Error.Validation("Validation.CollectionRequired", "At least one item is required.");

    public static Error InvalidEnumValue =>
        Error.Validation("Validation.InvalidEnumValue", "Invalid value was provided.");

    public static Error NumberMustBeGreaterThanZero =>
        Error.Validation("Validation.NumberMustBeGreaterThanZero", "Value must be greater than zero.");

    public static Error MaximumLengthExceeded =>
        Error.Validation("Validation.MaximumLengthExceeded", "Value exceeds the maximum allowed length.");

    public static Error MinimumLengthNotMet =>
        Error.Validation("Validation.MinimumLengthNotMet", "Value does not meet the minimum required length.");

    public static Error RangeInvalid() =>
        Error.Validation("Validation.RangeInvalid", "The provided range is invalid.");
    
    public static Error RangeInvalid(string min, string max) =>
        Error.Validation("Validation.RangeInvalid", $"The provided range is invalid. Please provide a value between {min} and {max}.");

    public static Error WhitespaceNotAllowed =>
        Error.Validation("Validation.WhitespaceNotAllowed", "Whitespace-only values are not allowed.");
}