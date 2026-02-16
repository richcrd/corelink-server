using Corelink.Application.Contracts;
using Corelink.Application.Contracts.Persons;

namespace Corelink.Application.Abstractions.Services;

public interface IPersonService
{
    Task<Answer<PersonResponse?>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Answer<PersonResponse?>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Answer<Guid>> CreateAsync(CreatePersonRequest request, CancellationToken cancellationToken = default);
}
