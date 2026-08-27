# MIS Dialog System

A node-based dialog editor and runtime engine for Unity.

Every dialog asset (`DialogTree`) is rooted at a `CharacterDialogNode`, so a character's info
(display name, portrait) is always reachable from any node in the tree, no matter how deep. Dialog
nodes carry a main text and a collection of responses; each response can lead to another dialog node
and/or fire a `DialogEventTrigger` (an `EventId`/`Payload` pair) for your game logic to listen for.

The package is UI-agnostic: it exposes data and `event Action` hooks (`DialogRunner`) only. It does
not render any dialog UI itself -- wire it up to whatever UI system your project already uses.

## Authoring

- `Create > MIS Dialog System > Character` -- a reusable `DialogCharacter` asset (display name +
  portrait sprite), shareable across multiple dialog trees.
- `Create > MIS Dialog System > Dialog Tree` -- a `DialogTree` asset; double-click it to open the
  node editor. Every new tree starts with its `CharacterDialogNode` root already in place.
- `DialogCharacter` and `DialogTree` assets each show a custom Project window icon, and the graph
  editor shows the matching icon in each node's header (`Editor/Icons/`).

## Runtime

```csharp
var runner = new DialogRunner();
runner.OnResponseEvent += trigger => { /* dispatch trigger.EventId / trigger.Payload */ };
runner.Start(myDialogTree);

// runner.CurrentCharacter, runner.CurrentText, runner.CurrentResponses drive your own UI
runner.SelectResponse(0);
```

## Sample: Basic Demo

Import via Package Manager > MIS Dialog System > Samples > **Basic Demo**. It contains:

- `Data/WhiterunGuard.asset` + `Data/GuardDialog.asset` -- a small Skyrim-flavored example dialog (a
  Whiterun guard, branching into an "arrow in the knee" backstory or a question about dragons, each
  ending response firing a `DialogEventTrigger`). `GuardPortrait.png` is a placeholder solid-color
  sprite, not real game art -- swap in your own.
- `Scripts/DialogDemoController.cs` -- the example UI wiring described above, built on plain uGUI
  (`Text`/`Image`/`Button`).
- `Prefabs/ResponseButton.prefab` -- the response button template the controller instantiates per
  response.
- `Scenes/DialogDemo.unity` -- a Camera, Canvas, and an `EventSystem` using
  `InputSystemUIInputModule` (this repo's Active Input Handling is set to the New Input System only;
  if your own project uses the old Input Manager only, swap this for `StandaloneInputModule` after
  importing). Open the scene and press Play to try the conversation; selecting the "arrow in the
  knee" or dragon responses logs their `DialogEventTrigger.EventId` to the Console.
