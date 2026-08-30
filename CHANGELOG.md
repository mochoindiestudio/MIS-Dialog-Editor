# Changelog

All notable changes to this project are documented here.

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
