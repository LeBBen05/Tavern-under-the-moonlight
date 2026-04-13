using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CMJCookB : MonoBehaviour
{
    public GameObject AllUI;
    public GameObject CookingButton;
    public GameObject TodaysUI;
    // Start is called before the first frame update
    void Start()
    {
        AllUI.SetActive(false);
        TodaysUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (AllUI.activeSelf || TodaysUI.activeSelf)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
    private void OnMouseDown()
    {
        AllUI.SetActive(true);
        Time.timeScale = 0f;
    }
    public void CMJLoadScene()
    {
        TodaysUI.SetActive(true);
        Time.timeScale = 0f;
    }
    public void CMJLoadScene2()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void CMJLoadScene3()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void OFFUI()
    {
        AllUI.SetActive(false);
        TodaysUI.SetActive(false);
        Time.timeScale = 1f;
    }

}