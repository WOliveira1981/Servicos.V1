using ServicoApp.Domain.Entities;
using ServicoApp.Domain.Factories;
using ServicoApp.Domain.Repositories;

namespace ServicoApp.Application.Services;

/// <summary>
/// Padrão Facade (GoF): centraliza o fluxo de uso da aplicação e simplifica a comunicação com o domínio.
/// </summary>
public sealed class ServicoAppService : IServicoAppService
{
    private readonly IServicoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ServicoFactory _factory;

    public ServicoAppService(IServicoRepository repository, IUnitOfWork unitOfWork, ServicoFactory factory)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _factory = factory;
    }

    public Task<IReadOnlyCollection<Servico>> GetAllAsync() => _repository.GetAllAsync();

    public Task<Servico?> GetByIdAsync(Guid id) => _repository.GetByIdAsync(id);

    public async Task<Servico> CreateAsync(string nome, string descricao, decimal valor)
    {
        var servico = _factory.Create(nome, descricao, valor);

        // Padrão Repository: a aplicação grava a entidade por meio da abstração do repositório.
        await _repository.AddAsync(servico);

        // Padrão Unit of Work: garante que a alteração seja confirmada de forma coordenada.
        await _unitOfWork.SaveChangesAsync();

        return servico;
    }
}