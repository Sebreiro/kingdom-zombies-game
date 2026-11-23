'use strict';
// Phase 4 content and pack records keep their original repository paths as stable logical identities:
// `tools/phase4/...`, `production/phase4/...` and the game's `src/godot-game/...` resources. This one map resolves
// them to the physical folders of the current layout and back. Both directions are idempotent, so a path that is
// already physical (or already logical) passes through unchanged. Resource IDs, archive members and pinned recipe
// bytes therefore never change with the layout. A mapped folder matches itself and everything below it.
const WORKSPACE = 'src/godot-game/';
const FOLDERS = [
  ['tools/phase4', WORKSPACE + 'tools/asset-pipeline'],
  ['production/phase4', WORKSPACE + 'production/assets'],
];
const GAME = WORKSPACE + 'game/';
const GAME_ENTRY = /^(?:(?:assets|content|inspection|foundation|proofs|docs)(?:\/|$)|(?:project\.godot|export_presets\.cfg|KingdomZombies\.csproj|override\.cfg)$)/;

function within(rel, folder) {
  return rel === folder || rel.startsWith(folder + '/');
}

/** Maps a logical record path to its physical repository-relative path. */
function physical(rel) {
  for (const [logicalFolder, physicalFolder] of FOLDERS) {
    if (within(rel, logicalFolder)) return physicalFolder + rel.slice(logicalFolder.length);
  }
  if (rel.startsWith(WORKSPACE) && GAME_ENTRY.test(rel.slice(WORKSPACE.length))) {
    return GAME + rel.slice(WORKSPACE.length);
  }
  return rel;
}

/** Maps a physical repository-relative path back to its logical record path. */
function logical(rel) {
  for (const [logicalFolder, physicalFolder] of FOLDERS) {
    if (within(rel, physicalFolder)) return logicalFolder + rel.slice(physicalFolder.length);
  }
  if (rel.startsWith(GAME) && GAME_ENTRY.test(rel.slice(GAME.length))) {
    return WORKSPACE + rel.slice(GAME.length);
  }
  return rel;
}

module.exports = { physical, logical };
