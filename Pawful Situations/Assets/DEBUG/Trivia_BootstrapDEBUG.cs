// Bootstrap.cs (attach to the Bootstrap GameObject)
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] GameState gameState;
    [SerializeField] string[] playerNames = { "P1", "P2", "P3", "P4" };

    void Start()
    {
        if (gameState == null) gameState = GameState.I;
        gameState.StartNewMatch(playerNames);
    }
}
