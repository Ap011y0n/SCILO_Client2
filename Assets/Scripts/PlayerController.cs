using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
   private Rigidbody body;
   public float jumpforce = 20f;
   public float MovSpeed = 10f;
   public GameObject gun;
    public GameObject bullet;
   enum State
    {
        IDLE,
        JUMP
    }
    State state;

    void Start()
    {
        state = State.IDLE;
        body = this.gameObject.GetComponent<Rigidbody>();
    }

    void changeState(State newState)
    {
        switch (newState)
        {
            case State.IDLE:
                break;
            case State.JUMP:
                Vector3 force = new Vector3(0, jumpforce, 0);
                body.AddForce(force);
                state = State.JUMP;
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("floor") && state == State.JUMP)
        {
            state = State.IDLE;
        }
    }
    private void ProcessInput()
    {
        //de momento, el movimiento va asi, pero habria que hacer dos estados nuevos, de moverse derecha e izquierda, teniendo en cuenta si se esta en el suelo o no

        if (Input.GetKey(KeyCode.D))
        {
            Vector3 temp = new Vector3(0, 0, MovSpeed * Time.deltaTime);
            gameObject.transform.position += temp;
        }
        if (Input.GetKey(KeyCode.A))
        {
            Vector3 temp = new Vector3(0, 0, -MovSpeed * Time.deltaTime);
            gameObject.transform.position += temp;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(bullet, gun.transform.position, gun.transform.rotation);

        }
        if (Input.GetKeyDown(KeyCode.Space) && state != State.JUMP)
        {
            changeState(State.JUMP);

        }
    }
}
