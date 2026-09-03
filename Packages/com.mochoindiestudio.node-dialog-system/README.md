# MIS Dialog System

A node-based dialog editor and runtime engine for Unity.

Every dialog asset (`DialogTree`) is rooted at a `CharacterDialogNode`, so a character's info
(display name, portrait) is always reachable from any node in the tree, no matter how deep. Dialog
nodes carry a main text and a collection of responses; each response can lead to another dialog node
and/or fire a `DialogEventTrigger` (an `EventId`/`Payload` pair) for your game logic to listen for.

The package is UI-agnostic: it exposes data and `event Action` hooks (`DialogRunner`) only. It does
not render any dialog UI itself -- wire it up to whatever UI system your project already uses.

Its one dependency is **`com.mochoindiestudio.signals`** (the shared MIS Signals bus): opt in with
`DialogRunner.PublishEventsToSignalBus = true` and response events flow straight to a MIS Quest or
Inventory system with no game-side glue. Left off (the default), the package behaves exactly as
before -- pure data and events.

## Authoring

- `Create > MIS Dialog System > Character` -- a reusable `DialogCharacter` asset (display name +
  portrait sprite), shareable across multiple dialog trees.
- `Create > MIS Dialog System > Dialog Tree` -- a `DialogTree` asset; double-click it to open the
  node editor. Every new tree starts with its `CharacterDialogNode` root already in place.
- `DialogCharacter` and `DialogTree` assets each show a custom Project window icon, and the graph
  editor shows the matching icon in each node's header (`Editor/Icons/`).

## Runtime

`DialogRunner` is a plain C# class (not a `MonoBehaviour`). You create one, point it at a
`DialogTree` asset, and read its state to drive your own UI. It never touches the UI itself.

### Minimal example

```csharp
using MochoIndieStudio.DialogSystem;
using UnityEngine;

public class ExampleDialog : MonoBehaviour
{
    // Assign a DialogTree asset (Create > MIS Dialog System > Dialog Tree) in the Inspector.
    [SerializeField] private DialogTree dialogTree;

    private readonly DialogRunner runner = new DialogRunner();

    private void OnEnable()
    {
        runner.OnDialogAdvanced += Render;   // current node changed -> redraw
        runner.OnDialogEnded    += Close;    // conversation finished
        runner.OnResponseEvent  += HandleEvent;
    }

    private void OnDisable()
    {
        runner.OnDialogAdvanced -= Render;
        runner.OnDialogEnded    -= Close;
        runner.OnResponseEvent  -= HandleEvent;
    }

    // Call this to begin the conversation (e.g. from an NPC interaction).
    public void Begin()
    {
        runner.Start(dialogTree);
        if (runner.IsRunning)
        {
            Render();
        }
    }

    private void Render()
    {
        DialogCharacter speaker = runner.CurrentCharacter;   // .DisplayName, .Portrait
        string line = runner.CurrentText;

        // runner.CurrentResponses is an ordered list; build one button per entry.
        for (int i = 0; i < runner.CurrentResponses.Count; i++)
        {
            string label = runner.CurrentResponses[i].ResponseText;
            int index = i;                                   // capture for the closure
            // yourButton.onClick = () => runner.SelectResponse(index);
        }
    }

    private void HandleEvent(DialogEventTrigger trigger)
    {
        // The package never interprets these -- your game decides what the ids mean.
        switch (trigger.EventId)
        {
            case "quest_start": /* QuestSystem.Start(trigger.Payload); */ break;
            case "open_door":   /* ... */ break;
        }
    }

    // ...or skip HandleEvent entirely and let the runner publish to the shared MIS Signals bus:
    //   runner.PublishEventsToSignalBus = true;   // then a MIS Quest System objective sees it directly

    private void Close() { /* hide the dialog panel */ }
}
```

### API surface

| Member | Purpose |
| --- | --- |
| `Start(DialogTree)` | Begins a conversation at the tree's root character node. Throws on `null`. If the root points at no node, the dialog ends immediately. |
| `SelectResponse(int index)` | Picks `CurrentResponses[index]`: raises that response's events in order, then advances to its target node (or ends the dialog if it has none). Throws `ArgumentOutOfRangeException` on a bad index; no-op if nothing is running. |
| `End()` | Ends the conversation early. |
| `CurrentCharacter` | The `DialogCharacter` for the whole conversation (resolved from the root, so it stays valid at any depth). `null` when not running. |
| `CurrentText` | Main text of the current node; `""` when not running. |
| `CurrentResponses` | Ordered `IReadOnlyList<DialogResponse>` for the current node; empty when not running. |
| `IsRunning` | `true` while a conversation is in progress. |
| `OnDialogStarted` | Fires once, from inside `Start`. |
| `OnDialogAdvanced` | Fires each time the current node changes via `SelectResponse`. |
| `OnDialogEnded` | Fires when the dialog ends (a response with no target, or `End`). |
| `OnResponseEvent` | `Action<DialogEventTrigger>` -- fires once per event on the selected response, in order, before the node advances. |
| `PublishEventsToSignalBus` | `bool` (default `false`). When `true`, each response event is also reported to the shared `MisSignals` bus (`com.mochoindiestudio.signals`) as `Report(EventId, Payload)`, so a MIS Quest / Inventory system reacts without game-side glue. |

`DialogRunner` is deterministic: the same tree plus the same sequence of `SelectResponse` calls
always walks the same path.
