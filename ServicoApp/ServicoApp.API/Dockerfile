# Padrao Builder: a imagem usa um estagio com SDK para compilar/publicar e outro
# estagio menor apenas para executar a API. Assim o container final nao carrega
# ferramentas de build, reduzindo tamanho e superficie de manutencao.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ServicoApp.API/ServicoApp.API.csproj ServicoApp.API/
COPY ServicoApp.Application/ServicoApp.Application.csproj ServicoApp.Application/
COPY ServicoApp.Domain/ServicoApp.Domain.csproj ServicoApp.Domain/
COPY ServicoApp.Infrastructure/ServicoApp.Infrastructure.csproj ServicoApp.Infrastructure/
RUN dotnet restore ServicoApp.API/ServicoApp.API.csproj

COPY . .
RUN dotnet publish ServicoApp.API/ServicoApp.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ServicoApp.API.dll"]
