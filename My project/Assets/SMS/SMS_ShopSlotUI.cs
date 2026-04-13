using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SMS_ShopSlotUI : MonoBehaviour
{
    public Image itemiconImage; //아이템 아이콘 이미지
    public Text itemNameText;   //아이템 이름
    public Text itemPriceText;  //아이템 가격
    public Button slotButton;    //아이템 정보 및 구매 화면 생성버튼

    ItemData slotItemData;
    SMS_ShopManager shopManager;


    /// <summary>
    /// 슬롯에 Item데이터를 넣어주고 UI를 업데이트하는 함수
    /// </summary>
    /// <param name="data"></param>
    /// <param name="manager"></param>
    public void SetUpSlot(ItemData data, SMS_ShopManager manager)
    {
        slotItemData = data;
        shopManager = manager;

        itemiconImage.sprite = slotItemData.itemIcon;   //아이템 아이콘
        itemNameText.text = slotItemData.itemName;  //아이템 이름
        itemPriceText.text = slotItemData.buyPrice.ToString() + "전";    //아이템 가격

        //버튼 누르면 구매 함수 실행
        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(OnClickBuy);
    }

    void OnClickBuy()
    {
        shopManager.ShowItemInfo(slotItemData);
    }

}
