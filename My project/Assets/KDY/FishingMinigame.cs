using UnityEngine;
using UnityEngine.UI;

public class FishingMinigame : MonoBehaviour
{
    [Header("UI 연결 (Hierarchy에서 드래그)")]
    public RectTransform playerBar;    // PlayerBar 오브젝트
    public RectTransform fishIcon;    // FishIcon 오브젝트
    public Slider successSlider;      // SuccessGauge 오브젝트

    [Header("물리 설정 (조작감)")]
    public float panelHeight = 400f;   // 배경 패널의 세로 높이
    public float gravity = 800f;       // 아래로 떨어지는 힘
    public float jumpForce = 900f;     // 클릭 시 튀어오르는 힘
    public float maxSpeed = 500f;      // 바의 최대 이동 속도 제한

    [Header("난이도 설정 (손맛 조절)")]
    [Range(0.1f, 2f)]
    public float winSpeed = 0.5f;      // 게이지 상승 속도 (높을수록 쉬움)
    [Range(0.1f, 2f)]
    public float loseSpeed = 0.2f;     // 게이지 하락 속도 (낮을수록 쉬움)
    [Range(1.0f, 2.0f)]
    public float hitMargin = 1.2f;     // 판정 범위 배율 (1.2는 바 크기의 120%까지 인정)

    [Header("물고기 AI 설정")]
    public float fishMoveSpeed;   // 물고기가 움직이는 부드러움 (높을수록 빠름)
    public Vector2 fishWaitTime; // 목적지 변경 시간 (최소, 최대)

    [Header("시작 설정")]
    [Range(0f, 1f)]
    public float startGaugeAmount = 0.5f; // 인스펙터에서 슬라이더로 조절 가능

    [Header("현재 잡을 물고기 정보")]
    public ItemData currentFishData; // 팀원이 만든 스크립트 이름이 ItemData라면!

    private float playerBarVelocity;
    private float playerBarPos;
    private float fishPos;
    private float fishDestination;
    private float fishTimer;

    // 이 오브젝트(또는 부모)가 다시 활성화될 때마다 실행됨
    private void OnEnable()
    {
        ///<summary>
        ///
        /// </summary>
        fishMoveSpeed = currentFishData.MoveSpeed;
        fishWaitTime = currentFishData.WaitTime;

        // 1. 게이지 초기화 (아까 설정한 0.5f 또는 startGaugeAmount)
        if (successSlider != null)
        {
            successSlider.value = 0.5f;
        }

        // 2. 플레이어 바 위치 초기화 (바닥으로)
        playerBarPos = playerBar.rect.height / 2;
        playerBarVelocity = 0f;

        // 3. 물고기 위치 및 타이머 초기화
        fishPos = 20f;
        fishTimer = 0f;

        // 4. 스크립트 다시 작동시키기
        this.enabled = true;

        Debug.Log("낚시 준비 완료! 다시 시작합니다.");
    }

    void Start()
    {
        // 시작 시 바닥 위치로 초기화
        playerBarPos = playerBar.rect.height / 2;
        successSlider.value = startGaugeAmount; // 설정한 값으로 시작
    }

    void Update()
    {
        HandlePlayerInput();
        HandleFishAI();
        CheckWinLoss();
    }

    void HandlePlayerInput()
    {
        // 1. 입력 감지 (마우스 왼쪽 버튼 또는 스페이스바)
        if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space))
        {
            playerBarVelocity += jumpForce * Time.deltaTime;
        }
        else
        {
            playerBarVelocity -= gravity * Time.deltaTime;
        }

        // 2. 물리 계산 및 제한
        playerBarVelocity = Mathf.Clamp(playerBarVelocity, -maxSpeed, maxSpeed);
        playerBarPos += playerBarVelocity * Time.deltaTime;

        float halfBar = playerBar.rect.height / 2;
        if (playerBarPos < halfBar) { playerBarPos = halfBar; playerBarVelocity = 0; }
        if (playerBarPos > panelHeight - halfBar) { playerBarPos = panelHeight - halfBar; playerBarVelocity = 0; }

        // 3. UI 반영
        playerBar.anchoredPosition = new Vector2(0, playerBarPos);
    }

    void HandleFishAI()
    {
        fishTimer -= Time.deltaTime;
        if (fishTimer <= 0)
        {
            fishTimer = Random.Range(fishWaitTime.x, fishWaitTime.y);
            fishDestination = Random.Range(15f, panelHeight - 15f);
        }

        // Lerp를 이용해 부드럽게 이동
        fishPos = Mathf.Lerp(fishPos, fishDestination, Time.deltaTime * fishMoveSpeed);
        fishIcon.anchoredPosition = new Vector2(0, fishPos);
    }

    void CheckWinLoss()
    {
        float distance = Mathf.Abs(playerBarPos - fishPos);
        // 판정 범위 계산 (플레이어 바 높이의 절반 * 보너스 배율)
        float threshold = (playerBar.rect.height / 2) * hitMargin;

        if (distance < threshold)
        {
            successSlider.value += winSpeed * Time.deltaTime;
        }
        else
        {
            successSlider.value -= loseSpeed * Time.deltaTime;
        }

        // 결과 처리
        if (successSlider.value >= 1f)
        {
            Debug.Log("<color=green>" + currentFishData.itemName + "을(를) 낚았습니다!</color>");
            FinishGame();
        }
        else if (successSlider.value <= 0f)
        {
            Debug.Log("<color=red>ㅠㅠ 물고기가 도망갔습니다 ㅠㅠ</color>");
            FinishGame(); // 게임 종료 함수 호출
        }
    }
    // 게임을 종료하고 화면을 끄는 함수
    void FinishGame()
    {
        // 1. 현재 스크립트 비활성화 (중복 실행 방지)
        this.enabled = false;

        // 2. 낚시 UI 전체(Canvas)를 찾아 끄기
        // 만약 스크립트가 Panel에 붙어있다면 transform.parent.gameObject를 꺼야 캔버스가 꺼집니다.
        // 가장 확실한 방법은 최상위 부모인 Canvas를 비활성화하는 것입니다.
        transform.root.gameObject.SetActive(false);

        // 참고: transform.root는 계층 구조의 최상위(Canvas)를 찾아줍니다.
    }
}