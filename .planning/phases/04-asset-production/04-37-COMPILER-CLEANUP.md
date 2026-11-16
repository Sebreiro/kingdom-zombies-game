# Coordinator-directed compiler cleanup C1

Recorded after checkpoint `a7883c1`. R2's two-attempt budget remains exhausted at zero; it is not reset. Its behavioral implementation and the six-file WIP remain preserved in the recorded immutable backup.

The coordinator authorizes one narrow compiler/style cleanup as routine completion of the user's existing autonomous Phase 4 implementation request. Both R2 attempts stopped at compilation, so the new behavior has not yet been evaluated. This direction permits no new proof design, permission, implementation path or weakening of a check.

Only `tests/phase1/EngineGeneratedTests.cs` may change:

1. Correct the formatting diagnostic around the collection expression near line 247; whitespace formatting may cover this same file.
2. Split the existing 66-line `RejectInventoryMutations` body into private helpers below the existing source-rule length limit, preserving all mutation cases, order and assertions.
3. Correct MA0002 at the actual current line 618: it is the string `historical.ShouldNotBe(expected: fixture.DirectoryPath)` assertion, not the dictionary removal at line 616. Use an explicit ordinal comparison and assert its Boolean result, preserving the required unequal-path assertion.

Run exactly one strict test-project compile after these corrections. If it fails, make no more source changes and return the actual diagnostics. If it passes, run all existing written R2 controls without further behavior edits. A failed behavioral control returns an honest checkpoint with R2 still zero.

If every R2 control passes, R3 remains authorized under its existing budget: focused failed/affected controls, then the fresh full 18-case selection, restored native/solution/Core positives and all frozen-input checks. Complete Task 2 only after every required gate passes. Preserve prior failures and disclose C1 in the final summary. No frozen03 change, real game window, source-policy/visibility change or acceptance credit is authorized here.
