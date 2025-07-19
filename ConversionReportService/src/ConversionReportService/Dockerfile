FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5141
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ConversionReportService/ConversionReportService.csproj", "src/ConversionReportService/"]
COPY ["src/Infrastructure/ConversionReportService.Infrastructure.Persistence/ConversionReportService.Infrastructure.Persistence.csproj", "src/Infrastructure/ConversionReportService.Infrastructure.Persistence/"]
COPY ["src/Application/ConversionReportService.Application.Abstractions/ConversionReportService.Application.Abstractions.csproj", "src/Application/ConversionReportService.Application.Abstractions/"]
COPY ["src/Application/ConversionReportService.Application.Models/ConversionReportService.Application.Models.csproj", "src/Application/ConversionReportService.Application.Models/"]
COPY ["src/Application/ConversionReportService.Application/ConversionReportService.Application.csproj", "src/Application/ConversionReportService.Application/"]
COPY ["src/Application/ConversionReportService.Application.Contracts/ConversionReportService.Application.Contracts.csproj", "src/Application/ConversionReportService.Application.Contracts/"]
COPY ["src/Infrastructure/ConversionReportService.Infrastructure.BackgroundServices/ConversionReportService.Infrastructure.BackgroundServices.csproj", "src/Infrastructure/ConversionReportService.Infrastructure.BackgroundServices/"]
COPY ["src/Presentation/ConversionReportService.Presentation.Http/ConversionReportService.Presentation.Http.csproj", "src/Presentation/ConversionReportService.Presentation.Http/"]
COPY ["src/Infrastructure/ConversionReportService.Infrastructure.Caching/ConversionReportService.Infrastructure.Caching.csproj", "src/Infrastructure/ConversionReportService.Infrastructure.Caching/"]
COPY ["src/Infrastructure/ConversionReportService.Infrastructure.Kafka/ConversionReportService.Infrastructure.Kafka.csproj", "src/Infrastructure/ConversionReportService.Infrastructure.Kafka/"]
COPY ["src/Infrastructure/ConversionReportService.Infrastructure.RateLimiting/ConversionReportService.Infrastructure.RateLimiting.csproj", "src/Infrastructure/ConversionReportService.Infrastructure.RateLimiting/"]
COPY ["src/Presentation/ConversionReportService.Presentation.Grpc/ConversionReportService.Presentation.Grpc.csproj", "src/Presentation/ConversionReportService.Presentation.Grpc/"]
COPY ["src/Client/ConversionReportService.Client/ConversionReportService.Client.csproj", "src/Client/ConversionReportService.Client/"]
RUN dotnet restore "src/ConversionReportService/ConversionReportService.csproj"
COPY . .
WORKDIR "/src/src/ConversionReportService"
RUN dotnet build "./ConversionReportService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ConversionReportService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConversionReportService.dll"]
