// LOVEEVIXEN
using InputSystem;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        // Makes sure only one game manager instance exists to track manager data.
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
            Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float d = InputReader.joystickDead;
        float dd = InputReader.joystickDeadDiagonal;

        // Receive player 1 input to feed to InputReader.
        InputReader.Player1().holdingUp = Input.GetAxisRaw("VerticalP1") > d;
        InputReader.Player1().holdingRight = Input.GetAxisRaw("HorizontalP1") > d;
        InputReader.Player1().holdingDown = Input.GetAxisRaw("VerticalP1") < -d;
        InputReader.Player1().holdingLeft = Input.GetAxis("HorizontalP1") < -d;
        InputReader.Player1().holdingUpRight = Input.GetAxisRaw("VerticalP1") > dd && Input.GetAxisRaw("HorizontalP1") > dd;
        InputReader.Player1().holdingDownRight = Input.GetAxisRaw("VerticalP1") < -dd && Input.GetAxisRaw("HorizontalP1") > dd;
        InputReader.Player1().holdingDownLeft = Input.GetAxisRaw("VerticalP1") < -dd && Input.GetAxis("HorizontalP1") < -dd;
        InputReader.Player1().holdingUpLeft = Input.GetAxisRaw("VerticalP1") > dd && Input.GetAxis("HorizontalP1") < -dd;
        InputReader.Player1().holding0 = Input.GetButton("ZeroP1");
        InputReader.Player1().holding1 = Input.GetButton("OneP1");
        InputReader.Player1().holding2 = Input.GetButton("TwoP1");
        InputReader.Player1().holding3 = Input.GetButton("ThreeP1");
        InputReader.Player1().holdingStart = Input.GetButton("StartP1");
        InputReader.Player1().holdingSelect = Input.GetButton("SelectP1");


        // Receive player 2 input to feed to InputReader.
        InputReader.Player2().holdingUp = Input.GetAxisRaw("VerticalP2") > d;
        InputReader.Player2().holdingRight = Input.GetAxisRaw("HorizontalP2") > d;
        InputReader.Player2().holdingDown = Input.GetAxisRaw("VerticalP2") < -d;
        InputReader.Player2().holdingLeft = Input.GetAxis("HorizontalP2") < -d;
        InputReader.Player2().holdingUpRight = Input.GetAxisRaw("VerticalP2") > dd && Input.GetAxisRaw("HorizontalP2") > dd;
        InputReader.Player2().holdingDownRight = Input.GetAxisRaw("VerticalP2") < -dd && Input.GetAxisRaw("HorizontalP2") > dd;
        InputReader.Player2().holdingDownLeft = Input.GetAxisRaw("VerticalP2") < -dd && Input.GetAxis("HorizontalP2") < -dd;
        InputReader.Player2().holdingUpLeft = Input.GetAxisRaw("VerticalP2") > dd && Input.GetAxis("HorizontalP2") < -dd;
        InputReader.Player2().holding0 = Input.GetButton("ZeroP2");
        InputReader.Player2().holding1 = Input.GetButton("OneP2");
        InputReader.Player2().holding2 = Input.GetButton("TwoP2");
        InputReader.Player2().holding3 = Input.GetButton("ThreeP2");
        InputReader.Player2().holdingStart = Input.GetButton("StartP2");
        InputReader.Player2().holdingSelect = Input.GetButton("SelectP2");

        if (InputReader.Player1().holdingUp)
            Debug.Log("P1 Holding up!");

        if (InputReader.Player1().holdingRight)
            Debug.Log("P1 Holding right!");

        if (InputReader.Player1().holdingDown)
            Debug.Log("P1 Holding down!");

        if (InputReader.Player1().holdingLeft)
            Debug.Log("P1 Holding left!");

        if (InputReader.Player1().holdingUpRight)
            Debug.Log("P1 Holding up right!");

        if (InputReader.Player1().holdingDownRight)
            Debug.Log("P1 Holding down right!");

        if (InputReader.Player1().holdingDownLeft)
            Debug.Log("P1 Holding down left!");

        if (InputReader.Player1().holdingUpLeft)
            Debug.Log("P1 Holding up left!");

        if(InputReader.Player1().holding0)
            Debug.Log("P1 Holding 0!");

        if (InputReader.Player1().holding1)
            Debug.Log("P1 Holding 1!");

        if (InputReader.Player1().holding2)
            Debug.Log("P1 Holding 2!");

        if (InputReader.Player1().holding3)
            Debug.Log("P1 Holding 3!");

        if (InputReader.Player1().holdingStart)
            Debug.Log("P1 Holding start!");

        if (InputReader.Player1().holdingSelect)
            Debug.Log("P1 Holding select!");




        if (InputReader.Player2().holdingUp)
            Debug.Log("P2 Holding up!");

        if (InputReader.Player2().holdingRight)
            Debug.Log("P2 Holding right!");

        if (InputReader.Player2().holdingDown)
            Debug.Log("P2 Holding down!");

        if (InputReader.Player2().holdingLeft)
            Debug.Log("P2 Holding left!");

        if (InputReader.Player2().holdingUpRight)
            Debug.Log("P2 Holding up right!");

        if (InputReader.Player2().holdingDownRight)
            Debug.Log("P2 Holding down right!");

        if (InputReader.Player2().holdingDownLeft)
            Debug.Log("P2 Holding down left!");

        if (InputReader.Player2().holdingUpLeft)
            Debug.Log("P2 Holding up left!");

        if (InputReader.Player2().holding0)
            Debug.Log("P2 Holding 0!");

        if (InputReader.Player2().holding1)
            Debug.Log("P2 Holding 1!");

        if (InputReader.Player2().holding2)
            Debug.Log("P2 Holding 2!");

        if (InputReader.Player2().holding3)
            Debug.Log("P2 Holding 3!");

        if (InputReader.Player2().holdingStart)
            Debug.Log("P2 Holding start!");

        if (InputReader.Player2().holdingSelect)
            Debug.Log("P2 Holding select!");
    }
}
