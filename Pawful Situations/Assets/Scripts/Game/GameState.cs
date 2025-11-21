using UnityEngine;
using System;
using System.Collections.Generic;

[DefaultExecutionOrder(-1000)]
public class GameState : MonoBehaviour
{
    public static GameState I { get; private set; }

    [Header("Content")]
    [SerializeField] QuestionBank bank;
    QuestionData[] pool;  // runtime combined questions (from 1+ decks)


    [Header("Game Rules")]
    [SerializeField] int startingLives = 3;
    [SerializeField] int deathDieSides = 6;
    [SerializeField] bool shuffleAnswersEachTime = true;

    public IReadOnlyList<Player> Players => players;
    public int CurrentPlayerIndex { get; private set; }
    public int CurrentQuestionId { get; private set; } = -1;
    public int MappedCorrectIndex { get; private set; } = -1;
    public string[] CurrentChoices { get; private set; }

    public event Action OnQuestionChanged;
    public event Action<int, bool, int> OnPlayerAnswered;
    public event Action OnRoundEnded;

    List<Player> players = new();
    QuestionDeck deck;
    System.Random rng;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        rng = new System.Random();
        Debug.Log("[GameState] Awake - Singleton set");
    }

    void Start()
    {
        Debug.Log("[GameState] Start - Ready to begin");
    }

    // existing single-deck
    public void StartNewMatch(string[] playerNames)
    {
        if (bank == null) { Debug.LogWarning("[GameState] No default deck assigned."); return; }
        StartNewMatch(playerNames, new[] { bank });
    }

    // new multi-deck overload
    public void StartNewMatch(string[] playerNames, QuestionBank[] selectedBanks)
    {
        Debug.Log($"[GameState] StartNewMatch: {playerNames.Length} players, {selectedBanks?.Length ?? 0} deck(s).");

        players.Clear();
        foreach (var n in playerNames)
            players.Add(new Player { name = n, score = 0, lives = startingLives });

        // build runtime pool
        var list = new List<QuestionData>();
        foreach (var b in selectedBanks)
        {
            if (b == null || b.questions == null) continue;
            list.AddRange(b.questions);
        }
        pool = list.ToArray();

        if (pool.Length == 0) { Debug.LogError("[GameState] Selected decks contain no questions."); OnRoundEnded?.Invoke(); return; }

        deck = new QuestionDeck(pool.Length);
        deck.Shuffle(rng);
        CurrentPlayerIndex = 0;
        NextQuestion();
    }

    void NextQuestion()
    {
        if (!deck.HasNext) 
        { 
            Debug.Log("[GameState] No more questions."); 
            CurrentQuestionId = -1;
            CurrentChoices = null;
            OnRoundEnded?.Invoke(); 
            return; 
        }

        CurrentQuestionId = deck.NextId();
        var q = pool[CurrentQuestionId]; // << use runtime pool now

        // same shuffleAnswersEachTime logic
        if (shuffleAnswersEachTime)
        {
            var (choices, mapped) = ShuffleChoices(q.choices, q.correctIndex, rng.Next());
            CurrentChoices = choices;
            MappedCorrectIndex = mapped;
        }
        else
        {
            CurrentChoices = (string[])q.choices.Clone();
            MappedCorrectIndex = q.correctIndex;
        }

        OnQuestionChanged?.Invoke();
    }

    // Add a helper for UI to read the current question without needing a bank reference:
    public QuestionData GetCurrentQuestion() => (CurrentQuestionId >= 0 && CurrentQuestionId < pool.Length) ? pool[CurrentQuestionId] : null;


    public void SubmitAnswer(int playerIndex, int chosenIndex)
    {
        if (Players[playerIndex].eliminated)
        {
            Debug.LogWarning($"[GameState] Player {playerIndex} tried to answer but is eliminated.");
            return;
        }

        bool correct = (chosenIndex == MappedCorrectIndex);
        int roll = -1;

        if (correct)
        {
            players[playerIndex].score += 1;
            Debug.Log($"[GameState] Player {playerIndex} answered correctly! Score: {players[playerIndex].score}");
        }
        else
        {
            roll = rng.Next(1, deathDieSides + 1);
            bool death = (roll == 1);
            if (death) players[playerIndex].lives = Mathf.Max(0, players[playerIndex].lives - 1);
            Debug.Log($"[GameState] Player {playerIndex} WRONG! Rolled {roll} -> {(death ? "DEATH" : "SAFE")}. Lives: {players[playerIndex].lives}");
        }

        OnPlayerAnswered?.Invoke(playerIndex, correct, roll);
        AdvanceTurn();
    }

    void AdvanceTurn()
    {
        int old = CurrentPlayerIndex;
        int countTried = 0;
        do
        {
            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % players.Count;
            countTried++;
        } while (players[CurrentPlayerIndex].eliminated && countTried <= players.Count);

        Debug.Log($"[GameState] Advancing turn: P{old + 1} ? P{CurrentPlayerIndex + 1}");
        NextQuestion();
    }

    static (string[] choices, int mappedCorrect) ShuffleChoices(string[] src, int correctIdx, int seed)
    {
        var rng = new System.Random(seed);
        int n = src.Length;
        var order = new int[n];
        for (int i = 0; i < n; i++) order[i] = i;
        for (int i = n - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (order[i], order[j]) = (order[j], order[i]);
        }

        var outChoices = new string[n];
        int mapped = -1;
        for (int i = 0; i < n; i++)
        {
            outChoices[i] = src[order[i]];
            if (order[i] == correctIdx) mapped = i;
        }

        Debug.Log($"[GameState] Shuffled answers. Correct index ? {mapped}");
        return (outChoices, mapped);
    }
}
