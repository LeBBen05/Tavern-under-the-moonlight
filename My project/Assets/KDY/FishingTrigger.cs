using UnityEngine;
using UnityEngine.Tilemaps;

public class FishingTrigger : MonoBehaviour
{
    [Header("필수 연결")]
    public Tilemap fishingTilemap; // 여기에 FishingDataMap 연결
    public GameObject fishingCanvas;
    public ItemData[] fishPool;

    [Header("플레이어 제어")]
    public LTH_PlayerMove playerMovement;

    [Header("거리 제한 설정")]
    [Tooltip("낚시가 가능한 최대 거리입니다. (적정값: 1.5 ~ 2.5)")]
    public float fishingRange = 2.0f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 1. 마우스 클릭 위치를 월드 좌표로 변환
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f; // 2D 거리 계산을 위해 Z축 고정

            // 2. 타일맵 칸 좌표 변환
            Vector3Int cellPosition = fishingTilemap.WorldToCell(mouseWorldPos);

            // 3. 해당 칸에 타일이 있는지 확인
            if (fishingTilemap.HasTile(cellPosition))
            {
                // 4. ★ 핵심: 플레이어와 클릭 지점 사이의 거리 계산
                if (playerMovement != null)
                {
                    float distance = Vector2.Distance(playerMovement.transform.position, mouseWorldPos);

                    if (distance <= fishingRange)
                    {
                        // 범위 안이라면 낚시 시작!
                        TryStartFishing();
                    }
                    else
                    {
                        // 너무 멀다면 로그 출력
                        Debug.Log($"<color=orange>[Fishing]</color> 물가가 너무 멉니다! (현재 거리: {distance:F1} / 제한: {fishingRange})");
                    }
                }
            }
        }
    }

    private void TryStartFishing()
    {
        if (fishingCanvas != null && fishingCanvas.activeSelf)
        {
            Debug.Log("이미 낚시 중입니다!");
            return;
        }

        if (fishingCanvas != null && fishPool.Length > 0)
        {
            int randomIndex = Random.Range(0, fishPool.Length);
            ItemData selectedFish = fishPool[randomIndex];

            FishingMinigame game = fishingCanvas.GetComponentInChildren<FishingMinigame>(true);

            if (game != null)
            {
                game.currentFishData = selectedFish;
                game.enabled = false;
            }

            fishingCanvas.SetActive(true);
            if (game != null) game.enabled = true;

            // 플레이어 이동 정지 및 물리 속도 초기화
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
                Rigidbody2D rb = playerMovement.GetComponent<Rigidbody2D>();
                if (rb != null) rb.velocity = Vector2.zero;
            }

            Debug.Log($"<color=cyan>{selectedFish.itemName}</color> 입질! 낚시를 위해 이동을 중지합니다.");
        }
    }
}