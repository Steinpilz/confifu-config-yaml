#!/bin/bash
set -e
set -o allexport
eval $(cat .env | tr -d '\r' | sed 's_%\([^%]*\)%_$\1_g')
set +o allexport
if [ ! -d "packages" ]; then mono .paket/paket.exe restore; fi
mono packages/FAKE/tools/FAKE.exe scripts/build.fsx $@
