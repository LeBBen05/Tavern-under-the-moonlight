using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CMJCookB : MonoBehaviour
{
    public GameObject AllUI;
    public GameObject CookingButton;
    public GameObject TodaysUI;

    [Header("플레이어 설정")]
    public Transform player;   //플레이어 연결
    public float interactDistance = 3f; //상호작용 거리

    void Start()
    {
        AllUI.SetActive(false);
        TodaysUI.SetActive(false);
    }

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
        if (IsPlayerNear())
        {
            AllUI.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            Debug.Log("너무 멀어서 상호작용 불가");
        }
    }

    public void CMJLoadScene()
    {
        if (IsPlayerNear())
        {
            TodaysUI.SetActive(true);
            Time.timeScale = 0f;
        }
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

    //거리 체크 함수
    bool IsPlayerNear()
    {
        if (player == null) return false;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        return distance <= interactDistance;
    }
}