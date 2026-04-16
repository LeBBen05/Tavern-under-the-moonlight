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

    
    public bool IsFullyGrown => seedData != null && growthSprites != null && currentStep >= growthSprites.Length - 1;

    public void Initialize(ItemData data)
    {
        seedData = data;
        sr = GetComponent<SpriteRenderer>();

        if (seedData == null) return;

        
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

    public ItemData GetYieldItem() => seedData.yeldItem != null ? seedData.yeldItem : seedData;

    public int GetYieldAmount() => seedData != null ? seedData.harvestAmount : 1;
}