using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    // ... [기존 변수들 동일] ...
    [Header("이동 및 대기 설정")]
    public float moveSpeed = 3f;
    public float stayTime = 3f;

    [Header("주문 정보 및 UI")]
    public ItemData requestedItem;
    public GameObject speechBubbleCanvas;
    public Image foodIconImage;

    [Header("경로 데이터")]
    private List<Transform> waypoints = new List<Transform>();
    private int currentWaypointIndex = 0;
    private bool isMoving = false;
    private bool isLeaving = false;

    [Header("인내심 설정")]
    public float maxWaitTime = 10f;
    private float currentWaitTimer;
    public Sprite angryIcon;
    private bool isWaitingForFood = false;

    private bool isSeated = false;
    private bool isServed = false; // 서빙 완료 여부 체크
    private Seat assignedSeat;
    private Vector3 spawnPosition;
    private bool isOrderRestored = false;
    public void AssignOrder(ItemData data)
    {
        requestedItem = data;
        if (foodIconImage != null && requestedItem != null)
        {
            foodIconImage.sprite = requestedItem.itemIcon;
        }
    }

    public void StartMoving(Seat targetSeat, Vector3 startPos)
    {
        assignedSeat = targetSeat;
        spawnPosition = startPos;
        waypoints = new List<Transform>(targetSeat.pathToThisSeat);
        waypoints.Add(targetSeat.transform);

        targetSeat.isOccupied = true;
        currentWaypointIndex = 0;
        isMoving = true;
        isLeaving = false;

        // ★ [추가] 이동 중에는 클릭되지 않도록 콜라이더를 끕니다.
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (speechBubbleCanvas != null) speechBubbleCanvas.SetActive(false);
    }

    void Update()
    {
        if (!isMoving) return;

        Vector3 targetPos;
        if (currentWaypointIndex < waypoints.Count)
            targetPos = waypoints[currentWaypointIndex].position;
        else
            targetPos = spawnPosition;

        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPos) < 0.05f)
        {
            if (!isLeaving && currentWaypointIndex == waypoints.Count - 1)
            {
                isMoving = false;
                isSeated = true;
                currentWaypointIndex++;
                StartCoroutine(WaitAtSeat());
                return;
            }

            if (isLeaving && currentWaypointIndex >= waypoints.Count)
            {
                Spawner spawner = FindObjectOfType<Spawner>();
                if (spawner != null) spawner.OnCustomerLeave(this);
                Destroy(gameObject);
                return;
            }
            currentWaypointIndex++;
        }
    }

    IEnumerator WaitAtSeat()
    {
        isWaitingForFood = true;
        currentWaitTimer = maxWaitTime;

        // ★ [추가] 자리에 앉았을 때만 콜라이더를 켜서 클릭이 가능하게 만듭니다.
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        if (speechBubbleCanvas != null) speechBubbleCanvas.SetActive(true);

        while (currentWaitTimer > 0)
        {
            if (!isWaitingForFood) yield break;
            yield return new WaitForSeconds(1f);
            currentWaitTimer -= 1f;
        }

        Debug.Log("<color=red>손님: 너무 오래 걸리네요! 그냥 갑니다!</color>");
        if (!isOrderRestored)
        {
            Spawner spawner = FindObjectOfType<Spawner>();
            if (spawner != null)
            {
                // 여기서 딱 한 명분만 돌려보냅니다.
                spawner.ReturnToQueue(requestedItem);
                isOrderRestored = true; // 방패 활성화!
            }
        }

        if (foodIconImage != null && angryIcon != null)
        {
            foodIconImage.sprite = angryIcon;
        }

        yield return new WaitForSeconds(1.5f);
        if (speechBubbleCanvas != null) speechBubbleCanvas.SetActive(false);
        StartLeaving();
    }

    void StartLeaving()
    {
        // ★ [추가] 나갈 때도 중복 클릭 방지를 위해 콜라이더를 끕니다.
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (assignedSeat != null) assignedSeat.ReleaseSeat();
        waypoints.Remove(assignedSeat.transform);
        waypoints.Reverse();
        currentWaypointIndex = 0;
        isMoving = true;
        isLeaving = true;
    }

    public void OnServed(ItemData servedItem)
    {
        // 이미 1차적으로 콜라이더를 껐다 켰다 하므로 더 안전해졌습니다.
        if (!isSeated || isServed) return;

        if (servedItem == requestedItem)
        {
            isServed = true;
            isSeated = false;
            isWaitingForFood = false;

            Debug.Log("<color=green>[Success]</color> 서빙 성공!");

            CMJCookScene cookScene = FindObjectOfType<CMJCookScene>();
            if (cookScene != null) cookScene.DecreaseMenuCount(requestedItem);

            if (speechBubbleCanvas != null) speechBubbleCanvas.SetActive(false);

            StopAllCoroutines();
            StartLeaving();
        }
        else
        {
            Debug.Log("<color=red>[Fail]</color> 원하던 음식이 아닙니다!");
        }
    }
    /// <summary>
    /// 메뉴가 삭제되었을 때 스포너에 의해 강제로 쫓겨나는 함수입니다.
    /// </summary>
    public void ForceLeave()
    {
        Debug.Log($"<color=orange>[Kick]</color> 내가 시킨 메뉴가 없어졌어! 나갑니다.");

        StopAllCoroutines(); // 기다리는 타이머 중단
        isWaitingForFood = false;

        // 화난 아이콘 표시 (선택 사항)
        if (foodIconImage != null && angryIcon != null)
        {
            foodIconImage.sprite = angryIcon;
        }

        // 말풍선 끄고, 콜라이더 끄기
        if (speechBubbleCanvas != null) speechBubbleCanvas.SetActive(false);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // 밖으로 걸어나감
        StartLeaving();
    }
}