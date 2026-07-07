# Stage 1 — SDK: restore, build, publish
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy NuGet config first — clears Windows fallback package folders
COPY NuGet.Config ./

# Copy solution + project files so restore layer is cached separately from source
COPY EmployeeApi.sln ./
COPY src/EmployeeApi.Domain/EmployeeApi.Domain.csproj             src/EmployeeApi.Domain/
COPY src/EmployeeApi.Application/EmployeeApi.Application.csproj   src/EmployeeApi.Application/
COPY src/EmployeeApi.Infrastructure/EmployeeApi.Infrastructure.csproj src/EmployeeApi.Infrastructure/
COPY src/EmployeeApi.Api/EmployeeApi.Api.csproj                   src/EmployeeApi.Api/
COPY tests/EmployeeApi.Tests/EmployeeApi.Tests.csproj              tests/EmployeeApi.Tests

RUN dotnet restore

# Copy the rest of the source and publish
COPY . .
RUN dotnet publish src/EmployeeApi.Api/EmployeeApi.Api.csproj \
    --configuration Release \
    --output /app/publish

# Stage 2 — Runtime: lean ASP.NET image, no SDK
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# ASP.NET Core listens on 8080 inside the container by default in .NET 8
EXPOSE 8080

ENTRYPOINT ["dotnet", "EmployeeApi.Api.dll"]
