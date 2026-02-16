using Corelink.Application.Abstractions.Services;
using Corelink.Application.Contracts;
using Corelink.Application.Contracts.Persons;
using Corelink.Application.Interface.Persistence;
using Corelink.Application.Mapper;
using Corelink.Domain.Entities;
using Corelink.Domain.Enums;

namespace Corelink.Application.Services;

public sealed class PersonService(IPersonRepository repository) : IPersonService
{
    public async Task<Answer<PersonResponse?>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var person = await repository.GetByIdAsync(id, cancellationToken);

        return person is null ? Answer<PersonResponse?>.NotFound("Person not found") : Answer<PersonResponse?>.Ok(PersonMapper.ToResponse(person));
    }

    public async Task<Answer<PersonResponse?>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Answer<PersonResponse?>.BadRequest("Email is required");
        }

        var person = await repository.GetByEmailAsync(email.Trim(), cancellationToken);
        return person is null ? Answer<PersonResponse?>.NotFound("Person not found") : Answer<PersonResponse?>.Ok(PersonMapper.ToResponse(person));
    }

    public async Task<Answer<Guid>> CreateAsync(CreatePersonRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName))
        {
            throw new ArgumentException("FirstName is required", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.LastName))
        {
            throw new ArgumentException("LastName is required", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ArgumentException("Email is required", nameof(request));
        }

        if (request.LocationId == Guid.Empty)
        {
            throw new ArgumentException("LocationId is required", nameof(request));
        }

        var person = PersonMapper.ToEntity(request);
        
        var id =  await repository.CreateAsync(person, cancellationToken);
        
        return Answer<Guid>.Ok(id, "Person created successfully");
    }
}
