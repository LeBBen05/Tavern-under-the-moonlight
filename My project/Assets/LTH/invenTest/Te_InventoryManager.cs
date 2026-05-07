using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int count;

    public InventorySlot(ItemData newItem, int newCount)
    {
        item = newItem;
        count = newCount;
    }
}

public class Te_InventoryManager : MonoBehaviour
{
    public static Te_InventoryManager Instance;

    public GameObject inventoryUI;
    private bool isInventoryOpen = false;

    [Header("설정")]
    public int slotCount = 36;       // 총 슬롯 개수
    public GameObject slotPrefab;    // 슬롯 UI 프리팹
    public Transform slotParent;     // Grid Layout Group이 있는 부모

    [Header("데이터")]
    public List<InventorySlot> slots = new List<InventorySlot>();

    private void Awake()
    {
        Instance = this;

        if (inventoryUI != null)
        {
            inventoryUI.SetActive(false);
            isInventoryOpen = false;
        }

        // 게임 시작 시 빈 슬롯 생성 및 데이터 초기화
        for (int i = 0; i < slotCount; i++)
        {
            slots.Add(new InventorySlot(null, 0));

            GameObject newSlot = Instantiate(slotPrefab, slotParent);
            Te_SlotUI slotUI = newSlot.GetComponent<Te_SlotUI>();
            if (slotUI != null)
            {
                slotUI.slotIndex = i; // 각 슬롯에 고유 번호 부여
            }
        }

        Time.timeScale = 1f;
    }

    private void Start()
    {
        //UpdateUI(item, amount);
    }

    void Update()
    {
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
            //LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent as RectTransform);
        }
        else
        {
            Time.timeScale = 1f; // 재개
            // Cursor.visible = false; // 필요 시 주석 해제
        }

    }

    // 아이템 추가 로직
    public void AddItem(ItemData item, int amount)
    {
        // 1. 중첩 가능한 경우: 기존에 같은 아이템이 있는지 확인
        if (item.isStackable)
        {
            foreach (var slot in slots)
            {
                if (slot.item == item)
                {
                    slot.count += amount;
                    UpdateUI(item, amount);
                    return;
                }
            }
        }

        // 2. 새 슬롯에 추가: 빈 칸(item이 null인 곳) 찾기
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].item = item;
                slots[i].count = amount;
                UpdateUI(item, amount);
                return;
            }
        }
    }

    public void RemoveItem(ItemData item, int amount)
    {
        // 인벤토리 슬롯을 뒤져서 해당 아이템이 있는지 확인
        foreach (var slot in slots)
        {
            if (slot.item == item)
            {
                // 아이템이 있다면 개수 감소
                slot.count -= amount;

                // 만약 개수가 0 이하라면 슬롯 비우기
                if (slot.count <= 0)
                {
                    slot.item = null;
                    slot.count = 0;
                }

                UpdateUI(item, amount); // 변경된 내용을 화면에 반영
                return;
            }
        }
        Debug.Log(item.itemName + "이(가) 인벤토리에 없습니다.");
    }

    // 모든 슬롯의 UI를 새로고침
    public void UpdateUI(ItemData item, int amount)
    {
        Te_SlotUI[] uiSlots = slotParent.GetComponentsInChildren<Te_SlotUI>();
        foreach (var ui in uiSlots)
        {
            ui.UpdateSlotUI(item, amount);
        }
    }
}