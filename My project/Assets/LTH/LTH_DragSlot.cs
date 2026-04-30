using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LTH_DragSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private LTH_Slot sourceSlot;
    private GameObject dragIcon;
    private Image dragImage;

    void Start()
    {
        // 내 오브젝트에 붙어있는 슬롯 정보를 가져옴
        sourceSlot = GetComponent<LTH_Slot>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 1. 슬롯에 아이템이 없으면 드래그 시작 안 함
        if (sourceSlot == null || sourceSlot.itemData == null) return;

        Debug.Log("드래그 시작: " + sourceSlot.itemData.itemName);

        // 2. 마우스를 따라다닐 임시 아이콘 생성
        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(GetComponentInParent<Canvas>().transform);
        dragIcon.transform.SetAsLastSibling(); // 맨 앞에 보이게 설정

        dragImage = dragIcon.AddComponent<Image>();
        dragImage.sprite = sourceSlot.itemData.itemIcon;
        dragImage.raycastTarget = false; // 마우스 클릭 방해 금지

        // 아이콘 크기 조절 (원본 슬롯 크기와 동일하게)
        RectTransform rect = dragIcon.GetComponent<RectTransform>();
        rect.sizeDelta = GetComponent<RectTransform>().sizeDelta;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
        {
            dragIcon.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon != null) Destroy(dragIcon);

        PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        bool isSuccess = false;

        foreach (var result in results)
        {
            // 1. 일단 닿은 곳에서 슬롯을 찾음
            LTH_Slot targetSlot = result.gameObject.GetComponentInParent<LTH_Slot>();

            if (targetSlot != null && targetSlot != sourceSlot)
            {
                if (targetSlot.gameObject.name == "Slot")
                {
                    LTH_Slot parentSlot = targetSlot.transform.parent.GetComponent<LTH_Slot>();
                    if (parentSlot != null) targetSlot = parentSlot;
                }

                targetSlot.UpdateSlot(sourceSlot.itemData, sourceSlot.currentCount);
                isSuccess = true;
                Debug.Log($"{targetSlot.gameObject.name}에 데이터 전달 성공!");
                break;
            }
        }

        if (isSuccess) sourceSlot.UpdateSlot(null, 0);
    }

    // 해당 슬롯이 퀵슬롯 배열에 포함되어 있는지 확인하는 함수
    private bool IsQuickSlot(LTH_Slot slot)
    {
        if (slot == null) return false;
        foreach (var qs in LTH_InventoryManager.Instance.quickSlots)
        {
            if (qs == slot) return true;
        }
        return false;
    }
}