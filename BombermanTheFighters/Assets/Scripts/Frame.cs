// LOVEEVIXEN
using InputSystem;
using EntitySystem;

[System.Serializable]
public class Frame
{
    public PlayerInputData player1Input = new PlayerInputData();
    public PlayerInputData player2Input = new PlayerInputData();
    public Player player1;
    public Player player2;
}
