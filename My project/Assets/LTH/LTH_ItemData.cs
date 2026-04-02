using System.Net;
using UnityEngine;

[CreateAssetMenu(fileName = "New LTH Item", menuName = "LTH_Inventory/Item")]
public class LTH_ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    [TextArea] public string description;
    public bool isStackable = true; // 농사 게임 특성상 대부분 겹치기 가능
}