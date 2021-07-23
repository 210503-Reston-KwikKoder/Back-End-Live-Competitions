FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app 

COPY *.sln .

COPY LiveCompetitionREST/*.csproj ./CBERest/
COPY LiveCompetitionBL/*.csproj ./CBEBL/
COPY LiveCompetitionDL/*.csproj ./CBEDL/
COPY LiveCompetitionModels/*.csproj ./CBEModels/
COPY LiveCompetitionTests/*.csproj ./CBETests/

RUN cd LiveCompetitionREST && dotnet restore

COPY . ./


RUN dotnet publish LiveCompetitionREST -c Release -o publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app 

COPY --from=build /app/publish ./
CMD ["dotnet", "LiveCompetitionREST.dll"]