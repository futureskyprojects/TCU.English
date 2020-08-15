#!/bin/bash

set -e
run_cmd="dotnet run --server.urls http://*:80"

timestamp=$(date +%s)
migrationName = "InitialCreate.Version.${timestamp}"

# Create new migration
until dotnet ef migrations add ${migrationName}; do
>&2 echo "Create new migration ${migrationName}"
sleep 1
done

# Update database
until dotnet ef database update ${migrationName}; do
>&2 echo "MySQL Server is starting up date migration ${migrationName}"
sleep 1
done

>&2 echo "MySQL Server is up - executing command"
exec $run_cmd