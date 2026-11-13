# Deferred execution issues

## 01-04 Task 2 strict-build checkpoint

The third bounded automatic correction pass leaves 151 strict-build diagnostics: 111 CS1591, 36 CA1707, two MA0051 and two MA0023. They block Task 2 completion; they are not accepted warnings, skipped tests or passed proof cases. See 01-04-CHECKPOINT.md and `.phase1/temp/01-04-strict-build3.log` for precise locations and continuation instructions. Existing source and lock changes remain reversible and uncommitted pending the continuation build.

## 01-18 aggregate pins selection

Five preexisting required edge identities remain `implementation: pending`, with declared owner `01-02`: `edge.rule-01.adjacency`, `edge.rule-01.empty`, `edge.rule-01.ordering`, `edge.rule-01.idempotency`, and `edge.rule-01.concurrency`. Existing related runner controls do not publish results for these distinct identities. They therefore remain blocked rather than borrowing another case's result. Plan 01-14's complete register/proof binding is the concrete closure owner; it must add distinct actual C# cases or explicitly bind independently executed parameterized rows before full phase acceptance. This is outside plan 01-18's compiler/configuration work. The literal pins group remains nonzero solely for these pending obligations after all 01-18 cases pass.

## 01-08 Task 1 activation checkpoint

The exact real-build SCR0006 tracer passes and is committed in b139948 and 3b45318. Three bounded inherited-policy prerequisite repair passes leave 170 distinct activation errors (IDE0055 63, IDE1006 10, IDE0300 2, IDE0305 6, SCR0009 80, SCR0006 5, SCR0007 4). The failed activation patch and exact diagnostics are retained under `.phase1/temp/08-*`; only these uncommitted candidate edits were restored. The five-project solution and tracer pass on the restored committed baseline. Task 1 expansion and Task 2 remain incomplete, with no progress advance. See `01-08-CHECKPOINT.md`; WINDOWS entry 23 tracks unfinished verification.
