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

    /// <summary>
    /// 영업을 시작합니다. (처음 장사를 시작할 때 호출)
    /// </summary>
    public void OpenShop()
    {
        isOpen = true;
        Debug.Log("<color=orange>[System]</color> 가게 문을 열었습니다! 손님을 받기 시작합니다.");
    }

    /// <summary>
    /// 영업을 마감합니다. (더 이상 메뉴 추가를 안 할 때 플레이어가 호출)
    /// 이 함수를 호출한 뒤 남은 손님이 다 나가면 결과창이 뜹니다.
    /// </summary>
    public void CloseShop()
    {
        isOpen = false;
        Debug.Log("<color=orange>[System]</color> 마감 준비! 현재 대기열까지만 손님을 받습니다.");

        // 만약 마감을 눌렀는데 이미 손님이 하나도 없다면 바로 결과창 출력
        CheckBusinessEnd();
    }

    /// <summary>
    /// 실시간으로 대기열에 메뉴를 추가합니다.
    /// </summary>
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
        // 대기열에 손님이 있고, 영업 중일 때만 소환 타이머 작동
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

        // 손님이 한 명 나갈 때마다 종료 조건인지 확인
        CheckBusinessEnd();
    }

    private void CheckBusinessEnd()
    {
        // 핵심 조건: 마감 상태(isOpen == false)이고, 대기열이 없고, 맵에 남은 손님도 없을 때
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
    

    /// <summary>
    /// 손님이 음식을 못 받고 그냥 갔을 때, 주문했던 메뉴를 다시 대기열에 넣습니다.
    /// </summary>
    public void ReturnToQueue(ItemData item)
    {
        if (item != null)
        {
            customerQueue.Add(item); // 대기열 맨 뒤에 추가
                                     // ShuffleQueue(); // 필요하다면 다시 섞어줄 수도 있습니다.
            Debug.Log($"<color=yellow>[Refund]</color> {item.itemName} 주문이 대기열로 복구되었습니다. (남은 대기: {customerQueue.Count})");
        }
    }

}