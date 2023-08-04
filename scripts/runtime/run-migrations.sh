#!/usr/bin/env bash

./wait-for-it.sh $HOLO_Database__Host:$HOLO_Database__Port -- dotnet ./Holo.Migrator/Holo.Migrator.dll update-database -c $HOLO_DatabaseOptions__ConnectionString -a ./Holo.Migrator/Migrations/**/Holo.Module.*.Migrations.dll -n "^Holo\\.Module\\.(?:\\w+)\\.Migrations\$"