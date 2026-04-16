using UnityEngine;

public class LTH_InputHandler : MonoBehaviour
{
    public LTH_FarmingManager farmingManager;
    public LTH_InventoryManager inventoryManager; // 태현님의 인벤토리 매니저 연결

    private int selectedSlotIndex = 0;

    void Update()
    {
        // 1. 퀵슬롯 숫자키 입력 (1~8)
        for (int i = 0; i < 8; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                selectedSlotIndex = i;
                Debug.Log($"{i + 1}번 슬롯 선택됨");
            }
        }

        // 2. 마우스 클릭 시 현재 선택된 슬롯의 아이템으로 농사 실행
        if (Input.GetMouseButtonDown(0))
        {
            // 인벤토리 매니저에서 현재 인덱스의 아이템 데이터를 가져옴
            ItemData currentItem = inventoryManager.GetItemFromSlot(selectedSlotIndex);

            // 아이템이 null(빈 손)이어도 일단 실행! (수확을 위해)
            farmingManager.ExecuteInteraction(currentItem);
        }
    }
}