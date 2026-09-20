using Portfolio.Api.Models;
using Portfolio.Api.Services;

namespace Portfolio.Api.Tests;

public sealed class ContactRequestValidatorTests
{
    [Fact]
    public void Validate_WithValidRequest_ReturnsNoErrors()
    {
        var request = new ContactRequest(
            "Ada Lovelace",
            "ada@example.com",
            "I would like to discuss a backend engineering role.");

        var errors = ContactRequestValidator.Validate(request);

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_WithInvalidFields_ReturnsErrorsForEveryField()
    {
        var request = new ContactRequest("A", "not-an-email", "short");

        var errors = ContactRequestValidator.Validate(request);

        Assert.Equal(3, errors.Count);
        Assert.Contains(nameof(ContactRequest.Name), errors.Keys);
        Assert.Contains(nameof(ContactRequest.Email), errors.Keys);
        Assert.Contains(nameof(ContactRequest.Message), errors.Keys);
    }
}
