# binaryen-dotnet GitHub Copilot Instructions

**CRITICAL: Always follow these instructions exactly and only search for additional information if these instructions are incomplete or incorrect.**

This repository provides .NET bindings for the Binaryen WebAssembly optimization toolkit. It includes managed P/Invoke wrappers, native Binaryen shared libraries, and the wasm-opt CLI tool for multiple platforms.

## Working Effectively

### Prerequisites
- Install Clang/GCC (Linux/macOS) or Visual Studio (Windows) - REQUIRED for native builds
- .NET 8.0+ SDK - REQUIRED for managed builds
- CMake and optionally Ninja for faster builds
- Git with full history (not shallow clone)

### Bootstrap and Build (REQUIRED SEQUENCE)
Always run commands in this exact order:

```bash
# 1. Initialize submodules (REQUIRED FIRST - but build script also handles this)
git submodule update --init --recursive extern/binaryen

# 2. Fix git versioning if shallow clone (only needed for shallow clones)
git fetch --unshallow  # Only if you see "Shallow clone lacks objects" errors

# 3. Build native Binaryen for current platform
./eng/native/build.sh

# Windows alternative (UNTESTED - may need manual setup):
# .\eng\native\build.ps1
```

**CRITICAL BUILD TIMING:**
- **NEVER CANCEL** the native build command `./eng/native/build.sh`
- Native build takes 8-60 minutes depending on system (validated: ~8 minutes on typical systems)
- **Set timeout to 3600 seconds (60 minutes) minimum**
- The build may appear to hang - this is normal, wait for completion
- Build script automatically initializes submodules if missing

**Build script options:**
```bash
./eng/native/build.sh --clean     # Wipe CMake cache and rebuild from scratch
./eng/native/build.sh --no-tools  # Build library only, skip wasm-opt
./eng/native/build.sh --rid linux-x64  # Explicit target platform  
```

```bash
# 4. Restore managed dependencies
dotnet restore src/Brim.Binaryen/Brim.Binaryen.csproj

# 5. Build managed code
dotnet build src/Brim.Binaryen/Brim.Binaryen.csproj
```

### Testing (CRITICAL: Always use explicit RID)
**NEVER run tests without specifying runtime identifier:**
```bash
# Run tests - MUST include -r flag with correct RID
dotnet test tests/Brim.Binaryen.Tests -r linux-x64 --verbosity minimal

# For other platforms use:
# -r osx-arm64    (macOS Apple Silicon)
# -r osx-x64      (macOS Intel)  
# -r win-x64      (Windows)
```

**Test timing:** Tests complete in 5-10 seconds once native libraries are built.

### Packaging
```bash
# Build in release mode first
dotnet build src/Brim.Binaryen/Brim.Binaryen.csproj -c Release

# Create NuGet package
dotnet pack src/Brim.Binaryen/Brim.Binaryen.csproj -c Release -o artifacts/package/release
```

## Validation Steps

### ALWAYS validate changes with these steps:
1. **Build validation:** Run the complete build sequence above
2. **Test validation:** Run tests with explicit RID: `dotnet test tests/Brim.Binaryen.Tests -r linux-x64`
3. **API validation:** Create a simple test program that creates a BinaryenModule and runs optimizations
4. **CLI validation:** Test wasm-opt tool: `./artifacts/native/linux-x64/wasm-opt --help`
5. **Format validation:** `dotnet format src/Brim.Binaryen/Brim.Binaryen.csproj --verify-no-changes`

### Manual API Testing
After making changes, always validate the core API works:
```csharp
using Brim.Binaryen;

// Test basic functionality
using var module = new BinaryenModule();
var options = OptimizeOptions.O2().AddPass(Passes.Dce).Normalize();
module.Optimize(options);
var bytes = module.WriteBinary();
Console.WriteLine($"Module serialized: {bytes.Length} bytes");
```

## Common Tasks and Solutions

### Build Issues
- **"Shallow clone lacks objects"**: Run `git fetch --unshallow` (only on shallow clones)
- **"Lock type not found"**: Ensure using .NET 8 compatible code (use `object` instead of `Lock`)
- **"libbinaryen.so not found"**: Native build didn't complete or artifacts missing
- **CMake cache issues**: Run `./eng/native/build.sh --clean` to wipe CMake cache
- **Build takes too long**: Normal - native C++ compilation is slow, wait for completion
- **Windows build issues**: The PowerShell script is untested - prefer Linux/macOS for development

