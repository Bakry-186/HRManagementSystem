# Stage 1: restore + build + publish
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY HRManagementSystem.sln ./
COPY HRM.API/HRM.API.csproj                       HRM.API/
COPY HRM.Application/HRM.Application.csproj       HRM.Application/
COPY HRM.Domain/HRM.Domain.csproj                 HRM.Domain/
COPY HRM.Infrastructure/HRM.Infrastructure.csproj HRM.Infrastructure/
COPY HRM.Tests/HRM.Tests.csproj                   HRM.Tests/

RUN dotnet restore

COPY . .

RUN dotnet publish HRM.API/HRM.API.csproj -c Release -o /app/publish

# Stage 2: runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "HRM.API.dll"]
