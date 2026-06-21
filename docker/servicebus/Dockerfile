# Imagem separada para um Service Bus mockado. O backend usa a abstracao IServiceBus;
# este container representa o broker externo no Compose para desenvolvimento local.
FROM python:3.12-alpine
WORKDIR /app
COPY mock_service_bus.py .
EXPOSE 8080
CMD ["python", "mock_service_bus.py"]
