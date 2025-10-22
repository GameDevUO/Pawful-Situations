[System.Serializable]
public class GameSettings
{
    // Game Settings Configuration
    // General game settings
    public float MasterVolume = 0.5f; // Master volume level (0.0 to 1.0)

    // Gameplay settings
    public int CardsPerHand = 7;
    public int Rounds = 4;
    public bool Teams = false;
}