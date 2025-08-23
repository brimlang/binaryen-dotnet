# AGENTS

Repository Purpose:
- Provides cross-platform build, test, packaging, and publishing for Binaryen-based .NET projects.

Entry Points:
- CI Workflow: `.github/workflows/ci.yaml`
- Native build scripts: `eng/native/build.sh`
- Test project: `tests/Brim.Binaryen.Tests`
- Packaging: `src/Brim.Binaryen/Brim.Binaryen.csproj`

Inputs/Outputs:
- Input: Source code, version tags (e.g., `v1.2.3`)
- Output: Native binaries in `artifacts/native/<rid>/`, NuGet package in `artifacts/package/release/`

Conventions:
- RID-based artifact staging: `artifacts/native/<rid>/`
- CMake build cache: `artifacts/build/`
- Publish triggers: push to `main` (GitHub Packages), tag `v*` (NuGet.org)

Environment:
- .NET SDK 9
- CMake
- Platform-specific build tools

Instructions for Agents:
- Check mise.toml for local build tasks.
- Use the CI workflow for build/test/pack/publish automation.
- Maintain artifact structure for compatibility with downstream automation.
- Do not change the .NET target framework unless asked.
