using UnityEngine;

public class LTH_InventoryTester : MonoBehaviour
{
    // 유니티 인스펙터에서 미리 만든 ItemData 에셋들을 드래그해서 넣어주세요.
    public LTH_ItemData potato;
    public LTH_ItemData seed;

    void Update()
    {
        // [숫자 1] 키를 누르면 감자 1개 획득 (슬롯 자동 생성/증가)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("감자 획득!");
            LTH_InventoryManager.Instance.AddItem(potato, 1);
        }

        // [숫자 2] 키를 누르면 씨앗 5개 획득
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("씨앗 5개 획득!");
            LTH_InventoryManager.Instance.AddItem(seed, 5);
        }

        // [숫자 3] 키를 누르면 감자 1개 감소 (0개가 되면 슬롯 삭제)
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("감자 1개 소비");
            UseItemFromInventory(potato, 1);
        }
    }

    // 인벤토리 매니저의 리스트를 뒤져서 해당 아이템의 수량을 깎는 함수
    void UseItemFromInventory(LTH_ItemData targetItem, int amount)
    {
        foreach (LTH_Slot slot in LTH_InventoryManager.Instance.activeSlots)
        {
            if (slot.itemData == targetItem)
            {
                slot.ChangeCount(-amount); // 음수를 넣어 수량 감소
                break;
            }
        }
    }
}