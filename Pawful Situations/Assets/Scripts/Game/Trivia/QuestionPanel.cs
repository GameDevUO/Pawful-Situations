using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestionPanel : MonoBehaviour
{
    [SerializeField] TMP_Text prompt;
    [SerializeField] Button[] choiceButtons;
    [SerializeField] TMP_Text[] choiceLabels;
    [SerializeField] QuestionBank bank;

    GameState gs;

    void OnEnable()
    {
        Debug.Log("[QuestionPanel] OnEnable fired.");
        TryHook();
    }

    void Start()
    {
        Debug.Log("[QuestionPanel] Start fired.");
        TryHook();
    }

    void OnDisable()
    {
        if (gs != null)
        {
            Debug.Log("[QuestionPanel] OnDisable - Unsubscribing from GameState events.");
            gs.OnQuestionChanged -= Refresh;
        }
    }

    void TryHook()
    {
        if (gs == null)
        {
            gs = GameState.I ?? FindObjectOfType<GameState>();
            if (gs == null)
            {
                Debug.LogWarning("[QuestionPanel] GameState not found yet. Will retry later.");
                return;
            }
        }

        gs.OnQuestionChanged -= Refresh;
        gs.OnQuestionChanged += Refresh;
        Debug.Log("[QuestionPanel] Hooked into GameState.OnQuestionChanged event.");

        if (gs.CurrentQuestionId >= 0)
        {
            Debug.Log("[QuestionPanel] GameState already has a question. Refreshing immediately.");
            Refresh();
        }
        else
        {
            Debug.Log("[QuestionPanel] No active question yet. Waiting for next OnQuestionChanged.");
        }
    }

    void Refresh()
    {
        var q = GameState.I.GetCurrentQuestion();
        if (q == null) return;

        prompt.text = q.prompt;
        var choices = GameState.I.CurrentChoices;

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            bool show = i < choices.Length;
            choiceButtons[i].gameObject.SetActive(show);
            if (!show) continue;

            choiceLabels[i].text = choices[i];
            choiceButtons[i].onClick.RemoveAllListeners();
            int idx = i;
            choiceButtons[i].onClick.AddListener(() => {
                // disable to avoid double-clicks while advancing
                for (int k = 0; k < choices.Length; k++) choiceButtons[k].interactable = false;
                GameState.I.SubmitAnswer(GameState.I.CurrentPlayerIndex, idx);
            });
            choiceButtons[i].interactable = true;
        }
    }

}
