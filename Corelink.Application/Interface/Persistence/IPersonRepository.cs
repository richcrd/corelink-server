using Corelink.Domain.Entities;

namespace Corelink.Application.Interface.Persistence;

public interface IPersonRepository
{
    Task<Person?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Person?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(Person person, CancellationToken cancellationToken = default);
}
