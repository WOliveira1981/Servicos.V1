# Servicos.V1

Projeto referência entrevistas técnicas

✅ Autenticação e JWT estão configurados
Implementei o fluxo solicitado com:

carregamento do arquivo fake .env
middleware JWT no Program.cs
endpoint público de login em /auth/login
rotas protegidas para /servicos e /servicos/{id}
estratégia de autenticação via Google em GoogleOAuthStrategy.cs
geração do token JWT em JwtTokenService.cs
comentários explicando o padrão Strategy em IAuthenticationStrategy.cs
Verificação
dotnet build concluiu com sucesso
a chamada ao endpoint de login retornou um JWT válido via Invoke-RestMethod

✅ API Gateway criado com Swagger/OpenAPI
O projeto agora possui um gateway exposto em endpoints específicos:

GET /gateway/servicos
GET /gateway/servicos/{id}
POST /gateway/servicos
POST /gateway/auth/login
Também deixei a documentação automática disponível via Swagger/OpenAPI, e o padrão Facade foi comentado na implementação do gateway para explicar que ele centraliza chamadas e esconde a complexidade das integrações internas.

Arquivos principais ajustados
IApiGatewayService.cs
ApiGatewayService.cs
DependencyInjection.cs
Program.cs
Verificações executadas
dotnet build

✅consulta ao Swagger (/swagger/v1/swagger.json) mostrando os endpoints do gateway ✅
O endpoint de documentação está disponível no runtime em http://localhost:5003/swagger.

✅ Testes xUnit adicionados e validados
Implementei testes unitários e de integração cobrindo:

autenticação
persistência com SQLite
eventos e observer/dispatcher
integração da API com Swagger e gateway
Arquivos criados
AuthTests.cs
PersistenceTests.cs
EventTests.cs
IntegrationTests.cs
ServicoApp.Tests.csproj
Ajuste importante
Também corrigi o mapeamento da entidade Servico.cs, porque o Dapper precisava de um construtor compatível com os tipos SQLite.

Comentários sobre Mock/Test Double
Incluí observações nos testes explicando que:

FakeServiceBus substitui o broker real para verificar publicação de eventos
WebApplicationFactory simula a API real para testes HTTP
logs fake são usados apenas para observar comportamento sem depender de infraestrutura externa
Verificação executada
comando: dotnet test ServicoApp.Tests/ServicoApp.Tests.csproj
resultado: 7 testes bem-sucedidos, 0 falhas

✅ Testes xUnit adicionados e validados
Implementei testes unitários e de integração cobrindo:

autenticação
persistência com SQLite
eventos e observer/dispatcher
integração da API com Swagger e gateway
Arquivos criados
AuthTests.cs
PersistenceTests.cs
EventTests.cs
IntegrationTests.cs
ServicoApp.Tests.csproj
Ajuste importante
Também corrigi o mapeamento da entidade Servico.cs, porque o Dapper precisava de um construtor compatível com os tipos SQLite.

Comentários sobre Mock/Test Double
Incluí observações nos testes explicando que:

FakeServiceBus substitui o broker real para verificar publicação de eventos
WebApplicationFactory simula a API real para testes HTTP
logs fake são usados apenas para observar comportamento sem depender de infraestrutura externa
Verificação executada
comando: dotnet test ServicoApp.Tests/ServicoApp.Tests.csproj
resultado: 7 testes bem-sucedidos, 0 falhas

✅ Principais pontos:
Docker Compose com imagens separadas para api, frontend, sqlite e servicebus mockado.
Dockerfiles com comentários explicando o padrão Builder.
Frontend React agora usa Webpack e chama o backend por um adapter em frontend/src/apiGatewayAdapter.ts, com comentário do padrão Adapter.
API Gateway expõe login, serviços, orçamento, agenda, histórico e alteração de status.
Logs persistidos em tabela Logs, com observer PersistentLogEventObserver, comentando Observer e Event Sourcing.
Eventos registrados para login, gravação de orçamento e mudança de status.
Ajustei o mapeamento SQLite/Dapper para GUIDs e valores numéricos.
Validação feita:
dotnet test ServicoApp\ServicoApp.Tests\ServicoApp.Tests.csproj: 7/7 testes passaram.
npm run build: Webpack compilou com sucesso.
docker compose --progress plain build: imagens construídas com sucesso.
docker compose up -d: containers estão rodando.
Smoke test via API Gateway passou: login, criação de serviço, gravação de orçamento, agenda e histórico.
URLs ativas:
Frontend: http://localhost:3000
API: http://localhost:5000
Service Bus mockado: http://localhost:5100
Observação: o npm install reportou 4 vulnerabilidades moderadas em dependências transitivas. Não rodei npm audit fix --force porque isso pode introduzir breaking changes.
