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

        // ★ 이동 중에는 클릭되지 않도록 콜라이더를 끕니다.
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
                CMJNPCEnime npcAnim = GetComponent<CMJNPCEnime>();

                if (npcAnim != null)
                {
                    npcAnim.SetSitting(true);
                }
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
        
        CMJNPCEnime npcAnim = GetComponent<CMJNPCEnime>();

        if (npcAnim != null)
        {
            npcAnim.SetSitting(true);
        }
        // ★ 자리에 앉았을 때만 콜라이더를 켜서 클릭이 가능하게 만듭니다.
        Collider2D col = GetComponent<Collider2D>();

        if (col != null)
        {
            col.enabled = true;
        }

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
        CMJNPCEnime npcAnim = GetComponent<CMJNPCEnime>();

        if (npcAnim != null)
        {
            npcAnim.SetEating(false);
            npcAnim.SetSitting(false);
        }
        // ★ 나갈 때도 중복 클릭 방지를 위해 콜라이더를 끕니다.
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
        if (!isSeated || isServed) return;

        // 플레이어가 서빙한 음식과 손님이 요청한 음식을 비교합니다.
        if (servedItem == requestedItem)
        {
            isServed = true;
            CMJNPCEnime npcAnim = GetComponent<CMJNPCEnime>();

            if (npcAnim != null)
            {
                npcAnim.SetEating(true);
            }
            isSeated = false;
            isWaitingForFood = false;

            Debug.Log("<color=green>[Success]</color> 서빙 성공!");

            if (MoneyManager.Instance != null && requestedItem != null)
            {
                int finalPrice = 0;

                // 프로젝트 내에 존재하는 모든 레시피 데이터들을 싹 불러와서 검색합니다.
                RecipeData[] allRecipes = Resources.FindObjectsOfTypeAll<RecipeData>();
                foreach (RecipeData recipe in allRecipes)
                {
                    // 손님이 먹은 음식 이름(requestedItem.itemName)과 레시피 이름(recipe.recipeName)이 일치하는지 확인
                    // 예: 완성된 음식 이름이 "생선 구이"이고 레시피 이름이 "생선 구이"일 때
                    if (recipe.recipeName == requestedItem.itemName)
                    {
                        finalPrice = recipe.sellPrice; // 레시피에 적힌 12원을 쏙 빼옵니다!
                        break;
                    }
                }

                // 가격을 정상적으로 찾았다면 그만큼 돈을 벌어옵니다!
                if (finalPrice > 0)
                {
                    MoneyManager.Instance.AddMoney(finalPrice);
                    Debug.Log($"<color=lime>[정산]</color> {requestedItem.itemName} 가격 {finalPrice}전 획득!");
                }
                else
                {
                    // 만약 이름이 안 맞거나 레시피를 못 찾으면 버그 방지용으로 기본 10전이라도 줍니다.
                    Debug.LogWarning($"<color=yellow>[경고]</color> {requestedItem.itemName}과 일치하는 레시피 이름을 찾지 못해 기본 가격으로 정산합니다.");
                    MoneyManager.Instance.AddMoney(10);
                }
            }
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

        StopAllCoroutines();
        isWaitingForFood = false;

        if (foodIconImage != null && angryIcon != null)
        {
            foodIconImage.sprite = angryIcon;
        }

        if (speechBubbleCanvas != null) speechBubbleCanvas.SetActive(false);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        StartLeaving();
    }
}