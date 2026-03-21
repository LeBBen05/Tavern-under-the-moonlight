using UnityEngine;

public class FishingTrigger : MonoBehaviour
{
    [Header("연결할 낚시 캔버스")]
    public GameObject fishingCanvas;

    [Header("낚시터 물고기 목록")]
    public ItemData[] fishPool; // 인스펙터에서 F_01, F_02 등을 넣어주세요.

    private void OnMouseDown()
    {
        if (fishingCanvas != null && fishPool.Length > 0) // fishPool이 비어있지 않은지 확인
        {
            // 1. [추가] 물고기 목록 중 하나를 랜덤하게 선택합니다.
            int randomIndex = Random.Range(0, fishPool.Length);
            ItemData selectedFish = fishPool[randomIndex];

            // 2. 미니게임 UI를 활성화합니다.
            fishingCanvas.SetActive(true);

            // 3. 미니게임 스크립트를 찾아 정보를 전달합니다.
            FishingMinigame game = fishingCanvas.GetComponentInChildren<FishingMinigame>();
            if (game != null)
            {
                // [추가] 선택된 물고기 데이터를 미니게임 스크립트에 넘겨줍니다.
                game.currentFishData = selectedFish;

                game.enabled = true;
            }

            Debug.Log(selectedFish.itemName + "이(가) 입질을 보냅니다! 낚시 시작!");
        }
        else if (fishPool.Length == 0)
        {
            Debug.LogWarning("Fish Pool이 비어있습니다! 인스펙터에서 물고기 데이터를 넣어주세요.");
        }
    }
}