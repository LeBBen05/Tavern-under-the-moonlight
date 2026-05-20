using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemNotificationPopup : MonoBehaviour
{
    public static ItemNotificationPopup Instance;

    [Header("UI 컴포넌트 연결")]
    public GameObject popupPanel;
    public Image itemIconImage;
    public Text itemText;

    [Header("애니메이션 설정")]
    [Tooltip("팝업이 화면에 머무르는 시간입니다.")]
    public float displayTime = 2.0f;

    [Tooltip("들어오고 나가는 이동 애니메이션의 속도(시간)입니다. 숫자가 낮을수록 '확' 들어옵니다.")]
    public float slideDuration = 0.2f;

    private RectTransform rectTransform;
    private Vector2 onScreenPos;  // 인펙터에서 설정한 화면 내 최종 목적지 좌표 (X: 50)
    private Vector2 offScreenPos; // 화면 왼쪽 밖으로 숨겨질 시작 좌표 (X: -400 등)
    private Coroutine popupAnimationCoroutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (popupPanel != null)
        {
            rectTransform = popupPanel.GetComponent<RectTransform>();

            // 1. 유니티 인스펙터에서 도연님이 세팅해 둔 이쁜 위치를 켜질 때의 목적지로 기억합니다.
            onScreenPos = rectTransform.anchoredPosition;

            // 2. 패널의 가로 크기를 계산해서 화면 왼쪽 바깥 좌표를 자동으로 구합니다.
            float panelWidth = rectTransform.rect.width;
            offScreenPos = new Vector2(-panelWidth - 100f, onScreenPos.y);

            // 3. 시작할 때는 패널을 화면 밖에 배치하고 꺼둡니다.
            rectTransform.anchoredPosition = offScreenPos;
            popupPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 요리/상점/낚시 성공 시 호출되는 핵심 함수
    /// </summary>
    public void TriggerPopup(ItemData item, int count)
    {
        if (item == null || popupPanel == null || rectTransform == null) return;

        // UI 데이터 매핑
        if (itemIconImage != null) itemIconImage.sprite = item.itemIcon;
        if (itemText != null) itemText.text = $"{item.itemName} x{count}";

        // 패널 활성화
        popupPanel.SetActive(true);

        // ★ 연타 예외 처리: 애니메이션 도중 아이템을 또 먹으면, 
        // 기존에 돌던 연출 코루틴을 강제로 끄고 새로운 연출을 실행합니다.
        if (popupAnimationCoroutine != null)
        {
            StopCoroutine(popupAnimationCoroutine);
        }
        popupAnimationCoroutine = StartCoroutine(PlayPopupAnimationSequence());
    }

    /// <summary>
    /// [확 들어오기 -> 대기 -> 슥 사라지기] 연출을 담당하는 시퀀스 코루틴
    /// </summary>
    IEnumerator PlayPopupAnimationSequence()
    {
        // --- 1단계: 왼쪽에서 오른쪽으로 확 들어오기 (Slide In) ---
        // 연타 시 뚝뚝 끊기지 않도록 '현재 위치'에서 목적지까지 부드럽게 이어지게 만듭니다.
        Vector2 currentPos = rectTransform.anchoredPosition;
        float time = 0f;

        while (time < slideDuration)
        {
            time += Time.deltaTime;
            // Lerp를 이용해 정해진 시간 동안 부드럽게 위치 이동
            rectTransform.anchoredPosition = Vector2.Lerp(currentPos, onScreenPos, time / slideDuration);
            yield return null; // 다음 프레임까지 대기
        }
        rectTransform.anchoredPosition = onScreenPos; // 정확한 목적지 안착

        // --- 2단계: 화면에 띄워진 상태로 지정된 시간 동안 대기 (Display) ---
        yield return new WaitForSeconds(displayTime);

        // --- 3단계: 오른쪽에서 다시 왼쪽 화면 밖으로 사라지기 (Slide Out) ---
        currentPos = rectTransform.anchoredPosition;
        time = 0f;

        while (time < slideDuration)
        {
            time += Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.Lerp(currentPos, offScreenPos, time / slideDuration);
            yield return null;
        }
        rectTransform.anchoredPosition = offScreenPos; // 화면 밖 안착

        // --- 4단계: 완전히 숨겨지면 오브젝트 끄기 ---
        popupPanel.SetActive(false);
    }
}