# Imagem separada para o SQLite. Ela compartilha o volume /data com a API, que
# executa as migrations no startup. Mantemos o container vivo para deixar clara
# a fronteira de infraestrutura do banco local no Docker Compose.
FROM alpine:3.21
WORKDIR /data
VOLUME ["/data"]
CMD ["sh", "-c", "touch /data/servicoapp.db && tail -f /dev/null"]
