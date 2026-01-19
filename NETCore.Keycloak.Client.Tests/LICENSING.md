Summary
=======

This repository contains test-only dependencies that were flagged by a license scanner. This file documents the flagged Microsoft license related to the `Microsoft.NET.Test.Sdk` package and the intended handling for auditors.

Flagged package
---------------

- Package: `Microsoft.NET.Test.Sdk`
- Version found: `17.6.0`
- Locator: `nuget+Microsoft.NET.Test.Sdk$17.6.0`
- Detected license: MS-NET (Microsoft Software License Terms)
- Project: `NETCore.Keycloak.Client.Tests` (direct dependency in `NETCore.Keycloak.Client.Tests/NETCore.Keycloak.Client.Tests.csproj`)

Why this is safe for consumers
------------------------------

- `Microsoft.NET.Test.Sdk` is a test-runner/test-SDK dependency used only to execute unit tests. It is not part of the runtime or production shipping artifacts for the library.
- The test project includes `IsTestProject=true` and the project-level `PackageReference`s have been marked with `PrivateAssets="all"` to prevent transitive flow to consuming packages.

Recommended actions for auditors
--------------------------------

1. If your organization policy accepts MS-NET for development/test tooling, allowlist the package or the MS-NET license in your scanner.
2. Alternatively, configure the license scanner to ignore dev/test-only dependencies or projects that have `<IsTestProject>true</IsTestProject>`.
3. If your policy forbids MS-NET entirely, remove or relocate test automation to an isolated repository or CI container and consult legal.

If you need, provide the scanner name and I can suggest or apply a scanner-specific ignore/allowlist configuration.
