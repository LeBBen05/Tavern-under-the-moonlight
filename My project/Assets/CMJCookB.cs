using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CMJCookB : MonoBehaviour
{
    public GameObject AllUI;
    public GameObject CookingButton;
    // Start is called before the first frame update
    void Start()
    {
        AllUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        

    }
    private void OnMouseDown()
    {
        AllUI.SetActive(true);
    }
    public void LoadScene()
    {
        SceneManager.LoadScene("TestCookScene");
    }

}