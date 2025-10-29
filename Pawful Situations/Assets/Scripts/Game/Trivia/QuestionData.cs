[System.Serializable]

// holds all necessary info about a trivia question
public class QuestionData
{
    public string id;          // "HIST_001" (optional but handy)
    public string prompt;      // The question text
    public string[] choices;   // e.g., length 4
    public int correctIndex;   // index into choices BEFORE shuffling
}