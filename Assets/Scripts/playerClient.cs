using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class playerClient : MonoBehaviour
{
    private GameObject clientObj;
    private Animator animator;
    int jumpAnimation;
    int shootAnimation;
    State playerState;
    [HideInInspector]
    public CustomClasses.PlayerState updateState;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {

            clientObj.GetComponent<clientUDP>().AddInput(KeyCode.D, "KeyPressed");
            // Debug.Log("D key Pressed");
            animator.SetFloat("isWalking", 1f);
            SetState("WALK_RIGHT");
            //clientObj.GetComponent<clientUDP>().AddState(GetState());
        }
        if (Input.GetKey(KeyCode.A))
        {
            clientObj.GetComponent<clientUDP>().AddInput(KeyCode.A, "KeyPressed");
            // Debug.Log("A key Pressed");
            animator.SetFloat("isWalking", 0f);
            SetState("WALK_LEFT");
            //clientObj.GetComponent<clientUDP>().AddState(GetState());
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            // Debug.Log("D key Up");
            clientObj.GetComponent<clientUDP>().AddInput(KeyCode.D, "KeyUp");
            animator.SetFloat("isWalking", 0.5f);
            SetState("IDLE");
            //clientObj.GetComponent<clientUDP>().AddState(GetState());
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            // Debug.Log("A key Up");
            clientObj.GetComponent<clientUDP>().AddInput(KeyCode.A, "KeyUp");
            animator.SetFloat("isWalking", 0.5f);
            SetState("IDLE");
            //clientObj.GetComponent<clientUDP>().AddState(GetState());
        }
        if (Input.GetMouseButtonDown(0))
        {
            clientObj.GetComponent<clientUDP>().AddInput(0, "MouseButtonDown");
            animator.SetTrigger(shootAnimation);

        }
        else { animator.SetBool("isShooting", false); }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            clientObj.GetComponent<clientUDP>().AddInput(KeyCode.Space, "KeyDown");
            animator.SetTrigger(jumpAnimation);
            SetState("JUMP");
            //clientObj.GetComponent<clientUDP>().AddState(GetState());
        }
    }

    public void SetState(string state)
    {
        playerState = (State)Enum.Parse(typeof(State), state, true);
    }

    public string GetState()
    {
        return playerState.ToString();
    }
}
