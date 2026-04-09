using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LTH_InventoryManager : MonoBehaviour
{
    public static LTH_InventoryManager Instance;

    [Header("UI Panels")]
    public GameObject totalUIWindow;   // 인벤토리와 레시피를 포함한 전체 부모 창
    public GameObject inventoryContent; // 인벤토리 아이템 리스트 패널
    public GameObject recipeContent;    // 레시피 정보 패널
    public Transform contentParent;     // 아이템 슬롯이 생성될 곳 (Scroll View의 Content)
    public GameObject slotPrefab;

    [Header("Quick Slots (Bottom UI)")]
    public LTH_Slot[] quickSlots;
    public ItemData[] startingTools;

    [Header("Player Visuals")]
    public SpriteRenderer playerHandRenderer;

    public List<LTH_Slot> activeSlots = new List<LTH_Slot>();
    public bool isInventoryOpen = false; // 다른 곳에서 참조할 수 있게 public으로 변경

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 시작할 때 시간은 정상적으로 흐르게 설정
        Time.timeScale = 1f;

        // 시작 시 모든 UI 창을 닫음
        if (totalUIWindow != null) totalUIWindow.SetActive(false);
    }

    void Start()
    {
        InitializeQuickSlots();
    }

    void Update()
    {
        // ESC 키로 전체 UI 창 토글
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleInventory();
        }

        // 창이 닫혀있을 때만 숫자 키로 도구 교체
        if (!isInventoryOpen)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) SelectTool(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SelectTool(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SelectTool(2);
        }
    }

    // --- 탭 전환 기능 ---

    public void ShowInventory()
    {
        if (inventoryContent != null) inventoryContent.SetActive(true);
        if (recipeContent != null) recipeContent.SetActive(false);
        Debug.Log("인벤토리 탭 표시");
    }

    public void ShowRecipe()
    {
        if (inventoryContent != null) inventoryContent.SetActive(false);
        if (recipeContent != null) recipeContent.SetActive(true);
        Debug.Log("레시피 탭 표시");
    }

    // --- 도구 및 인벤토리 로직 ---

    private void InitializeQuickSlots()
    {
        if (quickSlots == null) return;
        for (int i = 0; i < quickSlots.Length; i++)
        {
            if (quickSlots[i] != null && i < startingTools.Length && startingTools[i] != null)
            {
                quickSlots[i].UpdateSlot(startingTools[i], 1);
            }
        }
    }

    public void SelectTool(int index)
    {
        if (quickSlots == null || index < 0 || index >= quickSlots.Length) return;
        if (quickSlots[index] == null || quickSlots[index].itemData == null) return;

        ItemData selectedTool = quickSlots[index].itemData;
        if (playerHandRenderer != null) playerHandRenderer.sprite = selectedTool.itemIcon;

        for (int i = 0; i < quickSlots.Length; i++)
        {
            if (quickSlots[i] == null) continue;
            Image slotBg = quickSlots[i].GetComponent<Image>();
            if (slotBg != null)
                slotBg.color = (i == index) ? Color.white : new Color(0.7f, 0.7f, 0.7f, 0.8f);
        }
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;

        if (totalUIWindow != null)
        {
            totalUIWindow.SetActive(isInventoryOpen);

            if (isInventoryOpen)
            {
                ShowInventory(); // 창을 열 때 기본적으로 인벤토리가 보이게 함
                Time.timeScale = 0f; // 필요하다면 시간 정지 (안 보이던 문제 시 이 줄 삭제)
            }
            else
            {
                Time.timeScale = 1f; // 닫을 때 시간 다시 흐르게
            }
        }
    }

    public void AddItem(ItemData item, int amount)
    {
        if (slotPrefab == null) return;
        foreach (LTH_Slot slot in activeSlots)
        {
            if (slot.itemData == item) { slot.ChangeCount(amount); return; }
        }

        GameObject newSlotObj = Instantiate(slotPrefab, contentParent);
        newSlotObj.transform.SetAsLastSibling();
        LTH_Slot newSlot = newSlotObj.GetComponent<LTH_Slot>();
        if (newSlot != null) { newSlot.UpdateSlot(item, amount); activeSlots.Add(newSlot); }
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent as RectTransform);
    }
    public void RemoveSlotFromList(LTH_Slot slot)
    {
        if (activeSlots.Contains(slot))
        {
            activeSlots.Remove(slot);
        }
    }
}