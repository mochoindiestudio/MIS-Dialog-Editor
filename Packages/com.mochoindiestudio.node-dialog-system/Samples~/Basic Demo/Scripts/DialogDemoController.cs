using System.Collections.Generic;
using MochoIndieStudio.DialogSystem;
using UnityEngine;
using UnityEngine.UI;

namespace MochoIndieStudio.DialogSystem.Samples
{
    /// <summary>
    /// Minimal example wiring a <see cref="DialogRunner"/> to plain uGUI Text/Image/Button widgets.
    /// Shows the intended integration pattern: the runner is pure data/events, this script is the
    /// "whatever UI you want" half every consuming game writes for itself.
    /// </summary>
    public sealed class DialogDemoController : MonoBehaviour
    {
        [SerializeField]
        private DialogTree dialogTree;

        [SerializeField]
        private Image portraitImage;

        [SerializeField]
        private Text characterNameText;

        [SerializeField]
        private Text mainText;

        [SerializeField]
        private Transform responseButtonContainer;

        [SerializeField]
        private Button responseButtonPrefab;

        private readonly List<Button> spawnedButtons = new List<Button>();
        private DialogRunner runner;

        private void Start()
        {
            runner = new DialogRunner();
            runner.OnDialogStarted += Refresh;
            runner.OnDialogAdvanced += Refresh;
            runner.OnDialogEnded += HandleDialogEnded;
            runner.OnResponseEvent += HandleResponseEvent;
            runner.Start(dialogTree);
        }

        private void Refresh()
        {
            characterNameText.text = runner.CurrentCharacter != null ? runner.CurrentCharacter.DisplayName : string.Empty;
            portraitImage.sprite = runner.CurrentCharacter != null ? runner.CurrentCharacter.Portrait : null;
            mainText.text = runner.CurrentText;

            ClearResponseButtons();

            var responses = runner.CurrentResponses;
            for (int i = 0; i < responses.Count; i++)
            {
                int responseIndex = i;
                var button = Instantiate(responseButtonPrefab, responseButtonContainer);
                button.gameObject.SetActive(true);
                var buttonLabel = button.GetComponentInChildren<Text>();
                if (buttonLabel != null)
                {
                    buttonLabel.text = responses[i].ResponseText;
                }

                button.onClick.AddListener(() => runner.SelectResponse(responseIndex));
                spawnedButtons.Add(button);
            }
        }

        private void ClearResponseButtons()
        {
            for (int i = 0; i < spawnedButtons.Count; i++)
            {
                Destroy(spawnedButtons[i].gameObject);
            }

            spawnedButtons.Clear();
        }

        private void HandleDialogEnded()
        {
            characterNameText.text = string.Empty;
            mainText.text = "(conversation ended)";
            ClearResponseButtons();
        }

        private static void HandleResponseEvent(DialogEventTrigger trigger)
        {
            Debug.Log($"[DialogDemo] Event triggered: {trigger.EventId} ({trigger.Payload})");
        }
    }
}
