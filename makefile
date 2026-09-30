default:
	@dotnet watch

oneshot:
	@dotnet run

gdb:
	@dotnet build
	@gdb -ex r --args dotnet bin/Debug/net10.0/BlockGame.dll

clean:
	@dotnet clean