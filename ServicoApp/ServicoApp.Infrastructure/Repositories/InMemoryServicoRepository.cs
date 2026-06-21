using ServicoApp.Domain.Entities;
using ServicoApp.Domain.Repositories;

namespace ServicoApp.Infrastructure.Repositories;

/// <summary>
/// Padrão Repository com implementação em memória (simulando um repositório de dados).
/// </summary>
public sealed class InMemoryServicoRepository : IServicoRepository
{
    private readonly Dictionary<Guid, Servico> _store = new();

    public Task<IReadOnlyCollection<Servico>> GetAllAsync()
    {
        return Task.FromResult<IReadOnlyCollection<Servico>>(_store.Values.ToList());
    }

    public Task<Servico?> GetByIdAsync(Guid id)
    {
        _store.TryGetValue(id, out var servico);
        return Task.FromResult(servico);
    }

    public Task AddAsync(Servico servico)
    {
        _store[servico.Id] = servico;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Servico servico)
    {
        _store[servico.Id] = servico;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        _store.Remove(id);
        return Task.CompletedTask;
    }
}