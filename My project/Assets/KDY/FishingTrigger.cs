using UnityEngine;

public class FishingTrigger : MonoBehaviour
{
    [Header("연결할 낚시 캔버스")]
    public GameObject fishingCanvas;

    [Header("낚시터 물고기 목록")]
    public ItemData[] fishPool;

    private void OnMouseDown()
    {
        // 1.이미 낚시 UI가 켜져 있다면 클릭을 무시합니다.
        if (fishingCanvas != null && fishingCanvas.activeSelf)
        {
            Debug.Log("이미 낚시 중입니다!");
            return;
        }

        if (fishingCanvas != null && fishPool.Length > 0)
        {
            int randomIndex = Random.Range(0, fishPool.Length);
            ItemData selectedFish = fishPool[randomIndex];

            // 비활성화된 상태에서도 스크립트를 찾기 위해 (true) 사용
            FishingMinigame game = fishingCanvas.GetComponentInChildren<FishingMinigame>(true);

            if (game != null)
            {
                // 데이터를 먼저 셋팅하고 스크립트를 활성화 준비
                game.currentFishData = selectedFish;
                game.enabled = false; // 확실히 껐다가 아래에서 켭니다.
            }

            // 2. 이제 캔버스를 켜고 게임을 시작합니다.
            fishingCanvas.SetActive(true);
            if (game != null) game.enabled = true;

            Debug.Log($"<color=cyan>{selectedFish.itemName}</color> 입질! 낚시를 시작합니다.");
        }
    }
}