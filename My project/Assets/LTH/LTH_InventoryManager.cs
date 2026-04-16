using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LTH_InventoryManager : MonoBehaviour
{
    public static LTH_InventoryManager Instance;

    [Header("UI Panels")]
    public GameObject inventoryUI;   // 인스펙터에서 InventoryUI 연결
    private bool isInventoryOpen = false;

    [Header("Inventory Settings")]
    public List<LTH_Slot> activeSlots = new List<LTH_Slot>();
    public LTH_Slot[] quickSlots;
    public GameObject slotPrefab;
    public Transform contentParent;

    void Awake()
    {
        // 싱글톤 설정
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        // 리스트 초기화
        if (activeSlots == null) activeSlots = new List<LTH_Slot>();

        //시작하자마자 인벤토리를 확실히 끔
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(false);
            isInventoryOpen = false;
        }

        // 게임 속도 정상화
        Time.timeScale = 1f;
    }

    void Update()
    {
        // ESC 키로 토글
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if (inventoryUI == null) return;

        isInventoryOpen = !isInventoryOpen;
        inventoryUI.SetActive(isInventoryOpen);

        if (isInventoryOpen)
        {
            Time.timeScale = 0f; // 정지
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent as RectTransform);
        }
        else
        {
            Time.timeScale = 1f; // 재개
            // Cursor.visible = false; // 필요 시 주석 해제
        }
    
}

    public void AddItem(ItemData item, int amount)
    {
        if (item == null) return;

        foreach (LTH_Slot slot in activeSlots)
        {
            if (slot != null && slot.itemData == item)
            {
                slot.ChangeCount(amount);
                return;
            }
        }

        if (slotPrefab != null && contentParent != null)
        {
            GameObject newSlotObj = Instantiate(slotPrefab, contentParent);
            LTH_Slot newSlot = newSlotObj.GetComponent<LTH_Slot>();

            if (newSlot != null)
            {
                newSlot.UpdateSlot(item, amount);
                activeSlots.Add(newSlot);
                Canvas.ForceUpdateCanvases();
                LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent as RectTransform);
            }
        }
    }

    public void RemoveSlotFromList(LTH_Slot slot)
    {
        if (activeSlots.Contains(slot))
        {
            activeSlots.Remove(slot);
        }
    }

    public LTH_Slot[] allSlots; // 인스펙터에서 1~8번 슬롯을 드래그해서 넣어둔 배열

    public ItemData GetItemFromSlot(int index)
    {
        // 1. 슬롯 배열이 존재하는지 확인
        if (allSlots == null || index < 0 || index >= allSlots.Length) return null;

        // 2. 해당 슬롯에 아이템이 들어있는지 확인
        if (allSlots[index] != null && allSlots[index].itemData != null)
        {
            return allSlots[index].itemData;
        }

        Debug.LogWarning($"{index}번 슬롯이 비어있습니다.");
        return null;
    }
}