using Corelink.Application.Contracts;
using Corelink.Application.Contracts.Persons;

namespace Corelink.Application.Abstractions.Services;

public interface IPersonService
{
    Task<Answer<PersonResponse?>> GetByIdAsync(Guid id);
    Task<Answer<PersonResponse?>> GetByEmailAsync(string email);
    Task<Answer<Guid>> CreateAsync(CreatePersonRequest request);
}
