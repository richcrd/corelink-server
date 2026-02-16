using Corelink.Domain.Entities;

namespace Corelink.Application.Interface.Persistence;

public interface IPersonRepository
{
    Task<Person?> GetByIdAsync(Guid id);
    Task<Person?> GetByEmailAsync(string email);
    Task<Guid> CreateAsync(Person person);
}
