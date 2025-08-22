#!/usr/bin/env bash
set -Eeuo pipefail

# --- CLI ----------------------------------------------------
RID="${RID:-}"   # e.g., linux-x64, osx-arm64, osx-x64; default = host
CLEAN=0          # 1 = wipe cmake cache
TOOLS=1          # 1 = build wasm-opt, 0 = lib only
JOBS="${JOBS:-}" # parallelism; default cmake --build auto
while [[ $# -gt 0 ]]; do
  case "$1" in
  --rid)
    RID="$2"
    shift 2
    ;;
  --clean)
    CLEAN=1
    shift
    ;;
  --no-tools)
    TOOLS=0
    shift
    ;;
  -j | --jobs)
    JOBS="$2"
    shift 2
    ;;
  -h | --help)
    echo "Usage: $0 [--rid <linux-x64|osx-arm64|osx-x64>] [--clean] [--no-tools] [--jobs N]"
    exit 0
    ;;
  *)
    echo "Unknown arg: $1"
    exit 2
    ;;
  esac
done

# --- repo roots ---------------------------------------------
ROOT="$(git rev-parse --show-toplevel 2>/dev/null || true)"
if [[ -z "$ROOT" ]]; then ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")"/../.. && pwd)"; fi
BINY="$ROOT/extern/binaryen"

# --- host detection & RID default ----------------------------
uname_s="$(uname -s)"
uname_m="$(uname -m)"
if [[ -z "$RID" ]]; then
  case "$uname_s" in
  Linux) RID="linux-x64" ;;
  Darwin) RID=$([[ "$uname_m" == "arm64" ]] && echo "osx-arm64" || echo "osx-x64") ;;
  *)
    echo "Unsupported host OS: $uname_s"
    exit 1
    ;;
  esac
fi

case "$RID" in
linux-x64)
  LIB=libbinaryen.so
  RPATH="\$ORIGIN"
  ;;
osx-arm64)
  LIB=libbinaryen.dylib
  RPATH="@loader_path"
  ;;
osx-x64)
  LIB=libbinaryen.dylib
  RPATH="@loader_path"
  ;;
*)
  echo "Unsupported RID: $RID"
  exit 1
  ;;
esac

# --- toolchain ----------------------------------------------
GEN="Ninja"
command -v ninja >/dev/null 2>&1 || GEN="Unix Makefiles"

# Prefer clang; allow explicit override; fallback gcc
if command -v clang >/dev/null 2>&1 && command -v clang++ >/dev/null 2>&1; then
  : "${CC:=clang}"
  : "${CXX:=clang++}"
else
  echo "clang not found; falling back to gcc/g++ (set CC/CXX to override)"
  : "${CC:=gcc}"
  : "${CXX:=g++}"
fi

# macOS deployment target (keeps binaries runnable on older systems)
if [[ "$RID" == osx-* ]]; then
  export MACOSX_DEPLOYMENT_TARGET="${MACOSX_DEPLOYMENT_TARGET:-11.0}"
fi

# --- submodules ---------------------------------------------
git -C "$ROOT" submodule update --init --recursive extern/binaryen

# --- build dir per RID --------------------------------------
BUILD="$ROOT/artifacts/build/native/binaryen-$RID"
if [[ "$CLEAN" == "1" ]]; then rm -rf "$BUILD"; fi

mkdir -p "$BUILD"

# Arch flags for mac
CMAKE_OSX_ARCH=""
if [[ "$RID" == "osx-arm64" ]]; then CMAKE_OSX_ARCH="-DCMAKE_OSX_ARCHITECTURES=arm64"; fi
if [[ "$RID" == "osx-x64" ]]; then CMAKE_OSX_ARCH="-DCMAKE_OSX_ARCHITECTURES=x86_64"; fi

# Build tools flag
BUILD_TOOLS="-DBUILD_TOOLS=$([[ "$TOOLS" == "1" ]] && echo ON || echo OFF)"

# --- configure ----------------------------------------------
cmake -S "$BINY" -B "$BUILD" -G "$GEN" \
  -DCMAKE_BUILD_TYPE=Release \
  -DBUILD_SHARED_LIBS=ON \
  -DBUILD_TESTS=OFF \
  "$BUILD_TOOLS" \
  -DCMAKE_C_COMPILER="$CC" \
  -DCMAKE_CXX_COMPILER="$CXX" \
  -DCMAKE_INSTALL_RPATH="$RPATH" \
  -DCMAKE_BUILD_WITH_INSTALL_RPATH=ON \
  -DCMAKE_CXX_STANDARD=17 -DCMAKE_CXX_STANDARD_REQUIRED=ON \
  $CMAKE_OSX_ARCH

# --- build ---------------------------------------------------
if [[ -n "$JOBS" ]]; then
  cmake --build "$BUILD" --parallel "$JOBS"
else
  cmake --build "$BUILD" --parallel
fi

# --- stage ---------------------------------------------------
DEST="$ROOT/artifacts/native/$RID"
mkdir -p "$DEST"

cp "$BUILD/lib/$LIB" "$DEST/"
shopt -s nullglob
for exe in "$BUILD"/bin/wasm-opt*; do cp "$exe" "$DEST/"; done
shopt -u nullglob
chmod +x "$DEST"/wasm-opt* 2>/dev/null || true

# strip (best-effort)
STRIP="${STRIP:-strip}"
$STRIP "$DEST/$LIB" 2>/dev/null || true
$STRIP "$DEST"/wasm-opt* 2>/dev/null || true

# provenance
pushd "$BINY" >/dev/null
TAG="$(git describe --tags --always --dirty || true)"
SHA="$(git rev-parse --short=12 HEAD || true)"
popd >/dev/null
printf "binaryen-tag: %s\nbinaryen-commit: %s\n" "$TAG" "$SHA" >"$ROOT/UPSTREAM.txt"

echo "✅ Built $RID → $DEST ($LIB + wasm-opt*)"
