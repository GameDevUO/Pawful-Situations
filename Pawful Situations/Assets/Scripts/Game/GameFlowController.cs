// GameFlowController.cs
using UnityEngine;
using System.Collections;

public class GameFlowController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject deckSelectPanel;   // your DeckSelectPanel root
    [SerializeField] GameObject questionPanel;     // your question UI root
    [SerializeField] float returnDelay = 0.75f;    // small delay for last feedback

    void OnEnable()
    {
        if (GameState.I != null)
            GameState.I.OnRoundEnded += HandleRoundEnded;
    }

    void OnDisable()
    {
        if (GameState.I != null)
            GameState.I.OnRoundEnded -= HandleRoundEnded;
    }

    void HandleRoundEnded()
    {
        // Optionally wait a moment to let the last animation/text finish
        StartCoroutine(ReturnToDecksAfterDelay());
    }

    IEnumerator ReturnToDecksAfterDelay()
    {
        yield return new WaitForSeconds(returnDelay);

        // Hide trivia UI, show deck menu
        if (questionPanel != null) questionPanel.SetActive(false);
        if (deckSelectPanel != null) deckSelectPanel.SetActive(true);

        Debug.Log("[GameFlow] Returned to deck select.");
    }
}
