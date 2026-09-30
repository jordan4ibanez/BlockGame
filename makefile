default:
	@dotnet watch --project src/BlockGame.csproj

oneshot:
	@dotnet run --project src/BlockGame.csproj

gdb:
	@dotnet build src/BlockGame.csproj
	@gdb -ex r --args dotnet src/bin/Debug/net10.0/BlockGame.dll

clean:
	@dotnet clean src/BlockGame.csproj