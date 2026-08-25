#!/usr/bin/env sh
set -eu

configuration="${1:-Release}"
script_dir=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
repository_root=$(CDPATH= cd -- "$script_dir/.." && pwd)
source_directory="$repository_root/src/WxSharp.Native"
build_directory="$repository_root/build/native"

set -- -S "$source_directory" -B "$build_directory" "-DCMAKE_BUILD_TYPE=$configuration"
if [ -n "${CMAKE_TOOLCHAIN_FILE:-}" ]; then
  set -- "$@" "-DCMAKE_TOOLCHAIN_FILE=$CMAKE_TOOLCHAIN_FILE"
fi

cmake "$@"
cmake --build "$build_directory" --config "$configuration" --parallel

printf 'Native build completed: %s\n' "$build_directory"
