public class Player
{
    public string name;
    public int score;
    public int lives;
    public bool eliminated => lives <= 0;
}