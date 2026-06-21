using ServicoApp.Application.Events;
using ServicoApp.Domain.Entities;
using ServicoApp.Domain.Factories;
using ServicoApp.Domain.Repositories;

namespace ServicoApp.Application.Services;

/// <summary>
/// Padrao Facade (GoF): centraliza o fluxo de uso da aplicacao e simplifica a comunicacao com o dominio.
/// </summary>
public sealed class ServicoAppService : IServicoAppService
{
    private readonly IServicoRepository _repository;
    private readonly IOrcamentoRepository _orcamentoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ServicoFactory _factory;
    private readonly IEventPublisher _eventPublisher;

    public ServicoAppService(
        IServicoRepository repository,
        IOrcamentoRepository orcamentoRepository,
        IUnitOfWork unitOfWork,
        ServicoFactory factory,
        IEventPublisher eventPublisher)
    {
        _repository = repository;
        _orcamentoRepository = orcamentoRepository;
        _unitOfWork = unitOfWork;
        _factory = factory;
        _eventPublisher = eventPublisher;
    }

    public Task<IReadOnlyCollection<Servico>> GetAllAsync() => _repository.GetAllAsync();

    public Task<Servico?> GetByIdAsync(Guid id) => _repository.GetByIdAsync(id);

    public async Task<Servico> CreateAsync(string nome, string descricao, decimal valor)
    {
        var servico = _factory.Create(nome, descricao, valor);

        // Padrao Repository: a aplicacao grava a entidade por meio da abstracao do repositorio.
        await _repository.AddAsync(servico);

        // Padrao Unit of Work: garante que a alteracao seja confirmada de forma coordenada.
        await _unitOfWork.SaveChangesAsync();

        return servico;
    }

    public async Task<Orcamento> CriarOrcamentoAsync(Guid servicoId, decimal valorTotal)
    {
        var servico = await _repository.GetByIdAsync(servicoId);
        if (servico is null)
        {
            throw new InvalidOperationException("Servico nao encontrado.");
        }

        var orcamento = new Orcamento(Guid.NewGuid(), servicoId, valorTotal, DateTime.UtcNow);
        await _orcamentoRepository.AddAsync(orcamento);
        await _unitOfWork.SaveChangesAsync();

        await _eventPublisher.PublishAsync(new OrcamentoCriadoEvent(
            orcamento.Id,
            orcamento.ServicoId,
            orcamento.ValorTotal,
            orcamento.DataCriacao));

        return orcamento;
    }

    public Task<IReadOnlyCollection<Orcamento>> GetOrcamentosAsync() => _orcamentoRepository.GetAllAsync();

    public async Task<IReadOnlyCollection<AgendaItem>> GetAgendaAsync()
    {
        var orcamentos = await _orcamentoRepository.GetAllAsync();
        return orcamentos
            .OrderByDescending(orcamento => orcamento.DataCriacao)
            .Select(orcamento => new AgendaItem(
                orcamento.Id,
                $"Orcamento {orcamento.Id.ToString()[..8]}",
                orcamento.DataCriacao,
                orcamento.Ativo ? "Ativo" : "Inativo"))
            .ToList();
    }

    public async Task<Servico?> AlterarStatusAsync(Guid servicoId, bool ativo)
    {
        var servico = await _repository.GetByIdAsync(servicoId);
        if (servico is null)
        {
            return null;
        }

        if (ativo)
        {
            servico.Ativar();
        }
        else
        {
            servico.Desativar();
        }

        await _repository.UpdateAsync(servico);
        await _unitOfWork.SaveChangesAsync();
        await _eventPublisher.PublishAsync(new ServicoStatusAlteradoEvent(servico.Id, servico.Ativo, DateTime.UtcNow));

        return servico;
    }
}
