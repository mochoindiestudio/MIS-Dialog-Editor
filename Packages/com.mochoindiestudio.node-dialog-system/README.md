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

## Runtime

```csharp
var runner = new DialogRunner();
runner.OnResponseEvent += trigger => { /* dispatch trigger.EventId / trigger.Payload */ };
runner.Start(myDialogTree);

// runner.CurrentCharacter, runner.CurrentText, runner.CurrentResponses drive your own UI
runner.SelectResponse(0);
```
