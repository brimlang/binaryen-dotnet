# Brim.Binaryen

[![CI](https://github.com/brimlang/binaryen-dotnet/actions/workflows/ci.yaml/badge.svg)](https://github.com/brimlang/binaryen-dotnet/actions/workflows/ci.yaml)
[![NuGet](https://img.shields.io/nuget/v/Brim.Binaryen.svg)](https://www.nuget.org/packages/Brim.Binaryen/)
[![GitHub Packages](https://img.shields.io/badge/packages%40github-Brim.Binaryen-blue)](https://github.com/brimlang/binaryen-dotnet/pkgs/nuget/Brim.Binaryen)

---

Brim.Binaryen provides .NET bindings for the Binaryen toolchain:

* A managed P/Invoke wrapper in the `Brim.Binaryen` namespace
* Prebuilt native `libbinaryen` shared library (per RID)
* The `wasm-opt` executable, for CLI-style optimization

This package is maintained by the Brim project, but is usable in any .NET application that needs Binaryen’s optimization or codegen APIs.

## Getting Started

Install from NuGet:

```sh
dotnet add package Brim.Binaryen
```

Example usage:

```csharp
using Brim.Binaryen;

// Probe the native library once at startup
Brim.Binaryen.Internal.BinaryenLoadGuard.EnsureLoaded();

// Create a simple module and run optimizations
using var m = BinaryenModule.Create();

var options = OptimizeOptions.O2()
    .AddPass("dce")
    .AddPass("inlining-optimizing")
    .Normalize();

m.RunOptimizations(options);

// Serialize back to wasm
var bytes = m.Write();
File.WriteAllBytes("out.wasm", bytes);
```

You can also invoke `wasm-opt` directly from the package’s `runtimes/<rid>/native` folder if you prefer the CLI.

## Repo Layout

```
.
├── src/Brim.Binaryen        # Core bindings
├── tests/Brim.Binaryen.Tests # Smoke/unit tests
├── eng/native               # Native build scripts (Linux/macOS/Windows)
├── extern/binaryen          # Upstream Binaryen (git submodule)
├── artifacts                # Build outputs (ignored by git)
└── ...
```

## Native Library Release Process

To improve CI performance, native shared libraries are built separately and published as GitHub releases:

### Creating Native Library Releases

Native libraries are built via the **Build Native Libraries** workflow (`.github/workflows/native-libraries.yaml`):

1. **Manual trigger**: Go to Actions → "Build Native Libraries" → "Run workflow"
2. **Automatic**: Runs monthly to check for new Binaryen versions  

The workflow:
- Builds native libraries for linux-x64, osx-arm64, osx-x64, and win-x64
- Creates a release tagged as `binaryen-{version}` (e.g., `binaryen-version_123`)
- Includes proper licensing information (LICENSE-BINARYEN, UPSTREAM.txt)
- Only runs if the release doesn't already exist

### How Main CI Uses Prebuilt Libraries

The main CI workflow (`.github/workflows/ci.yaml`) automatically:
1. Downloads prebuilt artifacts from the matching `binaryen-{version}` release
2. Falls back to building natively if no prebuilt release is available
3. Supports all platforms without long build times

This reduces typical CI runs from ~60 minutes to ~5 minutes by reusing prebuilt libraries.

## Developer Quickstart

This repo uses [mise](https://mise.jdx.dev/) for toolchain management.

**Prequisites:**
  You need to have Clang (or gcc) installed on Linux/macOS, or Visual Studio on Windows.


1. **Install [mise](https://mise.jdx.dev/)**
   Follow the instructions for your platform.

   * Linux/macOS:

     ```bash
     curl https://mise.run | sh
     ```

   * Windows (PowerShell):

     ```powershell
     irm get.mise.jdx.dev | iex
     ```

2. **Clone**

   ```bash
   git clone https://github.com/brimlang/binaryen-dotnet.git
   cd binaryen-dotnet
   ```

3. **Run the build + tests using mise tasks**

   ```bash
   mise install

   # Initialize submodules, etc.
   mise run setup

   # Build native Binaryen for your platform - alias bn
   mise run build-native

   # Build managed bindings - alias bm
   # Pass -c Release for release build
   mise run build-manage

   # Run unit tests (includes smoke tests of wasm-opt + Binaryen interop)
   mise run test
   ```

4. **Pack the NuGet package (local)**

   ```bash
   mise run pack
   ```

5. Clean Slate

  ```bash
  mise run nuke
  ```


NuGet packages land in `artifacts/package/release/`.


## Contributing

* Code is MIT licensed.
* Upstream Binaryen is Apache-2.0.
* Please run `dotnet format` before committing.
* PRs welcome!

---

## Versioning

This repo uses [Nerdbank.GitVersioning (nbgv)](https://github.com/dotnet/Nerdbank.GitVersioning).
Package versions are derived from Git tags and commit history.

---

## License and Provenance

* **This repository:** MIT licensed (see [LICENSE](LICENSE))
* **Binaryen (bundled binaries):** Apache 2.0 (see THIRD-PARTY-NOTICES)

When distributing or consuming this package, you must comply with both licenses.

---

## Status

* ✅ Basic bring-up complete (native builds + bindings)
* 🚧 Public API is still evolving — expect breaking changes until 120.1.0
* 🗣 Contributions and issues welcome

---

## Links

* [Binaryen upstream](https://github.com/WebAssembly/binaryen)
* [Brim project](https://github.com/brimlang)

---

## THIRD-PARTY-NOTICES

This project includes components derived from the Binaryen project.

### Binaryen

* Upstream: [https://github.com/WebAssembly/binaryen](https://github.com/WebAssembly/binaryen)
* License: Apache License 2.0

```
Apache License
Version 2.0, January 2004
http://www.apache.org/licenses/

TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION

Copyright 2015-2025 WebAssembly Community Group

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
```

