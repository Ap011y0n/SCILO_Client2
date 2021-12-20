using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gun : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.mousePosition.x > 0 && Input.mousePosition.x < Screen.width && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height)
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            Vector2 mouse = new Vector2(Input.mousePosition.x - screenPos.x, Input.mousePosition.y - screenPos.y);




            Vector2 myPos = new Vector2(1, 0);

            myPos.Normalize();
            float angle = Vector2.Angle(myPos, mouse);

            transform.rotation.eulerAngles.Set(angle, 0, 0);
            int mod = -1;
            if (Input.mousePosition.y - screenPos.y < 0)
                mod = 1;
            transform.rotation = Quaternion.AngleAxis(angle * mod, Vector3.right);
        }
    }
}
