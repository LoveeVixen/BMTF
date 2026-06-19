// LOVEEVIXEN
using InputSystem;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    int targetFramerate = 60;

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
        Application.targetFrameRate = targetFramerate;
    }

    // Update is called once per frame
    void Update()
    {
        float d = InputReader.joystickDead;

        // Player 1.
        float horP1 = Input.GetAxisRaw("HorizontalP1");
        float verP1 = Input.GetAxisRaw("VerticalP1");

        if (Input.GetKey(InputReader.upP1))
            verP1 = 1f;
        
        if (Input.GetKey(InputReader.rightP1))
            horP1 = 1f;

        if (Input.GetKey(InputReader.downP1))
            verP1 = -1f;

        if (Input.GetKey(InputReader.leftP1))
            horP1 = -1f;

        // Receive holding input to feed to InputReader.
        InputReader.Player1().holdingUp = verP1 > d;
        InputReader.Player1().holdingRight = horP1 > d;
        InputReader.Player1().holdingDown = verP1 < -d;
        InputReader.Player1().holdingLeft = horP1 < -d;

        InputReader.Player1().holdingUpRight = InputReader.Player1().holdingUp && InputReader.Player1().holdingRight;
        if (InputReader.Player1().holdingUpRight)
        {
            InputReader.Player1().holdingUp = false;
            InputReader.Player1().holdingRight = false;
        }

        InputReader.Player1().holdingDownRight = InputReader.Player1().holdingDown && InputReader.Player1().holdingRight;
        if (InputReader.Player1().holdingDownRight)
        {
            InputReader.Player1().holdingDown = false;
            InputReader.Player1().holdingRight = false;
        }

        InputReader.Player1().holdingDownLeft = InputReader.Player1().holdingDown && InputReader.Player1().holdingLeft;
        if (InputReader.Player1().holdingDownLeft)
        {
            InputReader.Player1().holdingDown = false;
            InputReader.Player1().holdingLeft = false;
        }

        InputReader.Player1().holdingUpLeft = InputReader.Player1().holdingUp && InputReader.Player1().holdingLeft;
        if (InputReader.Player1().holdingUpLeft)
        {
            InputReader.Player1().holdingUp = false;
            InputReader.Player1().holdingLeft = false;
        }

        InputReader.Player1().holding0 = Input.GetButton("ZeroP1") || Input.GetKey(InputReader.zeroP1);
        InputReader.Player1().holding1 = Input.GetButton("OneP1") || Input.GetKey(InputReader.oneP1);
        InputReader.Player1().holding2 = Input.GetButton("TwoP1") || Input.GetKey(InputReader.twoP1);
        InputReader.Player1().holding3 = Input.GetButton("ThreeP1") || Input.GetKey(InputReader.threeP1);
        InputReader.Player1().holdingStart = Input.GetButton("StartP1") || Input.GetKey(InputReader.startP1);
        InputReader.Player1().holdingSelect = Input.GetButton("SelectP1") || Input.GetKey(InputReader.selectP1);

        // Receive player 1 pressing/tap input to feed to InputReader.
        bool pressedUpP1 = false;
        if (InputReader.Player1().holdingUp && !InputReader.Player1().RegPressingUp)
        {
            InputReader.Player1().RegPressingUp = true;
            pressedUpP1 = true;
        }
        else if(!InputReader.Player1().holdingUp)
            InputReader.Player1().RegPressingUp = false;
        InputReader.Player1().pressingUp = pressedUpP1;

        bool pressedRightP1 = false;
        if (InputReader.Player1().holdingRight && !InputReader.Player1().RegPressingRight)
        {
            InputReader.Player1().RegPressingRight = true;
            pressedRightP1 = true;
        }
        else if (!InputReader.Player1().holdingRight)
            InputReader.Player1().RegPressingRight = false;
        InputReader.Player1().pressingRight = pressedRightP1;

        bool pressedDownP1 = false;
        if (InputReader.Player1().holdingDown && !InputReader.Player1().RegPressingDown)
        {
            InputReader.Player1().RegPressingDown = true;
            pressedDownP1 = true;
        }
        else if (!InputReader.Player1().holdingDown)
            InputReader.Player1().RegPressingDown = false;
        InputReader.Player1().pressingDown = pressedDownP1;

        bool pressedLeftP1 = false;
        if (InputReader.Player1().holdingLeft && !InputReader.Player1().RegPressingLeft)
        {
            InputReader.Player1().RegPressingLeft = true;
            pressedLeftP1 = true;
        }
        else if (!InputReader.Player1().holdingLeft)
            InputReader.Player1().RegPressingLeft = false;
        InputReader.Player1().pressingLeft = pressedLeftP1;

        bool pressedUpRightP1 = false;
        if (InputReader.Player1().holdingUpRight && !InputReader.Player1().RegPressingUpRight)
        {
            InputReader.Player1().RegPressingUpRight = true;
            pressedUpRightP1 = true;
        }
        else if (!InputReader.Player1().holdingUpRight)
            InputReader.Player1().RegPressingUpRight = false;
        InputReader.Player1().pressingUpRight = pressedUpRightP1;

        bool pressedDownRightP1 = false;
        if (InputReader.Player1().holdingDownRight && !InputReader.Player1().RegPressingDownRight)
        {
            InputReader.Player1().RegPressingDownRight = true;
            pressedDownRightP1 = true;
        }
        else if (!InputReader.Player1().holdingDownRight)
            InputReader.Player1().RegPressingDownRight = false;
        InputReader.Player1().pressingDownRight = pressedDownRightP1;

        bool pressedDownLeftP1 = false;
        if (InputReader.Player1().holdingDownLeft && !InputReader.Player1().RegPressingDownLeft)
        {
            InputReader.Player1().RegPressingDownLeft = true;
            pressedDownLeftP1 = true;
        }
        else if (!InputReader.Player1().holdingDownLeft)
            InputReader.Player1().RegPressingDownLeft = false;
        InputReader.Player1().pressingDownLeft = pressedDownLeftP1;

        bool pressedUpLeftP1 = false;
        if (InputReader.Player1().holdingUpLeft && !InputReader.Player1().RegPressingUpLeft)
        {
            InputReader.Player1().RegPressingUpLeft = true;
            pressedUpLeftP1 = true;
        }
        else if (!InputReader.Player1().holdingUpLeft)
            InputReader.Player1().RegPressingUpLeft = false;
        InputReader.Player1().pressingUpLeft = pressedUpLeftP1;

        InputReader.Player1().pressing0 = Input.GetButtonDown("ZeroP1") || Input.GetKeyDown(InputReader.zeroP1);
        InputReader.Player1().pressing1 = Input.GetButtonDown("OneP1") || Input.GetKeyDown(InputReader.oneP1);
        InputReader.Player1().pressing2 = Input.GetButtonDown("TwoP1") || Input.GetKeyDown(InputReader.twoP1);
        InputReader.Player1().pressing3 = Input.GetButtonDown("ThreeP1") || Input.GetKeyDown(InputReader.threeP1);
        InputReader.Player1().pressingStart = Input.GetButtonDown("StartP1") || Input.GetKeyDown(InputReader.startP1);
        InputReader.Player1().pressingSelect = Input.GetButtonDown("SelectP1") || Input.GetKeyDown(InputReader.selectP1);

        // Player 2.
        float horP2 = Input.GetAxisRaw("HorizontalP2");
        float verP2 = Input.GetAxisRaw("VerticalP2");

        if (Input.GetKey(InputReader.upP2))
        {
            verP2 = 1f;
            horP2 /= 2f;
        }

        if (Input.GetKey(InputReader.rightP2))
        {
            horP2 = 1f;
            verP2 /= 2f;
        }

        if (Input.GetKey(InputReader.downP2))
        {
            verP2 = -1f;
            horP2 /= 2f;
        }

        if (Input.GetKey(InputReader.leftP2))
        {
            horP2 = -1f;
            verP2 /= 2f;
        }

        // Receive holding input to feed to InputReader.
        InputReader.Player2().holdingUp = verP2 > d;
        InputReader.Player2().holdingRight = horP2 > d;
        InputReader.Player2().holdingDown = verP2 < -d;
        InputReader.Player2().holdingLeft = horP2 < -d;

        InputReader.Player2().holdingUpRight = InputReader.Player2().holdingUp && InputReader.Player2().holdingRight;
        if (InputReader.Player2().holdingUpRight)
        {
            InputReader.Player2().holdingUp = false;
            InputReader.Player2().holdingRight = false;
        }

        InputReader.Player2().holdingDownRight = InputReader.Player2().holdingDown && InputReader.Player2().holdingRight;
        if (InputReader.Player2().holdingDownRight)
        {
            InputReader.Player2().holdingDown = false;
            InputReader.Player2().holdingRight = false;
        }

        InputReader.Player2().holdingDownLeft = InputReader.Player2().holdingDown && InputReader.Player2().holdingLeft;
        if (InputReader.Player2().holdingDownLeft)
        {
            InputReader.Player2().holdingDown = false;
            InputReader.Player2().holdingLeft = false;
        }

        InputReader.Player2().holdingUpLeft = InputReader.Player2().holdingUp && InputReader.Player2().holdingLeft;
        if (InputReader.Player2().holdingUpLeft)
        {
            InputReader.Player2().holdingUp = false;
            InputReader.Player2().holdingLeft = false;
        }

        InputReader.Player2().holding0 = Input.GetButton("ZeroP2") || Input.GetKey(InputReader.zeroP2);
        InputReader.Player2().holding1 = Input.GetButton("OneP2") || Input.GetKey(InputReader.oneP2);
        InputReader.Player2().holding2 = Input.GetButton("TwoP2") || Input.GetKey(InputReader.twoP2);
        InputReader.Player2().holding3 = Input.GetButton("ThreeP2") || Input.GetKey(InputReader.threeP2);
        InputReader.Player2().holdingStart = Input.GetButton("StartP2") || Input.GetKey(InputReader.startP2);
        InputReader.Player2().holdingSelect = Input.GetButton("SelectP2") || Input.GetKey(InputReader.selectP2);

        // Receive player 2 pressing/tap input to feed to InputReader.
        bool pressedUpP2 = false;
        if (InputReader.Player2().holdingUp && !InputReader.Player2().RegPressingUp)
        {
            InputReader.Player2().RegPressingUp = true;
            pressedUpP2 = true;
        }
        else if (!InputReader.Player2().holdingUp)
            InputReader.Player2().RegPressingUp = false;
        InputReader.Player2().pressingUp = pressedUpP2;

        bool pressedRightP2 = false;
        if (InputReader.Player2().holdingRight && !InputReader.Player2().RegPressingRight)
        {
            InputReader.Player2().RegPressingRight = true;
            pressedRightP2 = true;
        }
        else if (!InputReader.Player2().holdingRight)
            InputReader.Player2().RegPressingRight = false;
        InputReader.Player2().pressingRight = pressedRightP2;

        bool pressedDownP2 = false;
        if (InputReader.Player2().holdingDown && !InputReader.Player2().RegPressingDown)
        {
            InputReader.Player2().RegPressingDown = true;
            pressedDownP2 = true;
        }
        else if (!InputReader.Player2().holdingDown)
            InputReader.Player2().RegPressingDown = false;
        InputReader.Player2().pressingDown = pressedDownP2;

        bool pressedLeftP2 = false;
        if (InputReader.Player2().holdingLeft && !InputReader.Player2().RegPressingLeft)
        {
            InputReader.Player2().RegPressingLeft = true;
            pressedLeftP2 = true;
        }
        else if (!InputReader.Player2().holdingLeft)
            InputReader.Player2().RegPressingLeft = false;
        InputReader.Player2().pressingLeft = pressedLeftP2;

        bool pressedUpRightP2 = false;
        if (InputReader.Player2().holdingUpRight && !InputReader.Player2().RegPressingUpRight)
        {
            InputReader.Player2().RegPressingUpRight = true;
            pressedUpRightP2 = true;
        }
        else if (!InputReader.Player2().holdingUpRight)
            InputReader.Player2().RegPressingUpRight = false;
        InputReader.Player2().pressingUpRight = pressedUpRightP2;

        bool pressedDownRightP2 = false;
        if (InputReader.Player2().holdingDownRight && !InputReader.Player2().RegPressingDownRight)
        {
            InputReader.Player2().RegPressingDownRight = true;
            pressedDownRightP2 = true;
        }
        else if (!InputReader.Player2().holdingDownRight)
            InputReader.Player2().RegPressingDownRight = false;
        InputReader.Player2().pressingDownRight = pressedDownRightP2;

        bool pressedDownLeftP2 = false;
        if (InputReader.Player2().holdingDownLeft && !InputReader.Player2().RegPressingDownLeft)
        {
            InputReader.Player2().RegPressingDownLeft = true;
            pressedDownLeftP2 = true;
        }
        else if (!InputReader.Player2().holdingDownLeft)
            InputReader.Player2().RegPressingDownLeft = false;
        InputReader.Player2().pressingDownLeft = pressedDownLeftP2;

        bool pressedUpLeftP2 = false;
        if (InputReader.Player2().holdingUpLeft && !InputReader.Player2().RegPressingUpLeft)
        {
            InputReader.Player2().RegPressingUpLeft = true;
            pressedUpLeftP2 = true;
        }
        else if (!InputReader.Player2().holdingUpLeft)
            InputReader.Player2().RegPressingUpLeft = false;
        InputReader.Player2().pressingUpLeft = pressedUpLeftP2;

        InputReader.Player2().pressing0 = Input.GetButtonDown("ZeroP2") || Input.GetKeyDown(InputReader.zeroP2);
        InputReader.Player2().pressing1 = Input.GetButtonDown("OneP2") || Input.GetKeyDown(InputReader.oneP2);
        InputReader.Player2().pressing2 = Input.GetButtonDown("TwoP2") || Input.GetKeyDown(InputReader.twoP2);
        InputReader.Player2().pressing3 = Input.GetButtonDown("ThreeP2") || Input.GetKeyDown(InputReader.threeP2);
        InputReader.Player2().pressingStart = Input.GetButtonDown("StartP2") || Input.GetKeyDown(InputReader.startP2);
        InputReader.Player2().pressingSelect = Input.GetButtonDown("SelectP2") || Input.GetKeyDown(InputReader.selectP2);

        string p1Input = "P1: ";
        if (InputReader.Player1().pressing0)
            p1Input += "0, ";

        if (InputReader.Player1().pressing1)
            p1Input += "1, ";

        if (InputReader.Player1().pressing2)
            p1Input += "2, ";

        if (InputReader.Player1().pressing3)
            p1Input += "3, ";

        if (InputReader.Player1().pressingStart)
            p1Input += "START, ";

        if (InputReader.Player1().pressingSelect)
            p1Input += "SELECT, ";

        if (InputReader.Player1().pressingUp)
            p1Input += "UP, ";

        if (InputReader.Player1().pressingRight)
            p1Input += "RIGHT, ";

        if (InputReader.Player1().pressingDown)
            p1Input += "DOWN, ";

        if (InputReader.Player1().pressingLeft)
            p1Input += "LEFT, ";

        if (InputReader.Player1().pressingUpRight)
            p1Input += "UP-RIGHT, ";

        if (InputReader.Player1().pressingDownRight)
            p1Input += "DOWN-RIGHT, ";

        if (InputReader.Player1().pressingDownLeft)
            p1Input += "DOWN-LEFT, ";

        if (InputReader.Player1().pressingUpLeft)
            p1Input += "UP-LEFT, ";

        if (p1Input != "P1: ")
            Debug.Log(p1Input);



        string p2Input = "P2: ";
        if (InputReader.Player2().pressing0)
            p2Input += "0, ";

        if (InputReader.Player2().pressing1)
            p2Input += "1, ";

        if (InputReader.Player2().pressing2)
            p2Input += "2, ";

        if (InputReader.Player2().pressing3)
            p2Input += "3, ";

        if (InputReader.Player2().pressingStart)
            p2Input += "START, ";

        if (InputReader.Player2().pressingSelect)
            p2Input += "SELECT, ";

        if (InputReader.Player2().pressingUp)
            p2Input += "UP, ";

        if (InputReader.Player2().pressingRight)
            p2Input += "RIGHT, ";

        if (InputReader.Player2().pressingDown)
            p2Input += "DOWN, ";

        if (InputReader.Player2().pressingLeft)
            p2Input += "LEFT, ";

        if (InputReader.Player2().pressingUpRight)
            p2Input += "UP-RIGHT, ";

        if (InputReader.Player2().pressingDownRight)
            p2Input += "DOWN-RIGHT, ";

        if (InputReader.Player2().pressingDownLeft)
            p2Input += "DOWN-LEFT, ";

        if (InputReader.Player2().pressingUpLeft)
            p2Input += "UP-LEFT, ";

        if (p2Input != "P2: ")
            Debug.Log(p2Input);




        /*if (InputReader.Player1().holdingUp)
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
            Debug.Log("P2 Holding select!");*/
    }
}
