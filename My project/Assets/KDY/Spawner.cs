using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 경영 게임의 손님 생성을 담당하는 스포너 클래스입니다.
/// 실시간 메뉴 추가를 지원하며, 마감 버튼을 누르기 전까지 영업을 지속합니다.
/// </summary>
public class Spawner : MonoBehaviour
{
    [Header("소환 설정")]
    public GameObject customerPrefab;
    public SeatManager seatManager;

    [Range(1f, 10f)]
    public float spawnInterval = 4f;

    [Header("소환 대기열")]
    public List<ItemData> customerQueue = new List<ItemData>();
    private float timer = 0f;

    [Header("영업 상태 제어")]
    [Tooltip("이 값이 true면 손님이 없어도 영업이 종료되지 않습니다.")]
    public bool isOpen = true;

    [Header("영업 종료 설정")]
    public GameObject resultUI;
    [Tooltip("결과창이 몇 초 뒤에 자동으로 꺼질지 설정합니다.")]
    public float resultDisplayTime = 3.0f;

    private List<Customer> activeCustomers = new List<Customer>();

    // ==========================================================
    // 1. 대기열 및 영업 상태 관리
    // ==========================================================

    public void OpenShop()
    {
        isOpen = true;
        Debug.Log("<color=orange>[System]</color> 가게 문을 열었습니다! 손님을 받기 시작합니다.");
    }

    public void CloseShop()
    {
        isOpen = false;
        Debug.Log("<color=orange>[System]</color> 마감 준비! 현재 대기열까지만 손님을 받습니다.");
        CheckBusinessEnd();
    }

    public void AddToQueue(ItemData menu, int count)
    {
        for (int i = 0; i < count; i++)
        {
            customerQueue.Add(menu);
        }
        Debug.Log($"<color=white>[Queue]</color> {menu.itemName} {count}명 추가 완료. (현재 대기: {customerQueue.Count})");
    }

    public void ShuffleQueue()
    {
        if (customerQueue.Count <= 1) return;

        for (int i = 0; i < customerQueue.Count; i++)
        {
            ItemData temp = customerQueue[i];
            int randomIndex = Random.Range(i, customerQueue.Count);
            customerQueue[i] = customerQueue[randomIndex];
            customerQueue[randomIndex] = temp;
        }

        Debug.Log($"<color=yellow>[Spawner]</color> 대기열 순서가 무작위로 섞였습니다!");
    }

    // ==========================================================
    // 2. 소환 로직
    // ==========================================================

    void Update()
    {
        if (customerQueue.Count > 0)
        {
            timer += Time.deltaTime;

            if (timer >= spawnInterval)
            {
                timer = 0f;
                TrySpawn();
            }
        }
    }

    void TrySpawn()
    {
        Seat target = seatManager.GetAvailableSeat();

        if (target != null)
        {
            GameObject go = Instantiate(customerPrefab, transform.position, Quaternion.identity);
            Customer customer = go.GetComponent<Customer>();

            if (customer != null)
            {
                ItemData orderedItem = customerQueue[0];
                customer.StartMoving(target, transform.position);
                customer.AssignOrder(orderedItem);

                activeCustomers.Add(customer);
                customerQueue.RemoveAt(0);

                Debug.Log($"<color=cyan>[Spawn]</color> 손님 입장! 주문: <b>{orderedItem.itemName}</b>");
            }
        }
    }

    // ==========================================================
    // 3. 영업 종료 로직
    // ==========================================================

    public void OnCustomerLeave(Customer customer)
    {
        if (activeCustomers.Contains(customer))
        {
            activeCustomers.Remove(customer);
        }

        CheckBusinessEnd();
    }

    private void CheckBusinessEnd()
    {
        if (!isOpen && customerQueue.Count == 0 && activeCustomers.Count == 0)
        {
            ShowResult();
        }
    }

    void ShowResult()
    {
        if (resultUI != null && !resultUI.activeSelf)
        {
            Debug.Log("<color=magenta>[System]</color> 오늘 영업이 완전히 종료되었습니다.");
            StopAllCoroutines();
            StartCoroutine(ShowAndHideResult());
        }
    }

    IEnumerator ShowAndHideResult()
    {
        resultUI.SetActive(true);
        yield return new WaitForSeconds(resultDisplayTime);
        resultUI.SetActive(false);
        Debug.Log("<color=magenta>[System]</color> 결과창이 닫혔습니다.");
    }

    // ==========================================================
    // 4. 주문 복구 및 취소 (메뉴 삭제 관련)
    // ==========================================================

    public void ReturnToQueue(ItemData item)
    {
        if (item != null)
        {
            customerQueue.Add(item);
            Debug.Log($"<color=yellow>[Refund]</color> {item.itemName} 주문 복구. (남은 대기: {customerQueue.Count})");
        }
    }

    /// <summary>
    /// 대기열에서 특정 메뉴와 관련된 모든 주문을 삭제합니다. (호출 시 맵에 있는 손님은 놔둠)
    /// </summary>
    public void RemoveAllFromQueue(ItemData item)
    {
        if (item == null) return;
        customerQueue.RemoveAll(x => x == item);
        Debug.Log($"<color=red>[Cancel]</color> {item.itemName} 대기 손님 삭제 완료.");
    }

    /// <summary>
    /// ★ [추가] 특정 메뉴를 주문한 대기열 손님 삭제 + 이미 맵에 들어온 손님 강제 퇴장!
    /// </summary>
    public void ForceRemoveMenu(ItemData item)
    {
        if (item == null) return;

        // 1. 아직 소환 안 된 입구 밖 대기열 손님 삭제 (메모리 주소 대신 이름으로 안전하게 비교)
        int removedCount = customerQueue.RemoveAll(x => x.itemName == item.itemName);
        Debug.Log($"<color=red>[Cancel]</color> {item.itemName} 대기 손님 {removedCount}명 삭제 완료.");

        // 2. 이미 맵에 들어온 손님 찾아서 쫓아내기
        List<Customer> customersToKick = new List<Customer>();

        foreach (var customer in activeCustomers)
        {
            if (customer != null && customer.requestedItem != null)
            {
                // 이름으로 비교해서 같은 메뉴를 시킨 손님 색출
                if (customer.requestedItem.itemName == item.itemName)
                {
                    customersToKick.Add(customer);
                }
            }
        }

        // 3. 색출한 손님들 강제 퇴장 처리
        foreach (var customer in customersToKick)
        {
            customer.ForceLeave();
        }
    }
}