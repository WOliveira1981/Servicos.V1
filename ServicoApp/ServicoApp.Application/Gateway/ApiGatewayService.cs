using ServicoApp.Application.Auth;
using ServicoApp.Application.Events;
using ServicoApp.Application.Services;
using ServicoApp.Domain.Entities;

namespace ServicoApp.Application.Gateway;

/// <summary>
/// Padrao Facade: o gateway centraliza chamadas para os microsservicos/fluxos da aplicacao,
/// escondendo a complexidade das integracoes internas e expondo uma API simples.
/// </summary>
public sealed class ApiGatewayService : IApiGatewayService
{
    private readonly IServicoAppService _servicoAppService;
    private readonly AuthenticationService _authenticationService;
    private readonly ILogService _logService;
    private readonly IEventPublisher _eventPublisher;

    public ApiGatewayService(
        IServicoAppService servicoAppService,
        AuthenticationService authenticationService,
        ILogService logService,
        IEventPublisher eventPublisher)
    {
        _servicoAppService = servicoAppService;
        _authenticationService = authenticationService;
        _logService = logService;
        _eventPublisher = eventPublisher;
    }

    public Task<IReadOnlyCollection<Servico>> GetAllAsync() => _servicoAppService.GetAllAsync();

    public Task<Servico?> GetByIdAsync(Guid id) => _servicoAppService.GetByIdAsync(id);

    public Task<Servico> CreateAsync(string nome, string descricao, decimal valor) =>
        _servicoAppService.CreateAsync(nome, descricao, valor);

    public async Task<AuthResult> LoginAsync(AuthenticationRequest request)
    {
        var result = await _authenticationService.AuthenticateAsync(request);
        if (result.IsAuthenticated)
        {
            await _eventPublisher.PublishAsync(new UsuarioLogadoEvent(
                Guid.NewGuid(),
                result.Email ?? request.Email ?? "unknown",
                DateTime.UtcNow));
        }

        return result;
    }

    // Padrao Adapter: o gateway converte os contratos consumidos pelo React/HTTP
    // para chamadas dos servicos internos, sem expor detalhes de dominio ao frontend.
    public Task<Orcamento> CriarOrcamentoAsync(Guid servicoId, decimal valorTotal) =>
        _servicoAppService.CriarOrcamentoAsync(servicoId, valorTotal);

    public Task<IReadOnlyCollection<Orcamento>> GetOrcamentosAsync() =>
        _servicoAppService.GetOrcamentosAsync();

    public Task<IReadOnlyCollection<AgendaItem>> GetAgendaAsync() =>
        _servicoAppService.GetAgendaAsync();

    public Task<IReadOnlyCollection<LogEntry>> GetHistoricoAsync() =>
        _logService.GetHistoricoAsync();

    public Task<Servico?> AlterarStatusAsync(Guid servicoId, bool ativo) =>
        _servicoAppService.AlterarStatusAsync(servicoId, ativo);
}
