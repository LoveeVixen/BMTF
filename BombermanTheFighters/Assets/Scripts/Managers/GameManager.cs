// LOVEEVIXEN
using InputSystem;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private int targetFramerate = 60;
    [SerializeField] List<Character> characters = new List<Character>();

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
        Stage.LoadStagesFromResources();
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
    }

    // Return a character by it's name.
    public Character FindCharacter(string characterName)
    {
        foreach(Character character in characters)
        {
            if(character.name == characterName)
                return character;
        }

        Debug.Log("Unable to find character with name: " + characterName);
        return null;
    }
}
