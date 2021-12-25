using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{
    public GameObject player = null;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        else
        {
            Vector3 pos = player.transform.position;
            pos.x += 10;
            transform.position = pos;
        }
    }
}
