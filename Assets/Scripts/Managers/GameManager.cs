using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TextGame.Core;
using TextGame.Stages;
using TextGame.Events.Shallow_Dream;

public class GameManager : MonoBehaviour
{
    // [1. UI 패널 및 씬 오브젝트 관리]
    [Header("UI 패널 제어")]
    [Tooltip("프롤로그 지문 및 이름 입력이 진행되는 패널")]
    public GameObject storyPanel;

    [Tooltip("본 게임 탐험 및 전투가 진행되는 메인 패널")]
    public GameObject mainGamePanel;

    [Tooltip("이벤트에 따라 이미지가 교체되는 배경 Image")]
    public Image backgroundImage;


    // [2. 상단 HUD 상태창 박스 (게이지 바 & 텍스트)]
    [Header("상단 HUD 상태창")]
    [Tooltip("플레이어 이름 출력 텍스트")]
    public TextMeshProUGUI playerNameText;

    [Tooltip("정신력(MT) 게이지 슬라이더")]
    public Slider mentalSlider;

    [Tooltip("정신력(MT) 수치 텍스트 (예: 60)")]
    public TextMeshProUGUI mentalValueText;

    [Tooltip("이성(SAN) 게이지 슬라이더")]
    public Slider sanitySlider;

    [Tooltip("이성(SAN) 수치 텍스트 (예: 55)")]
    public TextMeshProUGUI sanityValueText;

    [Tooltip("부가 스탯 텍스트 (Will, Madness, 별빛 등)")]
    public TextMeshProUGUI extraStatusText;

    [Tooltip("(선택 사항) 기존 한 줄 텍스트 상태창 - 연결 시 함께 갱신")]
    public TextMeshProUGUI statusText;


    // [3. 상세 상태창 팝업 카드 UI]
    [Header("상세 상태창 팝업 카드")]
    [Tooltip("화면 중앙에 표시되는 모달 팝업 패널 (StatusPopupPanel)")]
    public GameObject statusPopupPanel;

    [Tooltip("팝업 카드 내부의 상세 스탯 출력 텍스트 (StatusCardText)")]
    public TextMeshProUGUI statusCardText;


    // =========================================================================
    // [4. 인게임 텍스트 및 선택지 버튼 UI]
    // =========================================================================
    [Header("인게임 텍스트 UI")]
    [Tooltip("중앙 스토리 지문 및 이벤트 출력창")]
    public TextMeshProUGUI dialogueText;

    [Header("선택지 버튼 (MainGamePanel 하위)")]
    public GameObject option1Button;
    public GameObject option2Button;
    public GameObject option3Button;


    // =========================================================================
    // [5. 외부 시스템 및 엔딩 매니저]
    // =========================================================================
    [Header("엔딩 시스템")]
    public Ending endingManager;


    // =========================================================================
    // [6. 탐험 시스템 진행 변수 (State)]
    // =========================================================================
    private Player player;
    private int event_Count = 0;          // 현재 탐험 회차 (진행 턴 수)
    private int cheshire_Count = 0;       // 체셔캣 조우 횟수
    private int cheshireChance = 13;      // 체셔캣 등장 확률 (%)
    private bool met_WhiteRabbit = false; // 흰 토끼 메인 이벤트 달성 여부


