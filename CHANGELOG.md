# Changelog

All notable changes to this project are documented here.

## [0.1.0] - 2026-08-27

### Added

- Initial scaffold of the `com.mochoindiestudio.node-dialog-system` package ("MIS Dialog System"): `package.json`, `README.md`, `Runtime`/`Editor` asmdefs.
- Data layer: `DialogCharacter`, `DialogGraphNode`, `CharacterDialogNode`, `DialogNode`, `DialogResponse`, `DialogEventTrigger`, `DialogTree` (create-asset menus for Character and Dialog Tree).
- Node editor: `DialogGraphEditorWindow` + `DialogGraphView` (GraphView-based), opens on double-clicking a `DialogTree` asset, with pan/zoom/box-select, add/connect dialog nodes, and add/remove responses.
- Runtime engine: `DialogRunner` exposing current character/text/responses and `Start`/`SelectResponse`/`End` with `event Action` hooks, including `OnResponseEvent` for game-logic integration.
