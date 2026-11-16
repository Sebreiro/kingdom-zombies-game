# Phase 03 — UI Review

**Audited:** 2026-11-16
**Baseline:** Approved native Godot design contract, `03-UI-SPEC.md` (frontmatter `status: approved`), with the current local-testing policy.
**Screenshots:** Retained native captures available; 17 current actual PNGs opened for this audit and all 319 current image bindings independently SHA-256 checked. No new captures.
**Interaction captures:** off (`interaction_capture: false`). Existing post-interaction PNGs/observations were inspected; no interaction was executed by this audit.
**Scope:** Windows 1280×720 local development, no VM. No Linux, 1920×1080, physical mixed-computer LAN, release, or new execution claim.
**Disposition:** WARNING — 8 current presentation findings, 0 BLOCKER findings. The retained behavioral pass and repaired visual defects remain valid evidence within their stated scope; this stricter contract audit identifies additional presentation drift.

---

## Pillar Scores

| Pillar | Score | Key Finding |
|--------|-------|-------------|
| 1. Copywriting | 4/4 | Contracted actions, recovery reasons, singular/plural counts and Unicode text are present and readable. |
| 2. Visuals | 3/4 | Identity markers and hierarchy are readable, but remote bodies lack the contracted dark inset detail. |
| 3. Color | 3/4 | Focus and several native control/text colors fall back to a different theme rather than the specified palette. |
| 4. Typography | 3/4 | Roster uses the 24px default instead of 16px body text; browser heading misses the bold treatment. |
| 5. Spacing | 3/4 | Actual roster lines are 34px rather than 24px; profile-to-actions gap is 32px rather than 16px. |
| 6. Experience Design | 3/4 | Core flows pass; long-text disclosure and clearing feedback on address edit have code-derived gaps. |

**Overall: 19/24**

Scores are independent pillar assessments. The roster font/height defect affects both Typography and Spacing; it is one distinct finding in the count below. A 3 denotes a minor contract deviation, not a failed LAN behavior scenario.

---

## Top 3 Priority Fixes

1. **UI-01 — Apply the shared focus treatment to fields and session rows.** Current focused fields/rows use neutral outlines rather than the specified 4px gold focus. Reuse the existing `Focus` resource for both LineEdits and ensure dynamic session rows retain that outer outline independently of their selected inset.
2. **UI-02 — Apply 16px font size to the roster Labels themselves.** The current five visible lines measure 34px each. Set the effective child Label typography to 16px and retain 24px line boxes, restoring the compact HUD without shrinking participant identities.
3. **UI-07 — Make shortened summary/roster text inspectable.** Preserve the summary's fixed context/ordinal while shortening its name column, and provide the contracted full-text disclosure. Roster tooltips currently sit on mouse-ignoring Labels; provide a usable inspection route without taking gameplay input.

All three are WARNING fixes. No destructive confirmation, production art, campaign UI, new lobby, or unrelated feature is recommended.

---

## Evidence Identity and Method

The audit used the `gsd-ui-review` six-pillar method, adapted to the approved native scene. Project instructions, local-testing policy, all four Phase 03 PLANs and SUMMARYs, CONTEXT, UI-SPEC, VISUAL-REVIEW and EVIDENCE were loaded. The configured `gsd-ui-auditor --skills-only` query returned no additional skills.

- Metadata HEAD inspected: `fb158d572653f7541394ce7171957f8a784c5274`.
- Reviewed source/build: `c69442148c975de974ebbb1d8ef409380940ecb0`. `git diff` from that revision through current `src` was empty.
- Actual fresh full invocation: `841e10b1-f825-4527-865b-4703382f51eb`, execution revision `6ea600953be031bf2b467a50d35090a94e44f802`.
- Full report: `.phase3/runs/win32-x64/841e10b1-f825-4527-865b-4703382f51eb/summary.json`, SHA-256 `b7bdf55d74034a5c50527f0e8391e7a7be01d1e46867dd103b9d381e6b2e6bae`; actual hash matches `03-EVIDENCE.json`. It records the 481/481 final selection in 3,363,558 ms (56.06 minutes), not a rerun performed here.
- Visual report: `03-VISUAL-REVIEW.md`, SHA-256 `71771e0554e266af0b9093a6eb471a358bbd4bd6bf68de591cc02c21dde7d0c7`; actual hash matches the index. All 319 tabulated current PNG hashes matched their actual files. Its comprehensive review accounts for 255 distinct contents: 221 directly inspected current contents plus 34 exact-byte prior inspected contents.
- This audit independently opened the 17 representative images listed below. It does not claim a second comprehensive direct viewing of all 255 contents.
- Native source/scene and retained completed-draw JSON were read; palette-region pixel counting was read-only corroboration after actual image viewing, not a substitute for visual inspection.
- Web development-server probing and Playwright capture are **not applicable** to this Godot executable. No game/browser, build or tests were launched. The capture-directory creation gate is not applicable because this report-only audit created no captures. `git check-ignore` confirmed the inspected `.phase3/runs` and inherited `.phase2/runs` PNGs are already ignored.

