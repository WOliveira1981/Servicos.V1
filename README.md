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
dotnet build ✅
consulta ao Swagger (/swagger/v1/swagger.json) mostrando os endpoints do gateway ✅
O endpoint de documentação está disponível no runtime em http://localhost:5003/swagger.
