FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src

COPY hello.csproj ./
RUN dotnet restore hello.csproj

COPY . ./
RUN dotnet publish hello.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

CMD ["dotnet", "hello.dll"]
