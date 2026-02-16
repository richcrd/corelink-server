using Corelink.Domain.Enums;

namespace Corelink.Application.Contracts.Persons;

public sealed record CreatePersonRequest(
    string FirstName,
    string LastName,
    string Email,
    Guid LocationId,
    string? PhoneNumber,
    string? Address,
    StatusEnum? Status);
