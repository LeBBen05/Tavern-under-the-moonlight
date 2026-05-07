using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Te_ItemData : ScriptableObject
{
    public string itemName;      // 아이템 이름
    public Sprite icon;          // 인벤토리에 표시될 이미지
    public bool isStackable;     // 중첩 가능 여부 (체크 시 수량 증가)
    [TextArea]
    public string description;   // 아이템 설명
}