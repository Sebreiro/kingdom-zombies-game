---
phase: "03"
slug: "four-player-lan-foundation"
status: verified
threats_open: 0
asvs_level: 1
register_authored_at_plan_time: true
created: "2026-11-16"
---

# Phase 03 — Security

The L1 audit checked the sixteen unique predeclared threats against production controls, the completed summaries, independent review and current source-bound passing evidence. All four plans contain authored threat models; their shared T-03-SC row is one dependency threat. No new threat scope or accepted-risk shortcut was introduced. The secure-phase L1 short-circuit applies after every predeclared mitigation was found.

## Trust boundaries

| Boundary | Crossing and control |
|---|---|
| Native packet to host authority | Transient sender, current provider/peer generation and binding identify the ready actor before intent/sequence mutation. |
| Wire state to guest publication | Bounded framing/JSON, exact compatibility, complete detached snapshot, current session, revision and elapsed counters precede installation. |
| Discovery/profile input to services | Bounded sender-derived endpoint, opaque ordinal identity, owned canonical files and atomic exclusive writes. |
| Engine callbacks to state | Exact guarded native methods, context-owned input and retirement before late callbacks. |
| Driver/observer/processes to acceptance | Separate mutation and observation, actual provenance, conservative monotonic timing, exact nonce/hash membership and owned cleanup. |

## Threat register

| ID | Category | Severity | Disposition | Mitigation and actual proof | Status |
|---|---|---|---|---|---|
| T-03-01 | Spoofing | high | mitigate | `FoundationSession.TryAccept`, coordinator sender/binding/readiness checks; `core.authority`, `core.ordering`. | closed |
| T-03-02 | Tampering | high | mitigate | `FoundationProtocol` bounded depth/shape/value validation and `FoundationWireFrame` exact bounded decompression; `core.protocol`. | closed |
| T-03-03 | Denial of service | medium | mitigate | ENet capacity, 2–32768-byte application bound, encoded bound and 64-packet drain; `network.idle-poll`, protocol controls. | closed |
| T-03-04 | Elevation of privilege | high | mitigate | Exact 45 native tuples, eight public additions and adjacent rejection controls; `core.permissions`, inherited engine/graph rules. | closed |
| T-03-05 | Spoofing | high | mitigate | Pending-inclusive capacity and sender/readiness/owner/sequence validation; `core.admission`, `core.ordering`, real admission/race scenarios. | closed |
| T-03-06 | Tampering | high | mitigate | Session/revision/generation/actor-time fences and atomic publication; `core.protocol`, lifecycle and individual provider substitution. | closed |
| T-03-07 | Denial of service | high | mitigate | Bounded receive/admission work, 30000 ms join and 10000 ms response expiry, exact cleanup; core and actual lifecycle/race cases. | closed |
| T-03-08 | Spoofing | high | mitigate | Discovery endpoint from received sender, exact session/profile identity, join-time revalidation and active duplicate refusal; discovery/profile/browser/admission cases. | closed |
| T-03-09 | Tampering | high | mitigate | `LocalProfileStorage` canonical root, exclusive writer and same-directory atomic publication; actual persistence/corruption/concurrency controls. | closed |
| T-03-10 | Denial of service | medium | mitigate | Bounded datagrams/labels/drain and context-owned input; overflow/Unicode/focus/browser and input cases. | closed |
| T-03-11 | Elevation of privilege | high | mitigate | Discovery, identity and connection individually replaced through production composition; exact permissions remain enforced. | closed |
| T-03-12 | Tampering | high | mitigate | Read-only draw observer separated from physical/targeted/synthetic input; 48 actual conservative timing rows and over-limit controls. | closed |
| T-03-13 | Repudiation | high | mitigate | Independent exact case/role/image sets, immutable reports and nonce/hash/revision validation; runner integrity and 319 bound visual judgments. | closed |
| T-03-14 | Denial of service | high | mitigate | Bounded real nonzero/crash/hung child controls, owned descendant cleanup and shared lock; process proofs plus all current terminal/helper/lock records. | closed |
| T-03-15 | Tampering | high | mitigate | Canonical owned profile/evidence roots, exclusive publication, bounded validated request consumption and path substitution negatives. | closed |
| T-03-SC | Tampering | high | mitigate | Existing dependency pins and built-in APIs only; no package, project or reference graph addition. The proof project adds exact source links only. | closed |

## Evidence and limits

Source/build: `c69442148c975de974ebbb1d8ef409380940ecb0`. The immutable full report `841e10b1-f825-4527-865b-4703382f51eb` passes 481/481; `03-EVIDENCE.json` binds the actual current and historical reports. All eighteen independent code-review findings are resolved. The exact completion validator passed from metadata closure `fb158d572653f7541394ce7171957f8a784c5274` without rewriting the original execution.

All 14 native and 17 headless terminal records close normally or by declared loss; zero emergency kills, eleven helpers ended and no remaining desktop lock. All 688 retained console logs have no matching errors or warnings. The audit does not claim hosted/Linux, physical mixed-computer LAN, campaign, hardware or endurance execution.

## Accepted risks log

No accepted risks were needed to close the predeclared register. Later release procedures retain their own environment evidence; absence of those future proofs is not relabeled as current execution.

## Security audit trail

| Date | Unique threats | Closed | Open | Method |
|---|---:|---:|---:|---|
| 2026-11-16 | 16 | 16 | 0 | L1 production-control and actual-proof reconciliation against plan-authored register. |

## Sign-off

- [x] All predeclared threats retain a disposition and concrete mitigation.
- [x] No unresolved or accepted-risk shortcut hides a blocking threat.
- [x] `threats_open: 0`; `status: verified`.

Approval: verified against the current phase evidence on 2026-11-16.
