using UnityEngine;

/// <summary>
/// 경영 게임의 손님 생성을 담당하는 스포너 클래스입니다.
/// 일정 시간마다 빈 좌석을 확인하고 손님을 소환합니다.
/// </summary>
public class Spawner : MonoBehaviour
{
    [Header("소환 설정")]
    [Tooltip("생성할 손님 캐릭터의 프리팹")]
    public GameObject customerPrefab;

    [Tooltip("맵의 좌석 상태를 관리하는 매니저")]
    public SeatManager seatManager;

    [Range(1f, 10f)]
    [Tooltip("손님이 소환되는 간격 (초 단위)")]
    public float spawnInterval = 4f;

    [Header("상태 확인")]
    private float timer = 0f; // 소환 타이머

    // 매 프레임마다 시간을 체크하여 소환 주기를 관리합니다.
    void Update()
    {
        timer += Time.deltaTime; // 시간 누적

        // 설정한 소환 간격보다 시간이 지났을 때
        if (timer >= spawnInterval)
        {
            timer = 0f;    // 타이머 초기화
            TrySpawn();    // 소환 시도
        }
    }

    /// <summary>
    /// 빈 자리가 있는지 확인하고 손님을 생성하는 핵심 로직입니다.
    /// </summary>
    void TrySpawn()
    {
        // 1. 좌석 매니저에게 현재 비어있는 좌석이 있는지 물어봅니다.
        Seat target = seatManager.GetAvailableSeat();

        // 2. 만약 비어있는 자리가 있다면 소환 절차를 진행합니다.
        if (target != null)
        {
            // 3. 현재 스포너(입구) 위치에 손님 프리팹을 생성합니다.
            GameObject go = Instantiate(customerPrefab, transform.position, Quaternion.identity);

            // 4. 생성된 손님 오브젝트에서 Customer 스크립트를 가져옵니다.
            Customer customer = go.GetComponent<Customer>();

            // 5. 손님이 정상적으로 생성되었다면 이동 명령을 내립니다.
            if (customer != null)
            {
                // [목적지 좌석]과 [되돌아올 스포너 위치] 정보를 함께 전달합니다.
                customer.StartMoving(target, transform.position);
            }
        }
        else
        {
            // 자리가 없을 경우에 대한 예외 처리 (필요시 로그 출력)
            // Debug.Log("현재 모든 좌석이 만석이라 손님이 방문하지 못했습니다.");
        }
    }
}