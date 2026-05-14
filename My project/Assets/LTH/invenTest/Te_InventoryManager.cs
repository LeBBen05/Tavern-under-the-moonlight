using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public Transform slotParent;     // Grid Layout Group이 있는 부모(Panel)

    [Header("데이터")]
    public List<InventorySlot> slots = new List<InventorySlot>();

    private void Awake()
    {
        Instance = this;

        // 게임 시작 시 UI 초기화
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(false);
            isInventoryOpen = false;
        }

        // 게임 시작 시 빈 데이터 리스트 생성 및 UI 슬롯 생성
        for (int i = 0; i < slotCount; i++)
        {
            // 1. 데이터 리스트에 빈 칸 추가
            slots.Add(new InventorySlot(null, 0));

            // 2. 실제 화면에 보일 슬롯 오브젝트 생성
            GameObject newSlot = Instantiate(slotPrefab, slotParent);

            // 3. 슬롯 UI 스크립트에 인덱스 번호 부여 (매우 중요!)
            Te_SlotUI slotUI = newSlot.GetComponent<Te_SlotUI>();
            if (slotUI != null)
            {
                slotUI.slotIndex = i;
            }
        }

        Time.timeScale = 1f;
    }

    private void Start()
    {
        // 시작 시 한 번 UI를 정리해줍니다.
        UpdateUI();
    }

    void Update()
    {
        // I 키 또는 ESC 키로 인벤토리 열기/닫기
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Escape))
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
            // 인벤토리가 열릴 때 일시정지 (선택 사항)
            // Time.timeScale = 0f; 
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1f;
            // Cursor.visible = false;
        }
    }

    // 아이템 추가 로직
    public void AddItem(ItemData item, int amount)
    {
        // 1. 중첩 가능한 아이템인지 확인 (isStackable 체크박스가 켜져 있어야 함)
        if (item.isStackable)
        {
            foreach (var slot in slots)
            {
                // 슬롯이 비어있지 않고, 슬롯에 담긴 아이템의 ID와 새로 얻은 아이템의 ID가 일치하는지 확인
                if (slot.item != null && slot.item.ItemID == item.ItemID)
                {
                    slot.count += amount; // 기존 수량에 더하기
                    UpdateUI();          // 화면 갱신
                    return;               // 찾았으므로 함수 종료
                }
            }
        }

        // 2. 중첩되지 않거나(isStackable=false), 인벤토리에 같은 아이템이 없는 경우
        // 새로운 빈 칸(null)을 찾아 들어갑니다.
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].item = item;
                slots[i].count = amount;
                UpdateUI();
                return;
            }
        }

        Debug.Log("인벤토리가 가득 찼습니다!");
    }

    // 아이템 제거 로직
    public void RemoveItem(ItemData item, int amount)
    {
        foreach (var slot in slots)
        {
            if (slot.item != null && slot.item.ItemID == item.ItemID)
            {
                slot.count -= amount;

                if (slot.count <= 0)
                {
                    slot.item = null;
                    slot.count = 0;
                }

                UpdateUI(); // UI 새로고침
                return;
            }
        }
        Debug.Log(item.itemName + "이(가) 인벤토리에 없습니다.");
    }

    // 모든 슬롯 UI 새로고침 (매개변수 없이 현재 slots 데이터를 바탕으로 갱신)
    public void UpdateUI()
    {
        Te_SlotUI[] uiSlots = slotParent.GetComponentsInChildren<Te_SlotUI>();
        foreach (var ui in uiSlots)
        {
            ui.UpdateSlotUI();
        }
    }
}