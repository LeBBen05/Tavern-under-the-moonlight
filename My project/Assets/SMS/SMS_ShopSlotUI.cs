using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SMS_ShopSlotUI : MonoBehaviour
{
    public Image itemiconImage; //아이템 아이콘 이미지
    public Text itemName;   //아이템 이름
    public Text itemPrice;  //아이템 가격
    public Button buyButton;    //아이템 구매 버튼

    ItemData slotItemData;
    SMS_ShopManager shopManager;
    LTH_ItemData ItemData;

    /// <summary>
    /// 슬롯에 Item데이터를 넣어주고 UI를 업데이트하는 함수
    /// </summary>
    /// <param name="data"></param>
    /// <param name="manager"></param>
    public void SetUpSlot(ItemData data, SMS_ShopManager manager)
    {
        itemiconImage.sprite = slotItemData.itemIcon;   //아이템 아이콘
        itemName.text = slotItemData.itemName;  //아이템 이름
        itemPrice.text = slotItemData.buyPrice.ToString() + "전";    //아이템 가격

        //버튼 누르면 구매 함수 실행
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnClickBuy);
    }

    void OnClickBuy()
    {
        shopManager.BuyItem(slotItemData, ItemData);
    }

}
