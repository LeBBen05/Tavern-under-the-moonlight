using UnityEngine;
using UnityEngine.UI;

public class FishingMinigame : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private RectTransform playerBar;    // 플레이어가 조종하는 바 (Green Bar)
    [SerializeField] private RectTransform fishIcon;    // 추격 대상인 물고기 아이콘 (Red Icon)
    [SerializeField] private Slider successSlider;      // 성공 게이지 슬라이더

    [Header("Physics Settings")]
    public float panelHeight = 1000f;   // 미니게임 배경 패널의 세로 길이
    public float gravity = 800f;       // 아래로 떨어지는 중력 수치
    public float jumpForce = 900f;     // 클릭 시 발생하는 상승 수치
    public float maxSpeed = 500f;      // 이동 속도의 최대 제한값

    [Header("Difficulty Settings")]
    [Range(0.1f, 2f)] public float winSpeed = 0.5f;     // 바 안에 있을 때 게이지 상승 속도
    [Range(0.1f, 2f)] public float loseSpeed = 0.2f;    // 바 밖에 있을 때 게이지 하락 속도
    [Range(1.0f, 2.0f)] public float hitMargin = 1.2f;  // 판정 범위 배율 (기본 바 크기의 n배)

    [Header("Player Control")]
    [Tooltip("낚시 종료 후 다시 활성화할 플레이어 이동 스크립트")]
    public LTH_PlayerMove playerMovement;

    [Header("Fish AI (Loaded from ItemData)")]
    public float fishMoveSpeed;         // 물고기의 민첩성 (이동 속도)
    public Vector2 fishWaitTime;        // 다음 목적지로 이동할 때까지의 대기 시간(Min, Max)

    [Header("Game State")]
    public float startGaugeAmount = 0.5f; // 게임 시작 시 초기 게이지 위치
    public ItemData currentFishData;      // 현재 낚시 중인 물고기 정보 

    private float playerBarVelocity;
    private float playerBarPos;
    private float fishPos;
    private float fishDestination;
    private float fishTimer;

    /// <summary>
    /// 게임 오브젝트가 활성화될 때마다 실행되는 초기화 함수.
    /// </summary>
    private void OnEnable()
    {
        // 1. 데이터 연동 및 방어 코드 
        if (currentFishData != null)
        {
         
            fishMoveSpeed = currentFishData.MoveSpeed;
            fishWaitTime = currentFishData.WaitTime;
            Debug.Log($"<color=cyan>{currentFishData.itemName}</color> 데이터를 로드했습니다.");
        }
        else
        {
            // 데이터가 없을 경우 에러 방지를 위해 기본값 할당
            Debug.LogWarning("전달된 물고기 데이터가 없어 기본 난이도로 시작합니다.");
            fishMoveSpeed = 3f;
            fishWaitTime = new Vector2(0.5f, 2.0f);
        }

        // 2. 물리 및 게이지 상태 리셋
        ResetGame();

        this.enabled = true;
    }

    private void Start()
    {
        ResetGame();
    }

    private void Update()
    {
        HandlePlayerInput();
        HandleFishAI();
        CheckWinLoss();
    }

    /// <summary>
    /// 플레이어의 키 입력(마우스, 스페이스바)에 따른 바의 움직임을 제어합니다.
    /// </summary>
    void HandlePlayerInput()
    {
        // 1. 입력 감지 및 속도 계산
        if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space))
        {
            // deltaTime을 한 번만 곱해서 가속도를 더 확실하게 줍니다.
            playerBarVelocity += jumpForce * Time.deltaTime * 10f;
        }
        else
        {
            playerBarVelocity -= gravity * Time.deltaTime * 10f;
        }

        // 2. 물리 계산 및 제한
        playerBarVelocity = Mathf.Clamp(playerBarVelocity, -maxSpeed, maxSpeed);
        playerBarPos += playerBarVelocity * Time.deltaTime;

        // 경계 제한 (패널 밖으로 나가지 않게)
        float halfBar = playerBar.rect.height / 2;
        playerBarPos = Mathf.Clamp(playerBarPos, halfBar, panelHeight - halfBar);

        playerBar.anchoredPosition = new Vector2(0, playerBarPos);
    }

    /// <summary>
    /// 물고기가 일정 시간마다 새로운 위치로 부드럽게 이동하게 만드는 AI 로직입니다.
    /// </summary>
    void HandleFishAI()
    {
        fishTimer -= Time.deltaTime;
        if (fishTimer <= 0)
        {
            fishTimer = Random.Range(fishWaitTime.x, fishWaitTime.y);
            fishDestination = Random.Range(15f, panelHeight - 15f);
        }

        // Lerp를 사용하여 지정된 속도만큼 부드럽게 이동
        fishPos = Mathf.Lerp(fishPos, fishDestination, Time.deltaTime * fishMoveSpeed);
        fishIcon.anchoredPosition = new Vector2(0, fishPos);
    }

    /// <summary>
    /// 플레이어 바와 물고기의 거리를 체크하여 성공/실패 여부를 판단합니다.
    /// </summary>
    void CheckWinLoss()
    {
        float distance = Mathf.Abs(playerBarPos - fishPos);
        float threshold = (playerBar.rect.height / 2) * hitMargin;

        if (distance < threshold)
            successSlider.value += winSpeed * Time.deltaTime;
        else
            successSlider.value -= loseSpeed * Time.deltaTime;

        // 최종 판정
        if (successSlider.value >= 1f)
        {
            string name = currentFishData != null ? currentFishData.itemName : "물고기";
            Debug.Log($"<color=green>{name} 포획 성공!</color>");
            FinishGame();
        }
        else if (successSlider.value <= 0f)
        {
            Debug.Log("<color=red>물고기를 놓쳤습니다.</color>");
            FinishGame();
        }
    }

    /// <summary>
    /// 게임 시작 시 필요한 모든 수치를 초기 상태로 되돌리고 UI에 즉시 반영합니다.
    /// </summary>
    void ResetGame()
    {
        if (successSlider != null) successSlider.value = startGaugeAmount;

        // 1. 위치 수치 초기화
        playerBarPos = playerBar.rect.height / 2;
        playerBarVelocity = 0f;
        fishPos = 20f;
        fishTimer = 0f;

        // 2. ★ 중요: 초기화된 위치를 실제 UI 좌표에 즉시 적용
        // 이 코드가 없으면 첫 프레임에서 바가 0,0 좌표에 박혀있을 수 있습니다.
        playerBar.anchoredPosition = new Vector2(0, playerBarPos);
        fishIcon.anchoredPosition = new Vector2(0, fishPos);
    }

    /// <summary>
    /// 게임을 종료하고 미니게임 캔버스를 비활성화합니다.
    /// </summary>
    void FinishGame()
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
            Debug.Log("<color=orange>[System]</color> 플레이어 이동이 다시 활성화되었습니다.");
        }

        this.enabled = false;
        transform.root.gameObject.SetActive(false);

    }
}