#!/usr/bin/env bash

./run-migrations.sh

# Instead of running dotnet directly, use exec to discard the shell as the parent process.
# This is needed to receive Docker's signals.
cd ./Holo.ServiceHost
exec dotnet ./Holo.ServiceHost.dll