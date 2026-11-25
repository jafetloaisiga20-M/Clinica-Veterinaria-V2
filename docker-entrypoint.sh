#!/bin/sh
set -e

# Render y otros hostings inyectan la variable PORT.
if [ -n "$PORT" ]; then
    export ASPNETCORE_URLS="http://+:${PORT}"
fi

exec dotnet PRESENTATION.dll
