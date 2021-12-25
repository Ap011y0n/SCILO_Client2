using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player2Controller : MonoBehaviour
{
    private GameObject clientObj;
    private Animator animator;
    int jumpAnimation;
    int shootAnimation;
    Vector3 lastPos;
    int frameCounter;
    [HideInInspector]
    public CustomClasses.PlayerState updateState;
    State playerState;

    enum State
    {
        IDLE,
        WALK_RIGHT,
        WALK_LEFT,
        JUMP
    }

    // Start is called before the first frame update
    void Start()
    {
        clientObj = GameObject.Find("Client");
        animator = GetComponent<Animator>();
        jumpAnimation = Animator.StringToHash("Jump");
        shootAnimation = Animator.StringToHash("Shoot");
        frameCounter = 0;
        lastPos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Animate(this.transform.position);
        lastPos = this.transform.position;
        /*if(frameCounter >= 5)
        {
            if (lastPos != this.transform.position)
            {
                Animate(this.transform.position);
            }
            else { animator.SetFloat("isWalking", 0.5f); }

            lastPos = this.transform.position;
            frameCounter = 0;
        }

        frameCounter++;*/
    }

    public void Animate(Vector3 pos)
    {
        /*if (pos.z > lastPos.z)
        {
            animator.SetFloat("isWalking", 1f);
        }
        else if(pos.z < lastPos.z)
        {
            animator.SetFloat("isWalking", 0f);
        }

        if(pos.y > lastPos.y)
        {
            animator.SetTrigger(jumpAnimation);
        }*/
        if (playerState == State.IDLE) 
        {
            animator.SetFloat("isWalking", 0.5f);
        }
        else if(playerState == State.WALK_RIGHT)
        {
            animator.SetFloat("isWalking", 1f);
        }
        else if (playerState == State.WALK_LEFT)
        {
            animator.SetFloat("isWalking", 0f);
        }
        else if (playerState == State.JUMP)
        {
            animator.SetTrigger(jumpAnimation);
        }
    }

    public void UpdateState(CustomClasses.PlayerState state)
    {
        playerState = (State)Enum.Parse(typeof(State), state.state, true);
        Debug.Log("State: " + playerState.ToString());
    }
}
