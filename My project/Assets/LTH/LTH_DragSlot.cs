using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LTH_DragSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private LTH_Slot sourceSlot;
    private static GameObject dragIcon; // 드래그 시 마우스를 따라다닐 이미지
    private Canvas canvas;

    void Start()
    {
        sourceSlot = GetComponent<LTH_Slot>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (sourceSlot.itemData == null) return;

        // 드래그용 임시 아이콘 생성
        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(canvas.transform);
        dragIcon.transform.SetAsLastSibling();

        Image img = dragIcon.AddComponent<Image>();
        img.sprite = sourceSlot.itemData.itemIcon;
        img.raycastTarget = false; // 드래그 중인 아이콘이 마우스를 방해하지 않게 함

        // 아이콘 크기 조절
        RectTransform rt = dragIcon.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(50, 50);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
            dragIcon.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon != null) Destroy(dragIcon);

        // 마우스 위치에 있는 슬롯 찾기
        GameObject target = eventData.pointerCurrentRaycast.gameObject;
        if (target != null)
        {
            LTH_Slot targetSlot = target.GetComponent<LTH_Slot>();
            if (targetSlot != null)
            {
                // 데이터 전달: 인벤토리 슬롯 -> 퀵슬롯 (복사)
                targetSlot.UpdateSlot(sourceSlot.itemData, sourceSlot.currentCount);
            }
        }
    }
}