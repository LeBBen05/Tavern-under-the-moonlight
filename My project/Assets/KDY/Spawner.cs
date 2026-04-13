using UnityEngine;
using System.Collections; // ★ 코루틴(IEnumerator) 사용을 위해 필수!
using System.Collections.Generic; // ★ 리스트(List) 사용을 위해 필수!

/// <summary>
/// 경영 게임의 손님 생성을 담당하는 스포너 클래스입니다.
/// 요리 씬에서 확정된 메뉴 리스트를 받아 무작위 순서로 손님을 소환하고, 영업 종료를 관리합니다.
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

    [Header("영업 종료 설정")]
    public GameObject resultUI;           // 장사 결과창 UI
    [Tooltip("결과창이 몇 초 뒤에 자동으로 꺼질지 설정합니다.")]
    public float resultDisplayTime = 3.0f;

    private List<Customer> activeCustomers = new List<Customer>(); // 현재 맵에 있는 손님 리스트
    private bool isCheckingEnd = false;

    // ==========================================================
    // 1. 대기열 설정 함수 (CMJCookScene에서 호출)
    // ==========================================================

    public void AddToQueue(ItemData menu, int count)
    {
        for (int i = 0; i < count; i++)
        {
            customerQueue.Add(menu);
        }
        Debug.Log($"<color=white>[Queue]</color> {menu.itemName} {count}명 추가 완료.");
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

        timer = spawnInterval; // 섞자마자 첫 손님이 나오도록 설정
        Debug.Log($"<color=yellow>[Spawner]</color> 총 {customerQueue.Count}명의 손님 순서가 랜덤 결정되었습니다!");
    }

    // ==========================================================
    // 2. 소환 로직 (Update & TrySpawn)
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

                // 손님에게 데이터 전달 및 이동 시작
                customer.StartMoving(target, transform.position);
                customer.AssignOrder(orderedItem);

                // ★ 맵에 존재하는 손님 리스트에 등록
                activeCustomers.Add(customer);

                customerQueue.RemoveAt(0);

                Debug.Log($"<color=cyan>[Spawn]</color> 손님 입장! 주문: <b>{orderedItem.itemName}</b> (남은 대기: {customerQueue.Count})");
            }
        }
    }

    // ==========================================================
    // 3. 영업 종료 및 결과창 제어
    // ==========================================================

    /// <summary>
    /// 손님이 퇴장할 때 Customer 스크립트에서 호출합니다.
    /// </summary>
    public void OnCustomerLeave(Customer customer)
    {
        if (activeCustomers.Contains(customer))
        {
            activeCustomers.Remove(customer);
        }

        // 소환할 손님도 없고, 맵에 남은 손님도 없다면 영업 종료!
        if (customerQueue.Count == 0 && activeCustomers.Count == 0)
        {
            ShowResult();
        }
    }

    void ShowResult()
    {
        if (resultUI != null)
        {
            Debug.Log("<color=magenta>[System]</color> 오늘 영업 종료! 결과창을 표시합니다.");
            StopAllCoroutines(); // 혹시 모를 중복 실행 방지
            StartCoroutine(ShowAndHideResult());
        }
    }

    IEnumerator ShowAndHideResult()
    {
        // 1. 결과창 켜기
        resultUI.SetActive(true);

        // 2. 설정된 시간만큼 대기 (예: 3초)
        yield return new WaitForSeconds(resultDisplayTime);

        // 3. 결과창 끄기
        resultUI.SetActive(false);
        Debug.Log("<color=magenta>[System]</color> 결과창이 자동으로 닫혔습니다. 하루가 종료되었습니다.");

        // (선택 사항) 여기에 정산 씬으로 이동하거나 다음 날로 넘어가는 코드를 넣으세요.
        // UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }
}