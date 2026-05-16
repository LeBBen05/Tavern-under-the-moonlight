using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 이 스크립트는 아이템들의 데이터에 관한 스크립트 입니다.
/// </summary>
public enum SMS_ItemType
{
    Equipment,     //도구(초기 도구, 인벤토리 0)
    Seed,       // 씨앗 (상점 구매, 인벤토리 O)
    Ingredient, // 식재료 (수확/낚시/상점 구매, 인벤토리 O)
    Recipe      // 레시피 (경영 모드 전용, 인벤토리 X)

}

public enum SMS_FishSize
{ 
    None,
    Small,
    Medium,
    Large
}


[CreateAssetMenu(fileName = " New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("공통 정보")]
    public string ItemID;
    public string itemName; //아이템 이름
    public SMS_ItemType itemType;   //아이템 타입
    public Sprite itemIcon;  //아이템 아이콘
    public bool isStackable; // 중첩 가능 여부 (체크시 수량증가) - 추가자:이태현
    public GameObject dropPrefab;   //드랍될 아이템 프리펩
    public string ItemInfo; //아이템 정보
    

    [Header("상점 구매 설정")]
    public bool isSell = false;
    public int buyPrice;    //구매가
    public int buyAmount = 1;   //구매 양

    [Header("농작물 전용 데이터::Seed")]
    public ItemData yeldItem;   //수확 시 생성되는 아이템
    public int harvestAmount;   //수확 개수
    public int growMinutes;     //성장 시간 

    [Header("물고기 전용 데이터::Fish")]
    public SMS_FishSize fishSize;    //물고기 사이즈
    public float MoveSpeed;   // 물고기가 움직이는 부드러움 (높을수록 빠름)
    public Vector2 WaitTime = new Vector2(); // 목적지 변경 시간 (최소, 최대)

}



