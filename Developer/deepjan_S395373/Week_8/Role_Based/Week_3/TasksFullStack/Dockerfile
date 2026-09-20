# Stage 1: Build React frontend
FROM node:24-alpine AS frontend
WORKDIR /src/TasksWeb

COPY TasksWeb/package.json ./
RUN npm install

COPY TasksWeb/ ./
RUN npm run build

# Stage 2: Build ASP.NET Core backend and include the React build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY TasksApi/TasksApi.csproj TasksApi/
RUN dotnet restore TasksApi/TasksApi.csproj

COPY TasksApi/ TasksApi/
COPY --from=frontend /src/TasksWeb/dist/ /src/TasksApi/wwwroot/

RUN dotnet publish TasksApi/TasksApi.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Stage 3: Small runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish/ ./

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 10000

CMD ["sh", "-c", "dotnet TasksApi.dll --urls http://0.0.0.0:${PORT:-10000}"]
