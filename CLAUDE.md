# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

"MIS Dialog Editor" is a Unity project (6000.5.7f1, URP) whose entire deliverable is **an installable
UPM package** — a node-based dialog editor and runtime engine, reusable across several different Mocho
Indie Studio game projects. This is non-negotiable: nothing about the runtime API, editor tooling, or
data assets should assume it lives inside one specific game's `Assets/`. The game project in this repo
(if one gets built out) exists only to exercise/demo the package, not as the product itself.

**Package**: `Packages/com.mochoindiestudio.node-dialog-system/` (display name "MIS Dialog System"),
laid out like the Lucy project's `com.mochoindiestudio.sky-system` (see
`Packages/<id>/package.json` + `Runtime/` (own `.asmdef`, `MochoIndieStudio.DialogSystem` namespace) +
`Editor/` (own `.asmdef` referencing Runtime, editor-only tooling) + `Samples~/` + `README.md`.
No Assets-side duplicate of package scripts.

**Domain requirements** (see project memory `project_dialog_editor_spec` for full detail):
- A dialog asset is a tree rooted at a **Character node**; dialog nodes (main text + responses) stem
  from it, so the character (incl. its 2D sprite portrait) is reachable from anywhere in the tree.
  Retrieving a character's info from an arbitrary node reachable from that root is a hard invariant —
  when this can't be preserved, it needs asking, not a design detour.
- Each response can lead to another dialog node, recursively, to arbitrary depth.
- Responses can trigger events for game-logic integration (quest start/end, unlocking a door, marking
  an NPC known, etc.) — the package defines the hook surface, the consuming game supplies listeners.
  Since **v0.6.0** the package also depends on `com.mochoindiestudio.signals` (the shared MIS Signals
  bus): `DialogRunner.PublishEventsToSignalBus` (default `false`) optionally forwards each response
  event to `MisSignals.Report(EventId, Payload)` so a MIS Quest/Inventory system reacts with no
  game-side glue.
- **UI-agnostic**: the package must not be tied to specific UI controls/frameworks. Its runtime API
  should read as pull/subscribe data (current node text, responses, character portrait; "select
  response N" advances state), never anything that renders UI itself.

**MVP status (v0.1.0, implemented)**: 1) node types/data layer — `DialogCharacter`, `DialogGraphNode`,
`CharacterDialogNode`, `DialogNode`, `DialogResponse`, `DialogEventTrigger`, `DialogTree` (all in
`Runtime/`); 2) node editor — `DialogGraphEditorWindow`/`DialogGraphView`/`CharacterNodeView`/
`DialogNodeView` (`Editor/`), opens on double-clicking a `DialogTree` asset, add/connect dialog nodes
via right-click "Create Node", pan/zoom/box-select from `GraphView`; 3) serialization — deliberately no
custom file format, `DialogTree` is a native `ScriptableObject` asset with `[SerializeReference]` for
the polymorphic node list; 4) runtime engine — `DialogRunner` (plain C# class, not a `MonoBehaviour`)
exposing `CurrentCharacter`/`CurrentText`/`CurrentResponses` and `Start`/`SelectResponse`/`End` with
`event Action` hooks (`OnDialogStarted`/`OnDialogAdvanced`/`OnDialogEnded`/`OnResponseEvent`). No
Samples~ demo scene yet — `Samples~/` exists but is empty.

**Known gap**: `DialogGraphEditorWindow`'s `[OnOpenAsset]` callback takes a fixed `int instanceId`
(Unity API contract), so `EditorUtility.EntityIdToObject(instanceId)` emits an unavoidable CS0618
obsolete-API warning under Unity 6's newer `EntityId` type — harmless, don't "fix" it away by
reintroducing the actually-obsolete `InstanceIDToObject`.

Work happens on the `development` branch (tracking `origin/development`); PR into `main` only when
asked. Use the `push-it` skill for the commit/version-bump/changelog/push routine — package version
lives in the package's `package.json`, not `ProjectSettings.asset`.

## Working with Unity

There is no CLI build/test workflow yet — this project is developed through the Unity Editor. The **UnityMCP** MCP server is configured (`com.coplaydev.unity-mcp` in `Packages/manifest.json`) to let Claude Code drive the Unity Editor directly: creating/editing GameObjects, scenes, scripts, materials, and running Play Mode / tests. Use the `unity-mcp-skill` skill and the `mcpforunity://` resources (not tool-name-derived URIs) when working through that integration. Since Unity Editor state (open scenes, GameObjects, selection) isn't visible from the filesystem, prefer the MCP tools/resources over guessing at scene contents.

Key packages beyond Unity defaults (`Packages/manifest.json`):
- `com.unity.render-pipelines.universal` (URP 17.5.0) — render pipeline assets live in `Assets/Settings/` (`PC_RPAsset`, `Mobile_RPAsset`, matching renderers and volume profiles).
- `com.unity.visualscripting`, `com.unity.timeline`, `com.unity.ai.navigation`, `com.unity.inputsystem` — available but unused so far.
- `Assets/Plugins/Roslyn/` — Microsoft.CodeAnalysis assemblies (Roslyn), present for C# source-generator/analyzer support.

## Notes

- The default Unity "Readme"/TutorialInfo sample assets have been removed from this project — don't re-add them.
- No test framework, linter, or CI is configured yet.
