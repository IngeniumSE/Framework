#!/bin/bash

cd ./build/apps/build

dotnet run --target ${1:-Default}