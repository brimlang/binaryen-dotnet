# AGENTS


# AGENTS

Repository Purpose:
- Provides cross-platform build, test, packaging, and publishing for Binaryen-based .NET projects.


Entry Points:
- CI Workflow: `.github/workflows/ci.yaml`
- Native build scripts: `eng/native/build.sh` (Linux/macOS), `eng/native/build.ps1` (Windows)
- Test project: `tests/Brim.Binaryen.Tests`
- Packaging: `src/Brim.Binaryen/Brim.Binaryen.csproj`


Inputs/Outputs:
- Input: Source code, version tags (e.g., `v1.2.3`)
- Output: Native binaries in `artifacts/native/<rid>/`, NuGet package in `artifacts/package/release/`


Conventions:
- RID-based artifact staging: `artifacts/native/<rid>/`
- CMake build cache: `artifacts/build/`
- Publish triggers: push to `main` (GitHub Packages), tag `v*` (NuGet.org)
- Native libraries are built and published as GitHub Releases tagged `binaryen-<nbgv_version>`, and are required for CI builds.


Environment:
- .NET SDK 9.0+
- CMake
- Clang/GCC (Linux/macOS) or Visual Studio (Windows)
- Git with full history (not shallow clone)
- Optionally, [mise](https://mise.jdx.dev/) for local build tasks


Critical Instructions:
- Always follow `.github/copilot-instructions.md` for build and test steps.
- Always run native build scripts before managed builds.
- NEVER CANCEL native build commands; they may take up to 60 minutes.
- Always specify explicit RID (`-r <rid>`) when running tests locally.
- Do NOT change the .NET target framework unless explicitly asked.
- Always run `dotnet format <project>` before committing changes.

Instructions for Agents:
- Check `.github/copilot-instructions.md` for authoritative build/test/packaging steps.
- Check `mise.toml` for local build tasks (optional).
- Use the CI workflow for build/test/pack/publish automation.
- Maintain artifact structure for compatibility with downstream automation.
- Validate changes by building, testing (with explicit RID), and formatting code.
- For troubleshooting, refer to the "Common Tasks and Solutions" and "Troubleshooting" sections in `.github/copilot-instructions.md`.
