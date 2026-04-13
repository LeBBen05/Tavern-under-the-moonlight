using UnityEngine;

public class PlayerServing : MonoBehaviour
{
    [Header("서빙 가능 거리")]
    public float servingRange = 1.5f; // 이 거리 안에 들어와야 서빙 가능

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AutoServe();
        }
    }

    void AutoServe()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            Customer customer = hit.collider.GetComponent<Customer>();

            if (customer != null)
            {
                // ★ 핵심: 플레이어와 손님 사이의 거리를 계산합니다.
                // transform.position은 이 스크립트가 붙은 '플레이어'의 위치입니다.
                float distance = Vector2.Distance(transform.position, customer.transform.position);

                if (distance <= servingRange)
                {
                    // 거리가 가까우면 기존 서빙 로직 실행
                    if (HasItemInInventory(customer.requestedItem))
                    {
                        ConsumeItemFromInventory(customer.requestedItem);
                        customer.OnServed(customer.requestedItem);
                    }
                }
                else
                {
                    // 거리가 멀면 경고 로그 (선택 사항)
                    Debug.Log($"<color=yellow>[System]</color> 손님이 너무 멉니다! (현재 거리: {distance:F1})");
                }
            }
        }
    }

    // 인벤토리에 해당 아이템이 있는지 확인하는 함수
    bool HasItemInInventory(ItemData item)
    {
        foreach (var slot in LTH_InventoryManager.Instance.activeSlots)
        {
            if (slot.itemData == item && slot.currentCount > 0)
                return true;
        }
        return false;
    }

    // 인벤토리에서 아이템을 1개 줄이는 함수
    void ConsumeItemFromInventory(ItemData item)
    {
        foreach (var slot in LTH_InventoryManager.Instance.activeSlots)
        {
            if (slot.itemData == item && slot.currentCount > 0)
            {
                slot.ChangeCount(-1); // 1개 소모
                break;
            }
        }
    }
}