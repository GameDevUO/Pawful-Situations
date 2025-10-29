// DeckInfo.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Trivia/Deck Info")]
public class DeckInfo : ScriptableObject
{
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;
    public QuestionBank bank;   // the actual questions
    public string[] tags;       // "easy","science","kids", etc. (optional)
}
