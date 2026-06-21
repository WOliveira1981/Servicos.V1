using ServicoApp.Domain.Entities;

namespace ServicoApp.Application.Services;

public interface IServicoAppService
{
    Task<IReadOnlyCollection<Servico>> GetAllAsync();
    Task<Servico?> GetByIdAsync(Guid id);
    Task<Servico> CreateAsync(string nome, string descricao, decimal valor);
}