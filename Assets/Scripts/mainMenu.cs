using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    public GameObject mainCamera;
    Vector3 eulerAngles;
    // Start is called before the first frame update
    void Start()
    {
        eulerAngles = mainCamera.transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        mainCamera.transform.eulerAngles = eulerAngles;
        eulerAngles.y += 0.002f;
    }

    public void onClick()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
