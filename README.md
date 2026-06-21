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
