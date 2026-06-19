# TODO

## Import Fidelity

- [ ] Preserve cluster identity on import.
  Import should keep the original cluster name, cluster linkage, and stable cluster identity instead of normalizing clusters into ambiguous fallback names.

- [ ] Preserve sector linkage on import.
  Imported sectors should keep the original relationship graph and not rely on sector-name uniqueness for later gate pairing or export.

- [ ] Preserve reciprocal gate linkage by path.
  Import/export logic should consistently use source/destination paths as the canonical identity for matching gates, not display names.

- [x] Stop renaming imported sectors just to force uniqueness.
  `EnsureUniqueSectorNames` currently changes imported names and can break round-trip fidelity. Replace this with a non-destructive display strategy.

- [x] Stop reordering imported custom sectors by name during ID assignment.
  Imported order should remain stable so IDs and gate paths do not drift between import and export.

## Metadata Import

- [ ] Import sector ownership from `libraries/mapdefaults.xml`.
  Custom sectors should restore `Owner` correctly.

- [x] Import sector resource areas from `libraries/mapdefaults.xml`.
  Custom sectors should restore `ResourceAreas` instead of defaulting to empty lists.

- [x] Import sector economy/security/sunlight values from `libraries/mapdefaults.xml`.
  Custom sectors should restore `Sunlight`, `Economy`, and `Security` instead of falling back to defaults.

- [x] Import faction logic and anomaly flags from `libraries/mapdefaults.xml`.
  Custom sectors should restore `DisableFactionLogic` and `AllowRandomAnomalies` accurately.

- [ ] Verify station ownership import.
  Confirm imported station objects keep correct owners after mod import.

## Placement Fidelity

- [ ] Preserve imported cluster positions without unnecessary collision rewrites.
  `ResolveCustomClusterPositionCollisions` should not rewrite valid imported placements unless absolutely required.

- [ ] Preserve sector offsets and placement semantics exactly.
  Import should distinguish between real custom offsets, inferred placement, and fallback placement.

- [ ] Preserve zone positions exactly.
  Imported zone positions should round-trip without drift.

- [ ] Verify imported gate rotations and positions round-trip correctly.
  Imported gate `Yaw`, `Pitch`, `Roll`, and local position should survive import/export unchanged.

## Visual / Language Fidelity

- [x] Import skybox/background mapping faithfully.
  Verify `BackgroundVisualMapping` and related cluster visual data are restored for imported clusters.

- [x] Verify soundtrack import fidelity.
  Imported clusters should keep intended soundtrack mapping where present.

- [ ] Improve translation import beyond display names only.
  Current import only uses translation files for display-name recovery. Review whether descriptions, page-title fallback, and multi-language behavior need to be preserved more accurately.

- [x] Make translation loading deterministic.
  Translation resolution should not depend on filesystem enumeration order.

- [ ] Verify non-English translation handling.
  Import should behave predictably when multiple language files exist or when the primary display text is not in the first loaded file.

## Tests

- [ ] Add regression tests for duplicate cluster and sector names during import/export.

- [ ] Add regression tests for owner/resource import from `mapdefaults.xml`.

- [ ] Add regression tests for background/skybox import.

- [ ] Add regression tests for imported placement fidelity.

- [ ] Add regression tests for translation resolution and language selection.

- [ ] Add a round-trip fixture test.
  Import a known mod, export it again, and compare key structural and metadata outputs.

## Import Audit

- [x] Build an importer audit harness.
  Add a reproducible tool or test workflow that imports a mod, captures the in-memory result, and compares it against source XML/JSON expectations.

- [ ] Audit cluster identity fidelity.
  Detect mismatches in cluster names, IDs, positions, DLC attribution, and link topology.

- [ ] Audit sector identity fidelity.
  Detect mismatches in sector names, counts, ordering, base/custom classification, and linkage preservation.

- [ ] Audit zone and gate fidelity.
  Detect dropped zones, gate count changes, position drift, rotation drift, or broken reciprocal links.

- [ ] Audit metadata fidelity.
  Detect loss of owner, resources, sunlight, economy, security, faction logic, anomaly flags, soundtrack, and background visuals.

- [ ] Audit translation fidelity.
  Detect where imported names/descriptions differ from source translation-backed values.

- [ ] Run the audit against known fixture mods.
  At minimum cover `export-test`, `uncharted_skies_mod_pack`, and one smaller vanilla-extension-style fixture.

- [ ] Convert every confirmed audit mismatch into a targeted regression test.

## Known Fixed Bugs

- [x] Fix export crash when reciprocal gate lookup encounters duplicate sector names.

- [x] Fix sector XML generation so new zone connections are inserted in sequence-safe order for vanilla sectors.
