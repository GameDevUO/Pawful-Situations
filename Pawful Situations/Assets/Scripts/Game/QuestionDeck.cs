using System;
using UnityEngine;

/// <summary>
/// Runtime deck that manages question order.
/// It just shuffles indices 0..N-1 and lets you draw them sequentially.
/// </summary>
[Serializable]
public class QuestionDeck
{
    private int[] ids;
    private int ptr = 0;

    public QuestionDeck(int count)
    {
        ids = new int[count];
        for (int i = 0; i < count; i++)
            ids[i] = i;
    }

    public void Shuffle(System.Random rng)
    {
        for (int i = ids.Length - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (ids[i], ids[j]) = (ids[j], ids[i]);
        }
        ptr = 0;
        Debug.Log("[QuestionDeck] Shuffled deck of " + ids.Length + " questions.");
    }

    public bool HasNext => ptr < ids.Length;

    public int NextId()
    {
        if (!HasNext)
        {
            Debug.LogWarning("[QuestionDeck] Tried to draw past end of deck!");
            return -1;
        }

        int id = ids[ptr++];
        Debug.Log("[QuestionDeck] Drew question index " + id);
        return id;
    }

    public void Reset()
    {
        ptr = 0;
        Debug.Log("[QuestionDeck] Reset pointer to 0.");
    }
}
