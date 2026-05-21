using UnityEngine;
using DG.Tweening;

public class FishPopEffect : MonoBehaviour
{
    [Header("참조")]
    public LTH_PlayerMove playerMove;

    [Header("연출 설정")]
    public float spawnDistance  = 1f;    // 플레이어에서 스폰 거리
    public float jumpHeight     = 1.5f;  // 포물선 높이
    public float forwardDistance = 0.5f; // 착지 위치 (바라본 방향으로 추가 이동)
    public float duration       = 0.8f;  // 전체 연출 시간
    public float fadeTime       = 0.3f;  // 페이드 아웃 시간
    public float iconSize       = 1f;    // 아이콘 크기 (월드 유닛)

    public void PlayFishPop(ItemData caughtFish)
    {
        if (playerMove == null)
        {
            Debug.LogWarning("[FishPopEffect] playerMove가 연결되지 않았습니다.");
            return;
        }

        if (caughtFish == null || caughtFish.itemIcon == null)
        {
            Debug.LogWarning("[FishPopEffect] ItemData 또는 itemIcon이 없습니다.");
            return;
        }

        //  방향 계산
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        Vector2 facing = ((Vector2)mouseWorldPos - (Vector2)playerMove.transform.position).normalized;
        if (facing == Vector2.zero) facing = Vector2.down;

        //  스폰 위치 & 착지 위치 계산 
        Vector3 spawnPos = playerMove.transform.position + (Vector3)(facing * spawnDistance);
        Vector3 landPos  = spawnPos + (Vector3)(facing * forwardDistance);
        spawnPos.z = 0f;
        landPos.z  = 0f;

        // 아이콘 오브젝트 생성
        GameObject fishObj = new GameObject("FishPop");
        fishObj.transform.position = spawnPos;

        SpriteRenderer sr = fishObj.AddComponent<SpriteRenderer>();
        sr.sprite       = caughtFish.itemIcon;
        sr.sortingOrder = 20;

        // 원본 비율 유지하면서 iconSize에 맞게 스케일 조정
        float pixelsPerUnit = caughtFish.itemIcon.pixelsPerUnit;
        float spriteWidth   = caughtFish.itemIcon.rect.width  / pixelsPerUnit;
        float spriteHeight  = caughtFish.itemIcon.rect.height / pixelsPerUnit;
        float maxSide       = Mathf.Max(spriteWidth, spriteHeight);
        float scaleFactor   = (maxSide > 0) ? iconSize / maxSide : 1f;
        fishObj.transform.localScale = Vector3.zero; // 시작은 0 (팡! 등장용)

       
        Sequence seq = DOTween.Sequence();

        // 등장 (스케일 0 → iconSize)
        seq.Append(
            fishObj.transform.DOScale(Vector3.one * scaleFactor, duration * 0.15f)
                   .SetEase(Ease.OutBack)
        );

        //  포물선 점프
        seq.Append(
            fishObj.transform.DOJump(landPos, jumpHeight, 1, duration * 0.65f)
                   .SetEase(Ease.OutQuad)
        );

        // 착지 스쿼시 (납작 → 복귀)
        seq.Append(fishObj.transform.DOScaleY(scaleFactor * 0.5f, 0.07f).SetEase(Ease.OutQuad));
        seq.Append(fishObj.transform.DOScaleY(scaleFactor,        0.07f).SetEase(Ease.OutBounce));

        //  페이드 아웃 후 삭제
        seq.Append(sr.DOFade(0f, fadeTime).SetEase(Ease.InQuad));
        seq.OnComplete(() => Destroy(fishObj));
    }
}
