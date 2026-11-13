# Deferred execution issues

## 01-04 Task 2 strict-build checkpoint

The third bounded automatic correction pass leaves 151 strict-build diagnostics: 111 CS1591, 36 CA1707, two MA0051 and two MA0023. They block Task 2 completion; they are not accepted warnings, skipped tests or passed proof cases. See 01-04-CHECKPOINT.md and `.phase1/temp/01-04-strict-build3.log` for precise locations and continuation instructions. Existing source and lock changes remain reversible and uncommitted pending the continuation build.

## 01-18 aggregate pins selection

Five preexisting required edge identities remain `implementation: pending`, with declared owner `01-02`: `edge.rule-01.adjacency`, `edge.rule-01.empty`, `edge.rule-01.ordering`, `edge.rule-01.idempotency`, and `edge.rule-01.concurrency`. Existing related runner controls do not publish results for these distinct identities. They therefore remain blocked rather than borrowing another case's result. Plan 01-14's complete register/proof binding is the concrete closure owner; it must add distinct actual C# cases or explicitly bind independently executed parameterized rows before full phase acceptance. This is outside plan 01-18's compiler/configuration work. The literal pins group remains nonzero solely for these pending obligations after all 01-18 cases pass.
