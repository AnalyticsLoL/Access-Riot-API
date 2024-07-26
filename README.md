# Access-Riot-API
This C# app aims to download the data from the Riot API

## Installation
1. Create a new dotnet web api project:
```
dotnet run webapi -n AccessRiotAPI
cd AccessRiotAPI
```
2. Add http dotnet package
```
dotnet add package Microsoft.AspNetCore.Http
dotnet add package System.Net.Http
```

## Run
```
dotnet run
```
Open http://127.0.0.1:5151/swagger/index.html

The correct order to not get any conflict with the function requirements is:
1. Define which summoner you want to get information about with api/RiotData/settings see backend/testapp.txt for an example of body
2. Then requests the puuid, game name, summonerID, and accountID with /api/RiotData/summoner
3. Then use the /api/RiotData/matchhistory to access all the matchids of the player (All the parameters for this controller are optionals and except idStartList and idEndList are for now not functional)
4. Finally enter one of the game id in the /api/RiotData/matchinfo to see the game details