### Test Issues  
- **Tests fail with library not found**: Must use explicit RID (`-r linux-x64`) and ensure native build completed
- **Tests hang**: Check that native artifacts exist in `artifacts/native/linux-x64/`
- **Platform-specific test failures**: Use correct RID for your platform (linux-x64, osx-arm64, osx-x64, win-x64)

### Alternative Build Methods

#### Using mise (preferred but optional)
If mise is available, you can use task shortcuts:
```bash
mise install          # Install tools
mise run setup         # Initialize submodules + restore
mise run build-native  # Build native (alias: bn)
mise run build-managed # Build managed (alias: bm)  
mise run test          # Run tests (alias: t)
mise run pack          # Create packages (alias: p)
mise run nuke          # Clean everything (alias: n)
```

#### Manual Commands (always work)
Use the explicit commands above if mise is not available.

## Repository Structure

### Key Projects and Paths
- **Main library:** `src/Brim.Binaryen/` - Core P/Invoke bindings
- **Tests:** `tests/Brim.Binaryen.Tests/` - Smoke tests and unit tests
- **Native build:** `eng/native/build.sh` (Linux/macOS), `eng/native/build.ps1` (Windows)
- **Upstream Binaryen:** `extern/binaryen/` - Git submodule, do not modify directly
- **Build outputs:** `artifacts/` - Ignored by git, contains native libraries and packages
- **Native artifacts:** `artifacts/native/<rid>/` - Platform-specific binaries (libbinaryen.so/dylib/dll + wasm-opt)

### Configuration Files  
- **mise.toml:** Task definitions and tool versions (optional)
- **global.json:** .NET SDK version requirements
- **Directory.Build.props:** Shared MSBuild properties
- **version.json:** Nerdbank.GitVersioning configuration

### Important Build Artifacts
- `artifacts/native/linux-x64/libbinaryen.so` - Core native library
- `artifacts/native/linux-x64/wasm-opt` - WebAssembly optimizer CLI tool
- `artifacts/package/release/*.nupkg` - NuGet packages

## Timing Expectations and Critical Warnings

### NEVER CANCEL Commands
- **Native build (`./eng/native/build.sh`):** 8-60 minutes - ALWAYS wait for completion
- Use timeout values of 3600+ seconds (60+ minutes) when scripting builds
- Build times vary significantly by CPU cores and disk speed
- **Validated timing on standard systems: ~8 minutes**

### Quick Commands (all validated)
- Git submodule init: ~30-120 seconds (depends on network)
- Managed restore: ~2-5 seconds  
- Managed build: ~3-5 seconds  
- Tests: ~5 seconds
- Packaging: ~3 seconds
- Format check: ~1-2 seconds

### Full Developer Workflow Timing
Complete first-time setup (validated sequence):
1. Submodule init: ~60 seconds
2. Native build: ~8 minutes  
3. Managed restore: ~3 seconds
4. Managed build: ~5 seconds
5. Tests: ~5 seconds  
6. Total: **~9 minutes for complete setup**

## CI/CD Integration Notes
- The repository uses `.github/workflows/ci.yaml` for automated builds
- CI builds native libraries on multiple platforms (linux-x64, osx-arm64, etc.)
- Tests run with explicit RID specification
- Packages are published to GitHub Packages (main branch) and NuGet.org (version tags)

## Troubleshooting

### When Builds Fail
1. Check prerequisites are installed (clang, cmake, .NET SDK)
2. Verify git submodules are initialized
3. Ensure full git history available (`git fetch --unshallow`)
4. Try clean build: `./eng/native/build.sh --clean`
5. Check disk space (build requires several GB)

### When Tests Fail
1. Ensure native build completed successfully
2. Always specify RID: `-r linux-x64` 
3. Check that native artifacts exist in `artifacts/native/<rid>/`
4. Verify the native library loads: check for DLL/shared library loading errors

### Performance Notes
- Use `ninja` if available (faster than make)
- Native builds are CPU-intensive (use all available cores)
- Builds generate significant disk I/O

Always run `dotnet format <project>` before committing changes.