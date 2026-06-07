// LOVEEVIXEN
using UnityEngine;

public class Player : Entity
{
    // Position
    private float posX;
    private float posY;
    private float posZ;

    // Animation
    private Animator anim;
    private float currentAnimationTime;

    // Misc..
    private enum CurrentState { idle, moveForward, moveBackward, sideStepUp, sideStepDown, attacking, lay, knockout };
    private CurrentState currentState;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
}
