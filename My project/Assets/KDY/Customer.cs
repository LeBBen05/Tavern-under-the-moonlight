using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 손님의 이동, 대기, 퇴장 로직을 관리하는 AI 클래스입니다.
/// </summary>
public class Customer : MonoBehaviour
{
    [Header("이동 및 대기 설정")]
    [Tooltip("손님의 이동 속도")]
    public float moveSpeed = 3f;
    [Tooltip("좌석에 머무는 시간 (초)")]
    public float stayTime = 3f;

    [Header("경로 데이터")]
    private List<Transform> waypoints = new List<Transform>(); // 이동할 지점 리스트
    private int currentWaypointIndex = 0; // 현재 목표로 하는 지점의 번호
    private bool isMoving = false;         // 현재 이동 중인지 여부
    private bool isLeaving = false;        // 퇴장 중인지 여부 (True면 스포너로 이동)

    private Seat assignedSeat;      // 배정받은 좌석 정보
    private Vector3 spawnPosition;  // 소멸될 위치 (처음 소환된 스포너 위치)

    /// <summary>
    /// 스포너에 의해 호출되어 손님의 목적지와 경로를 초기화합니다.
    /// </summary>
    /// <param name="targetSeat">배정된 좌석</param>
    /// <param name="startPos">되돌아올 스포너의 위치</param>
    public void StartMoving(Seat targetSeat, Vector3 startPos)
    {
        assignedSeat = targetSeat;
        spawnPosition = startPos;

        // 1. 경로 설정: [좌석 전용 길목들]을 복사한 뒤 마지막에 [실제 좌석]을 추가
        waypoints = new List<Transform>(targetSeat.pathToThisSeat);
        waypoints.Add(targetSeat.transform);

        // 2. 상태 설정
        targetSeat.isOccupied = true; // 좌석 선점
        currentWaypointIndex = 0;     // 첫 번째 길목부터 시작
        isMoving = true;              // 이동 시작
        isLeaving = false;            // 입장 상태
    }

    void Update()
    {
        // 이동 상태가 아니면 로직을 실행하지 않음
        if (!isMoving) return;

        // [STEP 1] 현재 이동해야 할 목표 지점(targetPos) 결정
        Vector3 targetPos;
        if (currentWaypointIndex < waypoints.Count)
        {
            // 아직 거쳐야 할 웨이포인트(길목/좌석)가 남은 경우
            targetPos = waypoints[currentWaypointIndex].position;
        }
        else
        {
            // 모든 웨이포인트를 다 돌았다면 최종적으로 스포너(입구) 위치로 향함
            targetPos = spawnPosition;
        }

        // [STEP 2] 목표 지점을 향해 이동
        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        // [STEP 3] 목표 지점에 도착했는지 체크 (오차범위 0.05 이내)
        if (Vector2.Distance(transform.position, targetPos) < 0.05f)
        {
            // CASE [A]: 입장 중이고, 방금 도착한 곳이 '좌석'인 경우 (리스트의 마지막)
            if (!isLeaving && currentWaypointIndex == waypoints.Count - 1)
            {
                isMoving = false;           // 이동을 멈추고
                currentWaypointIndex++;     // 인덱스를 미리 올려둠 (다음 목적지는 스포너)
                StartCoroutine(WaitAtSeat()); // 좌석 대기 코루틴 실행
                return;
            }

            // CASE [B]: 퇴장 중이고, 방금 도착한 곳이 '스포너 위치'인 경우
            if (isLeaving && currentWaypointIndex >= waypoints.Count)
            {
                Destroy(gameObject); // 객체 삭제 (퇴장 완료)
                return;
            }

            // CASE [C]: 다음 길목으로 이동해야 하는 일반적인 상황
            currentWaypointIndex++;
        }
    }

    /// <summary>
    /// 좌석에서 일정 시간 동안 머무르는 로직입니다.
    /// </summary>
    IEnumerator WaitAtSeat()
    {
        Debug.Log("손님: 좌석에 도착했습니다. 식사를 시작합니다.");

        // 설정된 시간(stayTime)만큼 대기
        yield return new WaitForSeconds(stayTime);

        Debug.Log("손님: 식사 완료! 이제 돌아갑니다.");
        StartLeaving(); // 퇴장 시작
    }

    /// <summary>
    /// 퇴장 상태로 전환하고 나가는 경로를 설정합니다.
    /// </summary>
    void StartLeaving()
    {
        // 1. 점유했던 좌석 비우기
        if (assignedSeat != null) assignedSeat.ReleaseSeat();

        // 2. 경로 반전: 현재 있는 [좌석]은 제외하고, 들어올 때 쓴 [길목]들만 뒤집음
        waypoints.Remove(assignedSeat.transform);
        waypoints.Reverse();

        // 3. 퇴장 상태 설정
        currentWaypointIndex = 0;
        isMoving = true;
        isLeaving = true;
    }
}