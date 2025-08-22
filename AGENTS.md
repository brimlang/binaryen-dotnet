# Agents / CI Guide

This repo ships with a single workflow that handles **build → test → pack → publish** across platforms.

* File: `.github/workflows/ci.yaml`
* Registries:

  * **GitHub Packages**: pushes on every successful push to `main`
  * **NuGet.org**: pushes only on version tags like `v1.2.3`

---

## Jobs Overview

### 1) `native`

Builds Binaryen for each RID using the scripts under `eng/native/` and uploads `artifacts/native/<rid>/`.

Matrix:

* `linux-x64` (ubuntu-latest)
* `osx-arm64` (macos-14)
* `osx-x64` (macos-13)
* `win-x64` (windows-2022)

### 2) `test`

Downloads the matching native artifact for the runner RID and runs `dotnet test`.

* Tests require a RID. CI passes `-r` explicitly or sets the RID via matrix.
* Native bits are copied beside the testhost from `artifacts/native/<rid>/`.

### 3) `pack`

Runs `dotnet pack` once on Ubuntu and uploads the resulting `.nupkg` to the workflow artifacts.

### 4) `publish-gh`

Runs on pushes to `main`. Pushes the `.nupkg` to **GitHub Packages** using `GITHUB_TOKEN`.

### 5) `publish-nuget`

Runs only when the ref starts with `refs/tags/v`. Pushes to **NuGet.org** using `NUGET_API_KEY`.

---

## Secrets & Permissions

* `permissions: packages: write` is set at the workflow root to allow publishing to GitHub Packages.
* **Secrets**:

  * `GITHUB_TOKEN`: built-in; used for GitHub Packages publish.
  * `NUGET_API_KEY`: add as a repo secret for NuGet.org publish.

---

## Maintainer Tasks

### Bump Binaryen

```
cd extern/binaryen
git fetch --tags origin
git checkout tags/<version_tag>
cd ../..
# Optionally build locally to refresh UPSTREAM.txt
./eng/native/build.sh
git add extern/binaryen UPSTREAM.txt
git commit -m "Bump Binaryen to <version_tag>"
```

### Cut a Release (NuGet)

1. Ensure `main` is green.
2. Tag the repo: `git tag vX.Y.Z && git push origin vX.Y.Z`.
3. Workflow packs and publishes to NuGet.org automatically.

---

## Local Dev Cheatsheet

```bash
# Build native for current host
./eng/native/build.sh

# Test (explicit RID)
dotnet test tests/Brim.Binaryen.Tests -r linux-x64

# Pack (managed + staged native payload)
dotnet pack src/Brim.Binaryen/Brim.Binaryen.csproj -c Release -o artifacts/package/release
```

---

## Design Choices (quick justifications)

* **No custom native resolvers**: we keep resolution explicit by RID for predictability.
* **Staging under artifacts/native/<rid>**: a single predictable location used by tests and packing.
* **CMake caches under artifacts/build/**: keeps the submodule pristine and easy to clean.
* **One workflow**: easier to reason about, with publish phases gated by branch/tag.

