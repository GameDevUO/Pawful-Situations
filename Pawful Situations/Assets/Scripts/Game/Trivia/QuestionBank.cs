using UnityEngine;

// Individual question data structure
[CreateAssetMenu(menuName = "Trivia/QuestionBank")]
public class QuestionBank : ScriptableObject
{
    public QuestionData[] questions;
}
