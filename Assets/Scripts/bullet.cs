using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    // Start is called before the first frame update
    public float velocity = 10;
    public float lifeTime = 5;
    private System.DateTime start;
    void Start()
    {
        Vector3 temp = transform.forward * velocity;
        GetComponent<Rigidbody>().velocity += temp;
        start = System.DateTime.UtcNow;
    }

    // Update is called once per frame
    void Update()
    {
        if((System.DateTime.UtcNow - start).Seconds > 5f)
        {
            Destroy(gameObject);
            Debug.LogError("Delete me from dictionary");
            //GameObject.Find("Client").GetComponent<clientUDP>().dynamicObjects.

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
