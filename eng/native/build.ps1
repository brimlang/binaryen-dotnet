# Dev Note: This has not been tested. It was produced by an AI (GPT-5).
Param(
  [string]$Rid = "",
  [switch]$Clean,
  [switch]$NoTools
)

$ErrorActionPreference = "Stop"

# repo root
$ROOT = (& git rev-parse --show-toplevel) 2>$null
if (-not $ROOT) { $ROOT = (Resolve-Path "$PSScriptRoot\..\..").Path }

$BINY = Join-Path $ROOT "extern/binaryen"

# RID default
if (-not $Rid) { $Rid = "win-x64" }
if ($Rid -ne "win-x64") { throw "Unsupported RID: $Rid" }

# Submodules
& git -C $ROOT submodule update --init --recursive extern/binaryen

# Build dir
$BUILD = Join-Path $ROOT "artifacts/build/binaryen-$Rid"
if ($Clean) { Remove-Item -Recurse -Force -ErrorAction Ignore $BUILD }
New-Item -ItemType Directory -Force $BUILD | Out-Null

# Use MSVC from Developer Prompt; fallback to cl in PATH
$CC  = $env:CC  ; if (-not $CC)  { $CC  = "cl" }
$CXX = $env:CXX ; if (-not $CXX) { $CXX = "cl" }

# generator
$GEN = "Ninja"
if (-not (Get-Command ninja -ErrorAction SilentlyContinue)) { $GEN = "NMake Makefiles" }

# tools flag
$buildTools = if ($NoTools) { "OFF" } else { "ON" }

# configure
cmake -S $BINY -B $BUILD -G $GEN `
  -DCMAKE_BUILD_TYPE=Release `
  -DBUILD_SHARED_LIBS=ON `
  -DBUILD_TESTS=OFF `
  -DBUILD_TOOLS=$buildTools `
  -DCMAKE_C_COMPILER="$CC" `
  -DCMAKE_CXX_COMPILER="$CXX"

# build
cmake --build $BUILD --parallel

# stage
$DEST = Join-Path $ROOT "artifacts/native/$Rid"
New-Item -ItemType Directory -Force $DEST | Out-Null

Copy-Item (Join-Path $BUILD "bin\wasm-opt*.exe") $DEST -ErrorAction SilentlyContinue
Copy-Item (Join-Path $BUILD "bin\binaryen.dll") $DEST -ErrorAction SilentlyContinue
Copy-Item (Join-Path $BUILD "binaryen.dll") $DEST -ErrorAction SilentlyContinue

# provenance
Push-Location $BINY
$TAG = (& git describe --tags --always --dirty) 2>$null
$SHA = (& git rev-parse --short=12 HEAD) 2>$null
Pop-Location
"binaryen-tag: $TAG`nbinaryen-commit: $SHA" | Out-File -Encoding utf8 (Join-Path $ROOT "UPSTREAM.txt")

Write-Host "✅ Built $Rid → $DEST (binaryen.dll + wasm-opt.exe)"

