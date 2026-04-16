using UnityEngine;

public class LTH_Crop : MonoBehaviour
{
    private ItemData seedData;
    private float growthTimer;
    private float totalGrowthTime;
    private int currentStep = 0;

    [Header("성장 단계별 이미지")]
    public Sprite[] growthSprites;

    private SpriteRenderer sr;

    // 다 자랐는지 확인하는 프로퍼티
    public bool IsFullyGrown => seedData != null && growthSprites != null && currentStep >= growthSprites.Length - 1;

    public void Initialize(ItemData data)
    {
        seedData = data;
        sr = GetComponent<SpriteRenderer>();

        if (seedData == null) return;

        // 성장 시간 설정 (분 단위 -> 테스트를 위해 아주 빠르게 설정함)
        // 실제 분 단위로 하려면 seedData.growMinutes * 60f 로 바꾸세요.
        totalGrowthTime = seedData.growMinutes * 0.1f;

        if (sr != null && growthSprites != null && growthSprites.Length > 0)
        {
            sr.sprite = growthSprites[0];
            currentStep = 0;
            growthTimer = 0;
        }
    }

    void Update()
    {
        if (seedData == null || sr == null || growthSprites == null || growthSprites.Length < 2 || IsFullyGrown) return;

        growthTimer += Time.deltaTime;
        float timePerStep = totalGrowthTime / (growthSprites.Length - 1);

        if (growthTimer >= timePerStep)
        {
            currentStep++;
            if (currentStep < growthSprites.Length)
            {
                sr.sprite = growthSprites[currentStep];
            }
            growthTimer = 0;
        }
    }

    // 태현님의 ItemData 변수명 yeldItem (오타 포함)에 맞춰 수정함
    public ItemData GetYieldItem() => seedData.yeldItem != null ? seedData.yeldItem : seedData;

    // harvestAmount 사용
    public int GetYieldAmount() => seedData != null ? seedData.harvestAmount : 1;
}