using System.ComponentModel.DataAnnotations;
using Portfolio.Api.Models;

namespace Portfolio.Api.Services;

public static class ContactRequestValidator
{
    private static readonly EmailAddressAttribute EmailValidator = new();

    public static Dictionary<string, string[]> Validate(ContactRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Trim().Length is < 2 or > 80)
        {
            errors[nameof(request.Name)] = ["Name must be between 2 and 80 characters."];
        }

        if (string.IsNullOrWhiteSpace(request.Email) ||
            request.Email.Length > 254 ||
            !EmailValidator.IsValid(request.Email))
        {
            errors[nameof(request.Email)] = ["Enter a valid email address."];
        }

        if (string.IsNullOrWhiteSpace(request.Message) || request.Message.Trim().Length is < 10 or > 2_000)
        {
            errors[nameof(request.Message)] = ["Message must be between 10 and 2,000 characters."];
        }

        return errors;
    }
}
