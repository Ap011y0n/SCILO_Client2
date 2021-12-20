using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerClient : MonoBehaviour
{
    private GameObject clientObj;
    private Animator animator;
    int jumpAnimation;
    int shootAnimation;
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
        }
        if (Input.GetKey(KeyCode.A))
        {
            clientObj.GetComponent<clientUDP>().AddInput(KeyCode.A, "KeyPressed");
            // Debug.Log("A key Pressed");
            animator.SetFloat("isWalking", 0f);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
           // Debug.Log("D key Up");
            clientObj.GetComponent<clientUDP>().AddInput(KeyCode.D, "KeyUp");
            animator.SetFloat("isWalking", 0.5f);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
           // Debug.Log("A key Up");
            clientObj.GetComponent<clientUDP>().AddInput(KeyCode.A, "KeyUp");
            animator.SetFloat("isWalking", 0.5f);
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
        }
    }
}
