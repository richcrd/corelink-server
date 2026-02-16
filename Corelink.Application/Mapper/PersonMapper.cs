using Corelink.Application.Contracts.Persons;
using Corelink.Domain.Entities;
using Corelink.Domain.Enums;

namespace Corelink.Application.Mapper;

public static class PersonMapper
{
    public static PersonResponse ToResponse(Person person) =>
        new PersonResponse(
            person.Id,
            person.FirstName,
            person.LastName,
            person.Email,
            person.PhoneNumber,
            person.Address,
            person.LocationId,
            person.Status,
            person.CreatedAt
        );
    
    public static Person ToEntity(CreatePersonRequest request)
    {
        return new Person
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim(),
            PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber)
                ? null
                : request.PhoneNumber.Trim(),
            Address = string.IsNullOrWhiteSpace(request.Address)
                ? null
                : request.Address.Trim(),
            LocationId = request.LocationId,
            Status = request.Status ?? StatusEnum.Active
        };
    }
}