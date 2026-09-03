# Changelog

All notable changes to this project are documented here.

## [0.6.0] - 2026-09-03

### Added

- **`DialogRunner.PublishEventsToSignalBus`** (`bool`, default `false`). When enabled, each
  `DialogEventTrigger` on a selected response is also reported to the shared **MIS Signals** bus as
  `MisSignals.Report(EventId, Payload)` — in addition to `OnResponseEvent` — so a MIS Quest or
  Inventory system can react to a dialog choice with no game-side glue. Left off, the runtime behaves
  exactly as before.

### Changed

- `package.json` now declares a dependency on `com.mochoindiestudio.signals` (>= 0.1.0); the runtime
  asmdef references `MochoIndieStudio.Signals`. It is consumed from
  `https://github.com/mochoindiestudio/MIS-Signals.git#v0.1.0`. UPM does not resolve a git package's
  dependencies — a game installing this package must add the signals git URL to its own manifest too
  (see the README).

## [0.5.0] - 2026-08-30

### Added

- Graph editor nodes are resizable (drag any edge/corner). The chosen width is saved per node in `DialogGraphNode.EditorWidth` and restored on reopen; height stays auto-fit to content.

### Changed

- Response rows in the graph editor no longer show the "Event" field -- event triggers are edited in the asset's Inspector. Removes the half-shown control (the payload was never editable there anyway).
- Response delete buttons use the `icon_delete` sprite (now under `Editor/Icons/`) instead of a text "X".

## [0.4.4] - 2026-08-30

### Fixed

- Committed `.meta` files for the package's `CHANGELOG.md` and `LICENSE.md`. Without them, consuming projects that install the package from Git (an immutable folder, where Unity can't generate metas) logged "has no meta file... The asset will be ignored."

## [0.4.3] - 2026-08-30

### Changed

- Moved `CHANGELOG.md` from the repo root into the package root so Unity's Package Manager shows it as the "Changelog" tab for Git-installed consumers.

### Added

- `LICENSE.md` (MIT) at the package root, plus `"license": "MIT"` in `package.json`, so the Package Manager "Licenses" link works.

## [0.4.2] - 2026-08-30

### Changed

- Each response's output port now sits at the end of that response's own row in the graph editor, lining its connector up with the response instead of stacking in the node's top-right output area. Dialog nodes are no longer collapsible (collapsing would hide the in-body ports).
- Rewrote the README's Runtime section: a complete `DialogRunner` MonoBehaviour example (event subscribe/unsubscribe, rendering, per-response buttons, event handling) and a full API-surface table.

## [0.4.1] - 2026-08-30

### Changed

- Graph editor "Main Text" and response-text fields are now word-wrapping, multi-line text areas (~3 lines tall) instead of single-line fields. The response "Event" id field stays single-line.

## [0.4.0] - 2026-08-30

### Added

- Graph editor now centers its viewport on the canvas origin (0,0) when opened.
- 36x36px grid background in the graph editor (`Editor/DialogGraphView.uss`), plus a "Snap to Grid" toggle in a new window toolbar that quantizes node positions to the grid while dragging.

### Changed

- New dialog nodes spawn just to the right of the current right-most node instead of at a mouse-derived position, so they no longer appear at random offsets or off-screen.
- Removed the "Basic Demo" sample (`Samples~/Basic Demo/`, `samples` entry in `package.json`, and its README section).

## [0.3.0] - 2026-08-27

### Added

- Portrait preview: `CharacterNodeView` shows the assigned character's portrait live in the graph editor, and a custom `DialogCharacterEditor` inspector draws a larger preview below the default fields.
- Custom Project window icons for `DialogCharacter` and `DialogTree` assets, and matching header icons on `CharacterNodeView`/`DialogNodeView` in the graph editor (`Editor/Icons/`).

## [0.2.0] - 2026-08-27

### Added

- "Basic Demo" sample (`Samples~/Basic Demo/`, declared in `package.json`): a Skyrim-flavored example dialog (a Whiterun guard, branching into an "arrow in the knee" backstory or a question about dragons, each ending response firing a `DialogEventTrigger`), a plain-uGUI `DialogDemoController` wiring `DialogRunner` to `Text`/`Image`/`Button` widgets, a `ResponseButton` prefab, and a ready-to-play `DialogDemo.unity` scene (Camera + Canvas + an `InputSystemUIInputModule`-based `EventSystem`).

## [0.1.0] - 2026-08-27

### Added

- Initial scaffold of the `com.mochoindiestudio.node-dialog-system` package ("MIS Dialog System"): `package.json`, `README.md`, `Runtime`/`Editor` asmdefs.
- Data layer: `DialogCharacter`, `DialogGraphNode`, `CharacterDialogNode`, `DialogNode`, `DialogResponse`, `DialogEventTrigger`, `DialogTree` (create-asset menus for Character and Dialog Tree).
- Node editor: `DialogGraphEditorWindow` + `DialogGraphView` (GraphView-based), opens on double-clicking a `DialogTree` asset, with pan/zoom/box-select, add/connect dialog nodes, and add/remove responses.
- Runtime engine: `DialogRunner` exposing current character/text/responses and `Start`/`SelectResponse`/`End` with `event Action` hooks, including `OnResponseEvent` for game-logic integration.
