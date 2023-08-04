# Setting DOCKER_BUILDKIT to 0 because of an on-going issue:
# https://github.com/EventStore/EventStore/issues/3013
# https://github.com/moby/buildkit/issues/1900
$env:DOCKER_BUILDKIT=0; docker build -t rexor12/holo.net:latest .