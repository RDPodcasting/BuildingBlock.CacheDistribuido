dotnet sonarscanner begin /k:RDPodcasting_BuildingBlock.CacheDistribuido /d:sonar.host.url=https://sonarcloud.io /d:sonar.login=f8f29a21df7fb618bd80a765b67dd490232168b0  /o:codinsights 
dotnet restore BuildingBlock.CacheDistribuido.sln
dotnet build BuildingBlock.CacheDistribuido.sln
dotnet-sonarscanner end /d:sonar.login="f8f29a21df7fb618bd80a765b67dd490232168b0"