# Stage 1 – Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY EmployeeApi.slnx ./
COPY EmployeeApi/EmployeeApi.csproj        EmployeeApi/
COPY EmployeeApi.Tests/EmployeeApi.Tests.csproj EmployeeApi.Tests/

RUN dotnet restore

COPY . .

RUN dotnet publish EmployeeApi/EmployeeApi.csproj \
        -c Release \
        -o /app/publish \
        --no-restore

# Stage 2 – Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "EmployeeApi.dll"]
