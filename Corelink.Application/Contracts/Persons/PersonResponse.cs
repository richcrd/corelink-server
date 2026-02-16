using Corelink.Domain.Enums;

namespace Corelink.Application.Contracts.Persons;

public sealed record PersonResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    string? Address,
    Guid LocationId,
    StatusEnum Status,
    DateTime CreatedAt);
