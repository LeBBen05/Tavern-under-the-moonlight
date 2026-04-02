using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MenuImageGroup
{
    public GameObject[] images;
}

public class CMJCookScene : MonoBehaviour
{
    public GameObject ClickMenuUI;
    public GameObject AddButton;

    public bool iSAliveClick = false;
    public bool isAliveAdd = false;

    public int currentMenuIndex = -1;

    public MenuImageGroup[] menuImageGroups;

    void Start()
    {
        HideAllImages();
    }

    void Update()
    {
        ClickMenuUI.SetActive(iSAliveClick);
        AddButton.SetActive(isAliveAdd);
    }

    void HideAllImages()
    {
        foreach (var group in menuImageGroups)
        {
            foreach (var img in group.images)
            {
                if (img != null) // null 체크 추가
                {
                    img.SetActive(false);
                }
            }
        }
    }

    public void OnMouseDown()
    {
        iSAliveClick = true;
    }

    public void LoadScene()
    {
        if (!iSAliveClick)
        {
            SceneManager.LoadScene("TestMapScene");
        }
    }

    public void Back()
    {
        iSAliveClick = false;
        isAliveAdd = false;
        currentMenuIndex = -1;

        HideAllImages();
    }

    // 메뉴 클릭
    public void MenuClick(int index)
    {
        Debug.Log("=== 메뉴 클릭: " + index + " ===");

        //index 체크
        if (index < 0 || index >= menuImageGroups.Length)
        {
            Debug.LogError("index 범위 초과!");
            return;
        }

        //같은 메뉴 다시 누르면 → 끄기
        if (currentMenuIndex == index)
        {
            Debug.Log("같은 메뉴 다시 클릭 → 끄기");

            HideAllImages();
            currentMenuIndex = -1;
            isAliveAdd = false;

            return;
        }

        //다른 메뉴 클릭
        isAliveAdd = true;
        //기존 이미지 끄기
        HideAllImages();
        //현재 메뉴 저장
        currentMenuIndex = index;
        //선택된 메뉴 이미지 켜기
        foreach (var img in menuImageGroups[index].images)
        {
            if (img != null)
            {
                img.SetActive(true);
                Debug.Log("켜짐: " + img.name);
            }
        }

    }

    public void Add()
    {
        if (currentMenuIndex < 0) return;

        iSAliveClick = false;
        isAliveAdd = false;
    }
}