# Game build policy

These files are compiler policy inputs, not text notes. [Directory.Build.targets](../Directory.Build.targets) attaches them to the applicable projects. [Code rules](../../../docs/code/README.md), the [rule register](../../../docs/code/register.json) and [exact exemptions](../../../docs/code/exemptions.json) remain the authority.

| File | Applied scope and reason |
| --- | --- |
| [BannedSymbols.txt](BannedSymbols.txt) | Production consumers: require explicit time/random inputs, explicit dependencies, controlled output and asynchronous coordination. Examples include rejecting `DateTime.Now`, unseeded randomness, service locators and blocking task waits. |
| [BannedSymbols.Pure.txt](BannedSymbols.Pure.txt) | Core and multiplayer contracts: additionally reject `GodotObject` to keep these assemblies engine-independent. |
| [BannedSymbols.Tests.txt](BannedSymbols.Tests.txt) | Test consumers: require Shouldly assertions, explicit test dependencies and the designated reporting mechanism. Process-proof tests can observe clocks and coordinate child processes. |
| [Directory.Xunit.targets](Directory.Xunit.targets) | Preserve the authentic pinned xUnit entry-point producer, verify its bytes and record a receipt. A stable logical seed avoids deriving the generated namespace from the checkout path. |

The production and test lists have distinct scopes; tests do not inherit every production API prohibition. Keep the exact compiler policy and positive/negative proofs aligned when changing a list. Do not remove a list to make the build pass or widen generated-source permissions to accommodate authored code.