For image references below, **B/** means `.phase3/runs/win32-x64/841e10b1-f825-4527-865b-4703382f51eb/G2/browser/`; **G3/** means the same invocation's `G3/`; **P2/** means `.phase2/runs/win32-x64/1e75e6b9-06b7-4e78-bf71-99ad4f02cc06/session/`. Every cited PNG is an exact current binding in `03-EVIDENCE.json` and `03-VISUAL-REVIEW.md`.

## Detailed Findings

### Pillar 1: Copywriting (4/4)

**PASS — no implementation copy defect found against the contract.** `Foundation.tscn:181–226` retains Start, Host session, Join LAN and Quit; `:303–355` supplies the address example and distinct Join address, Join session, Cancel join and Back actions. `FoundationCopy.cs:10–33` maps full, duplicate-profile, incompatible, timeout and host-loss/end reasons to the prescribed English next steps. `FoundationBrowser.cs:42–45` implements empty/vanished and one/many-session wording; `FoundationHud.cs:24–26` implements player/players.

Actual B/draw-empty-4.png, B/draw-invalid-malformed-10.png, B/draw-timeout-13.png, G3/duplicate-identity/draw-duplicate-1.png and G3/guest1/draw-host-end-scene-19.png / draw-host-loss-scene-21.png show complete, readable recovery text and actions. B/draw-unicode-15.png confirms accented, CJK and emoji display with an editable caret. `LocalProfileService.cs:37–41,91–93` retains explicit blank-name and storage-error recovery. Start is an explicitly locked short label, not a generic-copy defect.

### Pillar 2: Visuals (3/4)

**UI-06 — WARNING: remote actor interior detail is missing.** The color contract (`03-UI-SPEC.md:71`) specifies a text-colored remote body/triangle with dark inset detailing and a light outline. `FoundationWorld.tscn:37–43` has only a light outline and body; `FoundationView.cs:49–52` recolors the remote body to the same neutral as its outline, with no remote inset primitive. Actual G3/guest2/draw-walkers-scene-2.png and B/draw-manual-ready-7.png show solid neutral remote rectangles. **Fix:** add/reuse the small dominant-color inset for remote actors while retaining the light outline, P-number, facing triangle and local gold treatment. This is a placeholder styling correction, not a request for production artwork.

**PASS evidence:** B/draw-profiles-scene-17.png has a clear centered heading and Start focal point, with all four actions visible. Current four-player overlap keeps P1–P4 labels separate (`FoundationPresentation.cs:24–39`), while local You remains readable. P2/capture-158.png and capture-170.png show the complete repaired P1 · You backing at both viewport edges. There are no icon-only action buttons requiring invented labels/tooltips; the direction triangle is a world-facing indicator.

### Pillar 3: Color (3/4)

**UI-01 — WARNING: active focus treatment is inconsistent with the approved accent contract.** `Foundation.tscn:13–23` defines the correct gold 4px outer focus, but `:64–72` assigns styles only to Button. The LineEdits (`:155–162,303–309`) have no equivalent focus override. B/draw-invalid-malformed-10.png and B/draw-unicode-15.png show a thin neutral field outline instead of gold; the malformed checkpoint's JSON explicitly records `Browser/Address.focused: true`. B/draw-focused-detail-6.png also shows a white outer border on Session1, with its JSON explicitly recording that row focused. The selected inset and keyboard focus must remain independently visible in the specified styles (`03-UI-SPEC.md:29,45,67–73`). **Fix:** give fields and dynamic rows the shared gold outer focus resource; preserve the selected row's 4px inset when focus/hover changes. Normal button focus works, as P2/capture-216.png and B/draw-timeout-13.png show.

**UI-05 — WARNING: native fallback colors escape the authored palette.** `Foundation.tscn:64–72` sets Button text color but does not set the matching Label/LineEdit theme colors and field surface. Actual browser-title and roster regions contain solid `#FFFFFF` text rather than `#F4F2E8` (1,076 and 2,722 solid white pixels respectively in B/draw-browser-scene-8.png and G3/guest2/draw-walkers-scene-2.png). The focused address region in B/draw-invalid-malformed-10.png uses native `#191E21`, `#CCCCCC` and `#DFDFDF` surfaces/boundary/text. **Fix:** author the native Label and LineEdit theme roles using the approved text, dominant surface and focus resources; avoid depending on unspecified fallback colors.

**PASS evidence:** The authored accent appears in five permitted semantic groups: Start border, focused controls, selected row inset, local body/facing and You marker. `FoundationMenu.cs:432–438` and `FoundationView.cs:49–56` introduce no extra decorative accent role. Sparse world sky dominance is allowed explicitly; the 60/30/10 contract is emphasis rather than an exact image-area quota. Approved-palette contrast calculations are 13.98:1 text/sky, 9.24:1 text/secondary, 7.04:1 text/hover, 9.57:1 gold/sky and 6.32:1 gold/secondary. No low-contrast blocker was observed. Native authored color literals are expected here; they are not a web hardcoded-color defect by themselves.

### Pillar 4: Typography (3/4)

**UI-02 — WARNING: effective roster type is 24px, producing 34px lines.** The 16px override is placed on the VBoxContainer (`Foundation.tscn:367–375`), while its Header/Player1–4/Waiting Labels (`:377–405`) do not carry that effective override. The actual G3/guest2/draw-walkers-scene-2.png shows the larger type; its matching JSON measures Header and each player row at height **34**. The contract calls for body/status at 16px and four 24px participant lines (`03-UI-SPEC.md:53,121`). **Fix:** apply the 16px role to the Label nodes or a child Label theme, retaining the 24px line boxes and full P-number/You prefix. The same typography correction resolves the associated spacing drift below.

**UI-03 — WARNING: browser heading misses the shared bold role.** `FoundationRoot.cs:24–29` creates/applies the bundled FontVariation only to Menu/Title. Browser/Title (`Foundation.tscn:249–255`) has size 32 but no bold font assignment; B/draw-browser-scene-8.png shows the regular-weight heading. **Fix:** apply the same existing heading FontVariation to Browser/Title. The contract's heading role is 32px/bold (`03-UI-SPEC.md:55`).

**Distribution:** Only the declared 16/24/32 sizes are authored: scene declarations contain twelve 16px values, one 24px default and two 32px headings; dynamic row Labels explicitly choose 16 or 24 (`FoundationMenu.cs:457–462,494–505`). The implementation uses regular fallback and one local bold variation, with no external font. The issue is role application, not an excessive number of sizes/weights.

### Pillar 5: Spacing (3/4)

**UI-04 — WARNING: profile-to-actions gap is doubled.** Name/NewProfile end at menu-local y=248 (`Foundation.tscn:155–170`), but Start starts at y=280 (`:181–185`): a 32px gap. The intermediate ActionGap already reserves y=248–264 (`:174–179`), followed by a second 16px blank interval. The contract requires a 16px profile-to-action gap (`03-UI-SPEC.md:87`). B/draw-profiles-scene-17.png shows the resulting separation. **Fix:** remove the duplicated 16px interval and shift the action stack/feedback consistently, retaining 48px actions, 16px inter-action gaps and the feedback reserve. This is minor; Quit is fully visible.

**UI-02 cross-reference:** The retained draw JSON measures a five-line roster of 170px rather than five 24px line boxes (120px). The original container's y=24–168 allocation expands to accommodate its child minimums. Correct the child typography rather than masking overflow or reducing names. This is the same distinct defect as UI-02, not another recommendation.

**PASS evidence:** Menu width 640 and browser width 1120 remain centered inside the 24px vertical safe area (`Foundation.tscn:104–110,239–246`). Browser list is 192px tall, each row 64px, row horizontal text padding is 16px, and the address/actions are 48px high (`:282–309`; `FoundationMenu.cs:450–462,499–501`). The current overflow browser shows three rows and a vertical scrollbar while the footer remains visible. Native offsets and inherited world geometry are interpreted against their role rather than mechanically treated as arbitrary CSS spacing.

### Pillar 6: Experience Design (3/4)

**UI-07 — WARNING, code-derived interaction gap: long-text disclosure is incomplete outside session rows.** Browser/Summary ellipsizes the whole Label (`Foundation.tscn:257–264`); `FoundationMenu.cs:113,326–330` assigns one combined string, so a long name can also remove the trailing Profile ordinal. There is no summary tooltip or focus/detail route. The contract permits shortening only the summary's display name and requires full detail (`03-UI-SPEC.md:97`). Roster names visibly shorten in B/draw-manual-ready-7.png; `FoundationHud.cs:40–41` sets TooltipText, but Roster and every child Label are mouse-ignoring (`Foundation.tscn:367–405`) and have no inspection focus route. The required full hover/focus inspection is therefore not supplied by the authored interaction code (`03-UI-SPEC.md:121`). **Fix:** preserve fixed summary context/ordinal independently of name ellipsis and add accessible full-text disclosure for both surfaces without consuming movement keys. This audit did not execute a hover or long-summary interaction; the missing routes are established from source, while actual roster truncation is captured.

**UI-08 — WARNING, code-derived interaction gap: an address edit does not clear obsolete attempt/validation feedback.** The contract retains errors until another attempt, **edit**, or navigation clears them (`03-UI-SPEC.md:113`). `FoundationMenu.Attach`/`Detach` (`:44–89`) subscribe to address submission/focus, not TextChanged (zero TextChanged hooks in the production menu). `HideDetail` (`:488–492`) hides only the detail surface. Recover (`:162–175`) latches attempt feedback; invalid address handling (`FoundationRoot.cs:253–258`) likewise writes feedback without an edit-clearing path. **Fix:** handle idle address edits by clearing obsolete connection/validation feedback and its attempt-error latch, preserving the field/caret and active join lock. The error display itself was viewed; its persistence after editing is source-derived, not a newly captured interaction.

**PASS evidence:** B/draw-pending-12.png shows locked editing/Refresh, focused Cancel join, visible context and no actor. Timeout, duplicate-profile, host-ended and host-loss images show the correct usable recovery actions/focus. `FoundationMenu.cs:365–380,441–443` handles pending/disabled actions; `FoundationRoot.cs:80–88,327–333` activates the actual focused enabled Button. `FoundationInput.cs:16–45`, `FoundationMenu.cs:293–295` and `FoundationRoot.cs:375–379` clear held commands at screen/focus transitions. Existing cases cover cancellation, empty/overflow lists, coherent readiness, four peers, focus loss, departure/slot reuse and rehost/rejoin. Retained physical timing remains 48 rows with maximum 197.8586ms and 16 exact stopped comparisons; still PNGs are not credited as timing proof. Direct Escape/Quit with no deletion confirmation is explicitly approved for these temporary sessions.

## Historical Repairs and Scope Limits

The historical fourth full run's two actual visual findings remain preserved in `03-EVIDENCE.json` and the historical review. They are **not current warnings** here:

- Cleared idle browser progress: B/draw-browser-scene-8.png has no stale Receiving session state message; `FoundationMenu.cs:122–125` clears it on readiness.
- Viewport-edge identity: P2/capture-158.png and capture-170.png retain complete P1 · You/backing at both world bounds; `FoundationPresentation.cs:11–15` and `FoundationView.cs:57–60` implement the repaired projection.

The comprehensive retained appearance review's PASS and fresh behavioral PASS are not silently rewritten. This document adds exact contract findings that the earlier readability-focused conclusions did not classify. No new run, other display, Linux execution or physical mixed-LAN acceptance is inferred from these artifacts.

## Recommendation Count

- BLOCKER: 0
- WARNING findings: 8 distinct (UI-01–UI-08)
- Priority fixes: 3 (UI-01, UI-02, UI-07)
- Additional minor recommendations: 5 (UI-03, UI-04, UI-05, UI-06, UI-08)

Registry check after scoring: no `components.json` found; `03-UI-SPEC.md` declares native resources and no registries. Web/shadcn registry commands are not applicable; no packages were fetched and no Registry Safety section is required.

---

## Files Audited

- `AGENTS.md`; `docs/workflow/local-testing.md`; `docs/workflow/ai-collaboration.md`.
- Phase 03 `03-01` through `03-04` PLANs and SUMMARYs; `03-CONTEXT.md`; `03-UI-SPEC.md`; `03-VISUAL-REVIEW.md`; `03-EVIDENCE.json`; `.planning/config.json`.
- `src/godot-game/project.godot`; foundation `Foundation.tscn`, `FoundationWorld.tscn`, `FoundationRoot.cs`, `FoundationMenu.cs`, `FoundationBrowser.cs`, `FoundationCopy.cs`, `FoundationDisplay.cs`, `FoundationHud.cs`, `FoundationView.cs`, `FoundationPresentation.cs`, `FoundationInput.cs`, `FoundationActorNode.cs`, `FoundationServices.cs`, `LocalProfileService.cs`, `LanDiscoveryService.cs`, and observation fields in `FoundationDrawObserver.cs`.
- Final invocation `summary.json`; current `draw-focused-detail-6.json`, `draw-invalid-malformed-10.json`, `draw-walkers-scene-2.json`; all 319 current PNG files for hash identity only.
- **Actual PNGs opened in this audit (17):** B/draw-profiles-scene-17.png, draw-browser-scene-8.png, draw-focused-detail-6.png, draw-timeout-13.png, draw-pending-12.png, draw-invalid-malformed-10.png, draw-empty-4.png, draw-unicode-15.png, draw-manual-ready-7.png; G3/guest2/draw-walkers-scene-2.png; G3/duplicate-identity/draw-duplicate-1.png; G3/guest1/draw-host-end-scene-19.png and draw-host-loss-scene-21.png; P2/capture-158.png, capture-170.png, capture-216.png and capture-196.png.
