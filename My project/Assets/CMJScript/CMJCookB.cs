using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CMJCookB : MonoBehaviour
{
    public GameObject AllUI;
    public GameObject CookingButton;
    public GameObject TodaysUI;
    public GameObject TalkToHerUI;

    [Header("플레이어 설정")]
    public Transform player;   //플레이어 연결
    public float interactDistance = 3f; //상호작용 거리
    [Header("대화하기 텍스트")]
    public Text TalkToHerText;
    void Start()
    {
        AllUI.SetActive(false);
        TodaysUI.SetActive(false);
        TalkToHerUI.SetActive(false);
    }

    void Update()
    {
        if (AllUI.activeSelf || TodaysUI.activeSelf || TalkToHerUI.activeSelf)
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
    int lastIndex = -1;
    public void CMJTalkToHer()
    {
        TalkToHerUI.SetActive(true);
        Time.timeScale = 0f;

        string[] messages = {
        "안녕하세요",
        "반가워요 :)",
        "날씨가 좋네요 그쵸?",
        "오늘 기분 좋아보이네요",
        "오늘의 기분은 어떠신가요?",
        "또 만났네요!",
        "오늘의 메뉴는 뭔가요?",
        "배가 고프네요",
        "요즘 어떻게 지내시나요?",
        "바쁘신가요?",
        "좋은 하루 되세요!",
        "오늘도 힘내세요!",
        "오늘은 찐감자가 먹고싶네요",
        "오늘은 배추전이 먹고싶네요",
        "요리는 언제나 즐거워~",
        "하하",
        "맛있는 요리는 손님들을 행복하게만들죠",
        "맛의 비결이요? 하하 제 비밀이랍니다~?",
        "음...",
        "제 소원은 돈을 많이 버는 것이에요",
        "랄랄라~",
        "오늘도 바쁘겠는데요?",
        "오늘은 낚시하기 좋은 날이네요",
        "요즘 손님들 입맛이 까다로워요",
        "오늘 메뉴 괜찮은데요?",
        "냄새 좋은데요?"
        };
        int index;
        //같은 번호 방지
        do
        {
            index = UnityEngine.Random.Range(0, messages.Length);
        }
        while (index == lastIndex);
        lastIndex = index;
        TalkToHerText.text = messages[index];
    }
    public void CMJTalkNPCUIoff()
    {
        TalkToHerUI.SetActive(false);
        Time.timeScale = 0f;
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