using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LTH_InventoryManager : MonoBehaviour
{
    // 어디서든 접근 가능한 싱글톤 인스턴스
    public static LTH_InventoryManager Instance;

    [Header("UI Panels")]
    [Tooltip("인벤토리의 전체 부모 오브젝트 (예: Scroll View 또는 Panel)")]
    public GameObject inventoryPanel;

    [Header("Slot Settings")]
    [Tooltip("슬롯들이 배치될 부모 객체 (Scroll View의 Content)")]
    public Transform contentParent;
    [Tooltip("LTH_Slot 스크립트가 붙어있는 슬롯 프리팹")]
    public GameObject slotPrefab;

    [Header("Inventory Data")]
    // 현재 활성화된 모든 슬롯을 관리하는 리스트
    public List<LTH_Slot> activeSlots = new List<LTH_Slot>();

    private bool isInventoryOpen = false;

    void Awake()
    {
        // 싱글톤 초기화
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 게임 시작 시 인벤토리는 닫아둠
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }

    void Update()
    {
        // ESC 키 입력 체크
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleInventory();
            Debug.Log("떴음");
        }
    }

    /// <summary>
    /// 인벤토리를 열거나 닫습니다.
    /// </summary>
    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);

        if (isInventoryOpen)
        {
            // 농사 경영 시뮬레이션: 인벤토리 열릴 때 시간 정지
            Time.timeScale = 0f;
            // 커서 상태 설정 (필요 시)
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            // 인벤토리 닫힐 때 시간 재개
            Time.timeScale = 1f;
        }
    }

    /// <summary>
    /// 아이템을 획득할 때 호출합니다. 
    /// 기존 슬롯이 있으면 수량을 늘리고, 없으면 새 슬롯을 생성합니다.
    /// </summary>
    public void AddItem(LTH_ItemData item, int amount)
    {
        // 1. 이미 같은 아이템이 슬롯에 있는지 확인
        foreach (LTH_Slot slot in activeSlots)
        {
            if (slot.itemData == item)
            {
                slot.ChangeCount(amount);
                return;
            }
        }

        // 2. 없는 아이템이라면 새 슬롯 동적 생성
        GameObject newSlotObj = Instantiate(slotPrefab, contentParent);
        LTH_Slot newSlot = newSlotObj.GetComponent<LTH_Slot>();

        if (newSlot != null)
        {
            newSlot.UpdateSlot(item, amount);
            activeSlots.Add(newSlot);
        }

        // UI 즉시 갱신 (정렬 틀어짐 방지)
        RefreshLayout();
    }

    /// <summary>
    /// 슬롯 수량이 0이 되어 삭제될 때 리스트에서 제거합니다.
    /// </summary>
    public void RemoveSlotFromList(LTH_Slot slot)
    {
        if (activeSlots.Contains(slot))
        {
            activeSlots.Remove(slot);
        }
        // 삭제 후 레이아웃 재정렬
        Invoke("RefreshLayout", 0.05f); // 삭제 프레임 직후 정렬을 위해 약간의 지연
    }

    private void RefreshLayout()
    {
        if (contentParent != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent as RectTransform);
        }
    }
}