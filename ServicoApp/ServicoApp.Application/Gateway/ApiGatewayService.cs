using ServicoApp.Application.Auth;
using ServicoApp.Application.Services;
using ServicoApp.Domain.Entities;

namespace ServicoApp.Application.Gateway;

/// <summary>
/// Padrão Facade: o gateway centraliza chamadas para os microsserviços/fluxos da aplicação,
/// escondendo a complexidade das integrações internas e expondo uma API simples.
/// </summary>
public sealed class ApiGatewayService : IApiGatewayService
{
    private readonly IServicoAppService _servicoAppService;
    private readonly AuthenticationService _authenticationService;

    public ApiGatewayService(
        IServicoAppService servicoAppService,
        AuthenticationService authenticationService)
    {
        _servicoAppService = servicoAppService;
        _authenticationService = authenticationService;
    }

    public Task<IReadOnlyCollection<Servico>> GetAllAsync() => _servicoAppService.GetAllAsync();

    public Task<Servico?> GetByIdAsync(Guid id) => _servicoAppService.GetByIdAsync(id);

    public Task<Servico> CreateAsync(string nome, string descricao, decimal valor) =>
        _servicoAppService.CreateAsync(nome, descricao, valor);

    public Task<AuthResult> LoginAsync(AuthenticationRequest request) =>
        _authenticationService.AuthenticateAsync(request);
}