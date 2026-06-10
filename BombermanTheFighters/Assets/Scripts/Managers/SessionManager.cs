// LOVEEVIXEN
using UnityEngine;
using System.Collections.Generic;
using InputSystem;

public class SessionManager : MonoBehaviour
{
    private List<Frame> frames = new List<Frame>();
    private int currentFrame = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFrame();
    }

    Frame UpdateFrame()
    {
        Frame updateFrame = new Frame();
        updateFrame.player1Input = PlayerInputData.CloneData(InputReader.Player1());
        updateFrame.player2Input = PlayerInputData.CloneData(InputReader.Player2());

        frames.Add(updateFrame);
        currentFrame++;

        return updateFrame;
    }
}
