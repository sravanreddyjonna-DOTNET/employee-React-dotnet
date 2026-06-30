# Stage 1 — SDK: restore, build, publish
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution + project files first so restore is cached separately from source
COPY EmployeeApi.sln ./
COPY src/EmployeeApi.Domain/EmployeeApi.Domain.csproj             src/EmployeeApi.Domain/
COPY src/EmployeeApi.Application/EmployeeApi.Application.csproj   src/EmployeeApi.Application/
COPY src/EmployeeApi.Infrastructure/EmployeeApi.Infrastructure.csproj src/EmployeeApi.Infrastructure/
COPY src/EmployeeApi.Api/EmployeeApi.Api.csproj                   src/EmployeeApi.Api/

RUN dotnet restore

# Copy the rest of the source and publish
COPY . .
RUN dotnet publish src/EmployeeApi.Api/EmployeeApi.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

# Stage 2 — Runtime: lean ASP.NET image, no SDK
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# ASP.NET Core listens on 8080 inside the container by default in .NET 8
EXPOSE 8080

ENTRYPOINT ["dotnet", "EmployeeApi.Api.dll"]
