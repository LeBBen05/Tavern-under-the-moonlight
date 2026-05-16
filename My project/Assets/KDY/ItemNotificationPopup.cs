using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 아이템을 획득하거나 구매했을 때 화면 왼쪽에 잠깐 나타나는 
/// 토스트 알림 팝업을 제어하는 클래스입니다.
/// </summary>
public class ItemNotificationPopup : MonoBehaviour
{
    // 어디서나 한 줄로 편하게 부를 수 있도록 싱글톤 인스턴스 생성
    public static ItemNotificationPopup Instance;

    [Header("UI 컴포넌트 연결")]
    [Tooltip("껐다 켰다 할 부모 패널(PopupPanel)을 넣어주세요.")]
    public GameObject popupPanel;

    [Tooltip("아이템 아이콘이 들어갈 Image 컴포넌트를 넣어주세요.")]
    public Image itemIconImage;

    [Tooltip("아이템 이름과 개수가 적힐 Text 컴포넌트를 넣어주세요.")]
    public Text itemText;          // 만약 TextMeshPro를 쓰신다면 TMP_Text로 바꾸시면 됩니다!

    [Header("설정")]
    [Tooltip("팝업이 화면에 유지될 시간(초)입니다.")]
    public float displayTime = 2.0f;

    private Coroutine hideCoroutine;

    void Awake()
    {
        // 싱글톤 세팅
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // ★ 게임이 시작할 때는 평소대로 화면에서 안 보이게 꺼둡니다.
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 아이템을 사거나 요리해서 획득했을 때 외부 스크립트에서 호출하는 함수입니다.
    /// 예: ItemNotificationPopup.Instance.TriggerPopup(item, 5);
    /// </summary>
    public void TriggerPopup(ItemData item, int count)
    {
        if (item == null || popupPanel == null) return;

        // 1. UI 내용 최신 데이터로 매핑
        if (itemIconImage != null)
        {
            // ItemData 스크립터블 오브젝트 내부에 저장된 아이콘 이미지를 가져옵니다.
            itemIconImage.sprite = item.itemIcon;
        }

        if (itemText != null)
        {
            // 그려주신 기획서대로 "아이템명 x 개수" 형태로 텍스트를 구성합니다.
            itemText.text = $"{item.itemName} x{count}";
        }

        // 2. 팝업창 활성화
        popupPanel.SetActive(true);

        // 3. 연타 대처용 타이머 초기화 로직
        // 이미 팝업이 켜져 있는 도중에 다른 아이템을 또 먹으면, 
        // 기존 2초 타이머를 지우고 새로 2초를 처음부터 다시 셉니다.
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    /// <summary>
    /// 설정한 시간이 지나면 자동으로 팝업을 끄는 코루틴입니다.
    /// </summary>
    IEnumerator HideAfterDelay()
    {
        // 설정한 디스플레이 시간(예: 2초)만큼 대기합니다.
        yield return new WaitForSeconds(displayTime);

        // 시간이 다 지나면 다시 패널을 비활성화합니다.
        popupPanel.SetActive(false);
    }
}
