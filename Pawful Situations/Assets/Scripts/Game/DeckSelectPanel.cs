using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DeckSelectPanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Transform content;         // ScrollView/Viewport/Content
    [SerializeField] Toggle togglePrefab;       // ToggleDeckItem prefab
    [SerializeField] Button startButton;
    [SerializeField] GameObject questionPanelRoot; // Panel_Question root
    [SerializeField] TMP_Text headerText;       // optional: "Choose your deck(s)"

    [Header("Options")]
    [SerializeField] bool allowMultipleDecks = true;

    readonly List<DeckInfo> decks = new();
    readonly List<Toggle> toggles = new();

    void Awake()
    {
        decks.AddRange(Resources.LoadAll<DeckInfo>("Decks"));
        if (decks.Count == 0)
        {
            Debug.LogError("[DeckSelect] No DeckInfo assets found in Resources/Decks.");
        }

        foreach (var deck in decks)
        {
            var t = Instantiate(togglePrefab, content);
            t.isOn = false;
            t.group = allowMultipleDecks ? null : content.GetComponent<ToggleGroup>();

            var label = t.GetComponentInChildren<TMP_Text>();
            if (label) label.text = deck.displayName;

            // If you added an Icon Image child:
            var icon = t.transform.Find("Icon")?.GetComponent<Image>();
            if (icon && deck.icon) icon.sprite = deck.icon;

            toggles.Add(t);
        }

        startButton.onClick.AddListener(OnStartClicked);
        if (headerText) headerText.text = "Choose a deck" + (allowMultipleDecks ? "(s)" : "");
    }

    void OnStartClicked()
    {
        var selectedBanks = new List<QuestionBank>();
        for (int i = 0; i < decks.Count; i++)
        {
            if (toggles[i] != null && toggles[i].isOn && decks[i].bank != null)
            {
                selectedBanks.Add(decks[i].bank);
            }
        }

        if (selectedBanks.Count == 0)
        {
            Debug.LogWarning("[DeckSelect] No decks selected. Select at least one.");
            return;
        }

        // Example player names (swap to your menu’s inputs if you have them)
        string[] players = { "P1", "P2", "P3", "P4" };

        Debug.Log($"[DeckSelect] Starting match with {selectedBanks.Count} deck(s).");
        GameState.I.StartNewMatch(players, selectedBanks.ToArray());

        // Hide deck menu, show question panel
        gameObject.SetActive(false);
        if (questionPanelRoot) questionPanelRoot.SetActive(true);
    }
}
