using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SMS_ShopManager : MonoBehaviour
{
    [Header("상점 설정")]
    public List<ItemData> itemSale; //상점에서 팔 아이템 목록을 인스펙터에 넣어주는 역할

    [Header("UI연결")]
    bool isShopOpen;
    public GameObject shopUIPrefeb; //전체 상점 UI
    public Transform slotContainer; //슬롯들이 생성될 부모객체 
    public GameObject shopSlotPrefeb;   //슬롯 프리펩
    public GameObject shopExit;

    //테스트용 소지금
    public int playerMoney = 500;

    // Start is called before the first frame update
    void Start()
    {
        shopUIPrefeb.SetActive(false);  //닫아 두기

        //게임 시작 시 상점 목록 생성
        InitializeShop();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            ToggleShop();
        }

    }



    /// <summary>
    /// 상점 목록 생성
    /// </summary>
    void InitializeShop()
    {
        for (int i = 0; i < itemSale.Count; i++)
        {
            ItemData item = itemSale[i]; // 리스트에서 i번째 아이템을 꺼내옴

            // ItemData에서 isSell이 true인 것만 팔고 싶다면 여기서 체크!
            if (item.isSell)
            {
                GameObject newSlot = Instantiate(shopSlotPrefeb, slotContainer);
                SMS_ShopSlotUI slotUI = newSlot.GetComponent<SMS_ShopSlotUI>();
                if (slotUI != null)
                {
                    slotUI.SetUpSlot(item, this);
                }
            }
        }
    }


    /// <summary>
    /// shop을 클릭하면 shopUI가 나타난다
    /// </summary>
    void ToggleShop()
    {
        isShopOpen = true;
        shopUIPrefeb.SetActive(isShopOpen);

        if (isShopOpen)
        {
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }


    }

    /// <summary>
    /// 상점에서 나가게 하는 버튼 함수
    /// </summary>
    public void ToggleShopExit()
    {
        isShopOpen = false;
        shopUIPrefeb.SetActive(isShopOpen);

        Time.timeScale = 1f;    //시간이 다시 돌아가게 함

    }
    /// <summary>
    /// 아이템을 사는 로직
    /// </summary>
    /// <param name="item">상점 슬롯에 추가될 아이템 데이터</param>
    /// <param name="Initem">인벤토리에 실제로 들어갈 아이템 데이터</param>
    public void BuyItem(ItemData item, LTH_ItemData Initem)
    {
        if (playerMoney >= item.buyPrice)
        {
            playerMoney -= item.buyPrice;

            if (LTH_InventoryManager.Instance != null)
            {
                LTH_InventoryManager.Instance.AddItem(Initem, item.buyAmount);
                Debug.Log($"{item.itemName} 구매 완료! 남은 돈: {playerMoney}");

            }
            else
            {
                Debug.LogError("인벤토리 메니저 인벤토리를 찾을 수 없음");
            }
        }
        else
        {
            Debug.Log("돈이 부족하다, 돌쇠야!");
        }
    }
}
