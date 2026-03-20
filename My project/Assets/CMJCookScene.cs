using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CMJCookScene : MonoBehaviour
{
    public GameObject ClickMenuUI; //클릭창
    public bool iSAliveClick = false; //클릭창 상태
    public GameObject AddButton;
    public bool isAliveAdd = false; //클릭창 상태
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ClickMenuUI.SetActive(iSAliveClick);
        AddButton.SetActive(isAliveAdd);

    }
    public void OnMouseDown() //버튼이 눌릴경우 클릭창 활성화
    {
        iSAliveClick = true;
    }
    public void LoadScene() //창아예 닫기(씬 변경)
    {
        if(iSAliveClick == false)
        {
            SceneManager.LoadScene("TestMapScene");
        }
    }
    public void Back() //창 닫기
    {
        iSAliveClick = false;
        if(isAliveAdd == true)
        {
            isAliveAdd = false;
        }
    }
    public void MenuClick()
    {
        isAliveAdd = true;
    }
    public void Add() //메뉴 추가
    {
        iSAliveClick = false;
        isAliveAdd = false;
    }

}