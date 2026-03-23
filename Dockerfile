# Build stage
ARG DOTNET_VERSION=10.0
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /src

COPY corelink_server.sln ./
COPY Corelink.Application/Corelink.Application.csproj Corelink.Application/
COPY Corelink.Domain/Corelink.Domain.csproj Corelink.Domain/
COPY Corelink.Infrastructure/Corelink.Infrastructure.csproj Corelink.Infrastructure/
COPY Corelink.Presentation/Corelink.Presentation.csproj Corelink.Presentation/

RUN dotnet restore ./corelink_server.sln

COPY . .
RUN dotnet publish Corelink.Presentation/Corelink.Presentation.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS runtime
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["sh", "-c", "dotnet Corelink.Presentation.dll --urls http://0.0.0.0:${PORT:-8080}"]
