using Domain.Common;
using Domain.Common.Shared;

namespace Domain.ValueObjects;

public class Address : ValueObject
{
    public const int MaxAddressLength = 512;
    public const int MaxStreetLength = 256;
    public const int MaxCityLength = 128;
    public const int MaxRegionLength = 128;
    public const int MaxPostalCodeLength = 16;

    public string Value { get; } = null!;
    
    private Address() { }
    private Address(string value)
    {
        Value = value;
    }

    public static Result<Address> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return AddressErrors.EmptyAddress;

        return new Address(value.Trim());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(Address? address) => address?.Value ?? string.Empty;
}

public static class AddressErrors
{
    public static Error EmptyAddress =>
        Error.Validation("ADDRESS_REQUIRED", "Address is required.");

    public static Error ExceededAddressLength =>
        Error.Validation("ADDRESS_LENGTH_EXCEEDED", "Address length cannot exceed the defined maximum.");

    public static Error ExceededStreetLength =>
        Error.Validation("ADDRESS_STREET_LENGTH_EXCEEDED", "Street length cannot exceed the defined maximum.");

    public static Error ExceededCityLength =>
        Error.Validation("ADDRESS_CITY_LENGTH_EXCEEDED", "City length cannot exceed the defined maximum.");

    public static Error ExceededRegionLength =>
        Error.Validation("ADDRESS_REGION_LENGTH_EXCEEDED", "Region length cannot exceed the defined maximum.");
    
    public static Error ExceededPostalCodeLength =>
        Error.Validation("ADDRESS_POSTAL_CODE_LENGTH_EXCEEDED", "Postal code length cannot exceed the defined maximum.");

    public static Error EmptyStreet =>
        Error.Validation("ADDRESS_STREET_REQUIRED", "Street is required.");

    public static Error EmptyCity =>
        Error.Validation("ADDRESS_CITY_REQUIRED", "City is required.");
}