    // =========================================================================
    // [7. 생명주기 및 게임 시작 초기화]
    // =========================================================================
    void Start()
    {
        // 씬 단독 테스트 시 예외 방지용
        if (player == null)
        {
            // 에디터 테스트 기본 세팅 필요 시 작성
        }

        // 시작 시 상태창 팝업은 닫아둠
        if (statusPopupPanel != null)
        {
            statusPopupPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 프롤로그 완료 후 호출되는 본 게임 시작 진입점
    /// </summary>
    public void Game_Start(Player customPlayer)
    {
        this.player = customPlayer; //

        // 프롤로그 화면 끄고 메인 게임 UI 활성화
        if (storyPanel != null) storyPanel.SetActive(false); //
        if (mainGamePanel != null) mainGamePanel.SetActive(true); //
        if (statusPopupPanel != null) statusPopupPanel.SetActive(false);

        Status_UI(); //
        Show_Main_UI(); //
    }


    // =========================================================================
    // [8. 메인 탐험 대기 화면 및 턴 진행]
    // =========================================================================
    /// <summary>
    /// 메인 탐험 대기 화면 (앞으로 가기 / 상태 확인)
    /// </summary>
    public void Show_Main_UI()
    {
        if (CheckPlayerDeath()) return; //

        // 팝업창이 열려 있다면 닫기
        if (statusPopupPanel != null)
        {
            statusPopupPanel.SetActive(false);
        }

        dialogueText.text = $"[ 제 1장 : 얕은 꿈 탐험 중... ]\n" +
                            $"현재 이벤트 진행: {event_Count}회차\n\n" +
                            $"[1. 앞으로 나아간다]\n" +
                            $"[2. 잠시 눈을 감는다 (상태 확인)]"; //

        Set_Buttons("1. 앞으로 나아간다", "2. 상태 확인", null, Click_Advance, Click_Status, null); //
    }

    /// <summary>
    /// '앞으로 나아간다' 클릭 시 회차를 진행하고 다음 이벤트를 호출
    /// </summary>
    private void Click_Advance()
    {
        Status_UI(); //

        // 1장 Shallow_Dream 스테이지로부터 EventStep 수신
        EventStep currentStep = Shallow_Dream.Next_Event(
            player,
            ref event_Count,
            ref cheshire_Count,
            ref cheshireChance,
            ref met_WhiteRabbit
        ); //

        Execute_EventStep(currentStep); //
    }

    /// <summary>
    /// '2. 상태 확인' 클릭 시 중앙 카드 팝업 활성화
    /// </summary>
    private void Click_Status()
    {
        if (player == null) return;

        // 중앙 팝업 활성화
        if (statusPopupPanel != null)
        {
            statusPopupPanel.SetActive(true);
        }

        // 웹소설 스타일 카드 내부 텍스트 갱신
        if (statusCardText != null)
        {
            string madnessColor = player.Madness > 0 ? "#FF6B6B" : "#A0AEC0";

            statusCardText.text =
                $"<b>[ 신상 정보 ]</b>\n" +
                $"  이름  :  {player.Name}\n" +
                $"  계통  :  미지의 탐험가\n\n" +
                $"<b>[ 정신 및 심상 ]</b>\n" +
                $"  정신력 :  {player.Mental} / 100\n" +
                $"  이성치 :  {player.Sanity} / 100\n" +
                $"  광기침식:  <color={madnessColor}>{player.Madness}</color>\n\n" +
                $"<b>[ 잠재 역량 ]</b>\n" +
                $"  의지  :  {player.Will}\n" +
                $"  자아  :  {player.Ego}\n" +
                $"  보유 별빛: ★ {player.Starlight}";
        }

        // 하단 버튼을 '닫기' 단일 버튼으로 변경
        Set_Buttons("닫기 / 돌아가기", null, null, Close_StatusPopup, null, null);
    }

    /// <summary>
    /// 상태창 팝업 닫고 메인 탐험 복귀
    /// </summary>
    private void Close_StatusPopup()
    {
        if (statusPopupPanel != null)
        {
            statusPopupPanel.SetActive(false);
        }

        Show_Main_UI();
    }


    // =========================================================================
    // [9. EventStep 실행 및 화면 갱신]
    // =========================================================================
    public void Execute_EventStep(EventStep step)
    {
        // 이벤트 진입 시 팝업 닫기 보장
        if (statusPopupPanel != null)
        {
            statusPopupPanel.SetActive(false);
        }

        // 1. 지문 텍스트 출력
        dialogueText.text = $"{step.Title}\n\n{step.BodyText}"; //

        // 2. 배경 이미지 갱신
        if (backgroundImage != null && step.bgImage != null) //
        {
            backgroundImage.sprite = step.bgImage; //
            backgroundImage.color = Color.white; //
        }

        // 3. 동적 버튼 세팅
        Set_Buttons(
            step.Option1Text, //
            step.Option2Text, //
            step.Option3Text, //
            () => {
                step.OnOption1?.Invoke(player, this); //
                Status_UI(); //
            },
            () => {
                step.OnOption2?.Invoke(player, this); //
                Status_UI(); //
            },
            () => {
                step.OnOption3?.Invoke(player, this); //
                Status_UI(); //
            }
        );
    }


    // =========================================================================
    // [10. UI 렌더링 헬퍼 (상단 HUD & 버튼 세팅)]
    // =========================================================================
    /// <summary>
    /// 상단 HUD 게이지 및 스탯 UI 갱신
    /// </summary>
    public void Status_UI()
    {
        if (player == null) return; //

        // 1. 플레이어 이름
        if (playerNameText != null) //
        {
            playerNameText.text = $"[{player.Name}]"; //
        }

        // 2. 정신력 (Mental) 슬라이더 게이지 & 수치
        if (mentalSlider != null) //
        {
            mentalSlider.maxValue = 100; //
            mentalSlider.value = Mathf.Clamp(player.Mental, 0, mentalSlider.maxValue); //
        }
        if (mentalValueText != null) //
        {
            mentalValueText.text = $"{player.Mental}"; //
        }

        // 3. 이성 (Sanity) 슬라이더 게이지 & 수치
        if (sanitySlider != null) //
        {
            sanitySlider.maxValue = 100; //
            sanitySlider.value = Mathf.Clamp(player.Sanity, 0, sanitySlider.maxValue); //
        }
        if (sanityValueText != null) //
        {
            sanityValueText.text = $"{player.Sanity}"; //
        }

        // 4. 부가 스탯 텍스트
        if (extraStatusText != null) //
        {
            string madnessColor = player.Madness > 0 ? "#FF5555" : "#AAAAAA"; //
            extraStatusText.text = $"Will: {player.Will} | <color={madnessColor}>Madness: {player.Madness}</color> | ★ {player.Starlight}"; //
        }

        // 5. 구형 단일 텍스트 슬롯 호환
        if (statusText != null) //
        {
            statusText.text = $"[{player.Name}] MT: {player.Mental} | Will: {player.Will} | SAN: {player.Sanity} | Madness: {player.Madness}"; //
        }
    }

    /// <summary>
    /// 버튼 활성화/비활성화 및 텍스트, 클릭 리스너 연결
    /// </summary>
    private void Set_Buttons(string opt1Text, string opt2Text, string opt3Text,
                             UnityEngine.Events.UnityAction action1,
                             UnityEngine.Events.UnityAction action2,
                             UnityEngine.Events.UnityAction action3)
    {
        // 1번 버튼
        option1Button.SetActive(opt1Text != null); //
        if (opt1Text != null) //
        {
            option1Button.GetComponentInChildren<TextMeshProUGUI>().text = opt1Text; //
            Button b1 = option1Button.GetComponent<Button>(); //
            b1.onClick.RemoveAllListeners(); //
            if (action1 != null) b1.onClick.AddListener(action1); //
        }

        // 2번 버튼
        option2Button.SetActive(opt2Text != null); //
        if (opt2Text != null) //
        {
            option2Button.GetComponentInChildren<TextMeshProUGUI>().text = opt2Text; //
            Button b2 = option2Button.GetComponent<Button>(); //
            b2.onClick.RemoveAllListeners(); //
            if (action2 != null) b2.onClick.AddListener(action2); //
        }

        // 3번 버튼
        if (option3Button != null) //
        {
            option3Button.SetActive(opt3Text != null); //
            if (opt3Text != null) //
            {
                option3Button.GetComponentInChildren<TextMeshProUGUI>().text = opt3Text; //
                Button b3 = option3Button.GetComponent<Button>(); //
                b3.onClick.RemoveAllListeners(); //
                if (action3 != null) b3.onClick.AddListener(action3); //
            }
        }
    }


    // =========================================================================
    // [11. 상태 이상 및 플레이어 사망/엔딩 판정]
    // =========================================================================
    public bool CheckPlayerDeath()
    {
        if (player == null) return false; //

        // 정신력 0 이하, 이성 0 이하, 광기 침식 시 사망
        if (player.IsDead || player.Mental <= 0 || player.Sanity <= 0 || player.Sanity <= player.Madness) //
        {
            if (!player.IsDead) //
            {
                if (player.Mental <= 0) player.Die(DeathType.Mental_Zero); //
                else if (player.Sanity <= 0) player.Die(DeathType.Sanity_Zero); //
                else if (player.Sanity <= player.Madness) player.Die(DeathType.Madness_Over); //
            }

            if (endingManager != null) //
            {
                endingManager.Show_Ending(player); //
            }
            else
            {
                dialogueText.text = "<color=red>[ GAME OVER ]</color>\n\n정신이 완전히 붕괴되어 차가운 대지 위로 쓰러졌습니다..."; //
                option1Button.SetActive(false); //
                option2Button.SetActive(false); //
                if (option3Button != null) option3Button.SetActive(false); //
            }
            return true; //
        }
        return false; //
    }
}