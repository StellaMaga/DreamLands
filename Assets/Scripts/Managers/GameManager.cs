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

    [Tooltip("이벤트에 따라 전체 배경이 교체되는 Image (단색 배경용)")]
    public Image backgroundImage;


    // [2. 상단 HUD 상태창 박스 (게이지 바 & 텍스트)]
    [Header("상단 HUD 상태창")]
    public TextMeshProUGUI playerNameText;
    public Slider mentalSlider;
    public TextMeshProUGUI mentalValueText;
    public Slider sanitySlider;
    public TextMeshProUGUI sanityValueText;
    public TextMeshProUGUI extraStatusText;
    public TextMeshProUGUI statusText;


    // [3. 상세 상태창 팝업 카드 UI]
    [Header("상세 상태창 팝업 카드")]
    public GameObject statusPopupPanel;
    public TextMeshProUGUI statusCardText;


    // =========================================================================
    // [4. 인게임 본문 연출 UI (동적 삽화 & 텍스트)]
    // =========================================================================
    [Header("인게임 동적 본문 UI")]
    [Tooltip("상단 스토리 지문 출력창")]
    public TextMeshProUGUI dialogueText;

    [Tooltip("텍스트 사이/상/하단에 유동적으로 삽입되는 삽화 이미지")]
    public Image eventIllustration;

    [Tooltip("삽화 아래에 배치되는 하단 대사/지문창 (Middle 모드 전용)")]
    public TextMeshProUGUI bottomDialogueText;

    [Header("선택지 버튼 (MainGamePanel 하위)")]
    public GameObject option1Button;
    public GameObject option2Button;
    public GameObject option3Button;


    // [5. 외부 시스템 및 엔딩 매니저]
    [Header("엔딩 시스템")]
    public Ending endingManager;


    // [6. 탐험 시스템 진행 변수 (State)]
    private Player player;
    private int event_Count = 0;
    private int cheshire_Count = 0;
    private int cheshireChance = 13;
    private bool met_WhiteRabbit = false;


    void Start()
    {
        if (statusPopupPanel != null)
        {
            statusPopupPanel.SetActive(false);
        }
    }

    public void Game_Start(Player customPlayer)
    {
        this.player = customPlayer;

        if (storyPanel != null) storyPanel.SetActive(false);
        if (mainGamePanel != null) mainGamePanel.SetActive(true);
        if (statusPopupPanel != null) statusPopupPanel.SetActive(false);

        Status_UI();
        Show_Main_UI();
    }

    /// <summary>
    /// 메인 탐험 대기 화면 (앞으로 가기 / 상태 확인)
    /// </summary>
    public void Show_Main_UI()
    {
        if (CheckPlayerDeath()) return;

        if (statusPopupPanel != null)
            statusPopupPanel.SetActive(false);

        // 대기 화면에서는 삽화 및 하단 텍스트 숨김
        if (eventIllustration != null) eventIllustration.gameObject.SetActive(false);
        if (bottomDialogueText != null) bottomDialogueText.gameObject.SetActive(false);

        if (dialogueText != null)
        {
            dialogueText.gameObject.SetActive(true);
            dialogueText.transform.SetSiblingIndex(0);
            dialogueText.text = $"[ 제 1장 : 얕은 꿈 탐험 중... ]\n" +
                                $"현재 이벤트 진행: {event_Count}회차\n\n" +
                                $"[1. 앞으로 나아간다]\n" +
                                $"[2. 잠시 눈을 감는다 (상태 확인)]";
        }

        Set_Buttons("1. 앞으로 나아간다", "2. 상태 확인", null, Click_Advance, Click_Status, null);
    }

    private void Click_Advance()
    {
        Status_UI();

        EventStep currentStep = Shallow_Dream.Next_Event(
            player,
            ref event_Count,
            ref cheshire_Count,
            ref cheshireChance,
            ref met_WhiteRabbit
        );

        Execute_EventStep(currentStep);
    }

    private void Click_Status()
    {
        if (player == null) return;

        if (statusPopupPanel != null)
            statusPopupPanel.SetActive(true);

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

        Set_Buttons("닫기 / 돌아가기", null, null, Close_StatusPopup, null, null);
    }

    private void Close_StatusPopup()
    {
        if (statusPopupPanel != null)
            statusPopupPanel.SetActive(false);

        Show_Main_UI();
    }


    // =========================================================================
    // [9. EventStep 실행 및 화면 렌더링]
    // =========================================================================
    public void Execute_EventStep(EventStep step)
    {
        if (statusPopupPanel != null)
            statusPopupPanel.SetActive(false);

        // 1. 전체 배경 교체 (필요 시)
        if (backgroundImage != null && step.bgImage != null)
        {
            backgroundImage.sprite = step.bgImage;
            backgroundImage.color = Color.white;
        }

        // 2. 본문 및 삽화 동적 배치
        Render_Content(step);

        // 3. 버튼 세팅
        Set_Buttons(
            step.Option1Text,
            step.Option2Text,
            step.Option3Text,
            () => {
                step.OnOption1?.Invoke(player, this);
                Status_UI();
            },
            () => {
                step.OnOption2?.Invoke(player, this);
                Status_UI();
            },
            () => {
                step.OnOption3?.Invoke(player, this);
                Status_UI();
            }
        );
    }

    /// <summary>
    /// 위치 옵션(IllustrationPosition)에 따라 삽화와 지문 순서를 실시간 정렬
    /// </summary>
    private void Render_Content(EventStep step)
    {
        bool hasImage = (step.illustration != null && step.imgPos != IllustrationPosition.None && eventIllustration != null);

        if (hasImage)
        {
            eventIllustration.gameObject.SetActive(true);
            eventIllustration.sprite = step.illustration;
            eventIllustration.color = Color.white;

            switch (step.imgPos)
            {
                case IllustrationPosition.Top:
                    // [삽화] -> [본문 텍스트]
                    eventIllustration.transform.SetSiblingIndex(0);
                    dialogueText.transform.SetSiblingIndex(1);

                    dialogueText.gameObject.SetActive(true);
                    dialogueText.text = $"{step.Title}\n\n{step.BodyText}";

                    if (bottomDialogueText != null)
                        bottomDialogueText.gameObject.SetActive(false);
                    break;

                case IllustrationPosition.Middle:
                    // [상단 텍스트] -> [삽화] -> [하단 텍스트] (고양이 상인 형태)
                    dialogueText.transform.SetSiblingIndex(0);
                    eventIllustration.transform.SetSiblingIndex(1);

                    dialogueText.gameObject.SetActive(true);
                    dialogueText.text = $"{step.Title}\n\n{step.BodyText}";

                    if (bottomDialogueText != null)
                    {
                        bottomDialogueText.transform.SetSiblingIndex(2);
                        bottomDialogueText.gameObject.SetActive(!string.IsNullOrEmpty(step.BottomText));
                        bottomDialogueText.text = step.BottomText;
                    }
                    break;

                case IllustrationPosition.Bottom:
                    // [본문 텍스트] -> [삽화]
                    dialogueText.transform.SetSiblingIndex(0);
                    eventIllustration.transform.SetSiblingIndex(1);

                    dialogueText.gameObject.SetActive(true);
                    dialogueText.text = $"{step.Title}\n\n{step.BodyText}";

                    if (bottomDialogueText != null)
                        bottomDialogueText.gameObject.SetActive(false);
                    break;
            }
        }
        else
        {
            // 삽화가 없는 순수 텍스트 이벤트
            if (eventIllustration != null) eventIllustration.gameObject.SetActive(false);
            if (bottomDialogueText != null) bottomDialogueText.gameObject.SetActive(false);

            if (dialogueText != null)
            {
                dialogueText.gameObject.SetActive(true);
                dialogueText.transform.SetSiblingIndex(0);
                dialogueText.text = $"{step.Title}\n\n{step.BodyText}";
            }
        }
    }


    // =========================================================================
    // [10. 상단 HUD & 버튼 헬퍼]
    // =========================================================================
    public void Status_UI()
    {
        if (player == null) return;

        if (playerNameText != null) playerNameText.text = $"[{player.Name}]";

        if (mentalSlider != null)
        {
            mentalSlider.maxValue = 100;
            mentalSlider.value = Mathf.Clamp(player.Mental, 0, mentalSlider.maxValue);
        }
        if (mentalValueText != null) mentalValueText.text = $"{player.Mental}";

        if (sanitySlider != null)
        {
            sanitySlider.maxValue = 100;
            sanitySlider.value = Mathf.Clamp(player.Sanity, 0, sanitySlider.maxValue);
        }
        if (sanityValueText != null) sanityValueText.text = $"{player.Sanity}";

        if (extraStatusText != null)
        {
            string madnessColor = player.Madness > 0 ? "#FF5555" : "#AAAAAA";
            extraStatusText.text = $"Will: {player.Will} | <color={madnessColor}>Madness: {player.Madness}</color> | ★ {player.Starlight}";
        }

        if (statusText != null)
        {
            statusText.text = $"[{player.Name}] MT: {player.Mental} | Will: {player.Will} | SAN: {player.Sanity} | Madness: {player.Madness}";
        }
    }

    private void Set_Buttons(string opt1Text, string opt2Text, string opt3Text,
                             UnityEngine.Events.UnityAction action1,
                             UnityEngine.Events.UnityAction action2,
                             UnityEngine.Events.UnityAction action3)
    {
        option1Button.SetActive(opt1Text != null);
        if (opt1Text != null)
        {
            option1Button.GetComponentInChildren<TextMeshProUGUI>().text = opt1Text;
            Button b1 = option1Button.GetComponent<Button>();
            b1.onClick.RemoveAllListeners();
            if (action1 != null) b1.onClick.AddListener(action1);
        }

        option2Button.SetActive(opt2Text != null);
        if (opt2Text != null)
        {
            option2Button.GetComponentInChildren<TextMeshProUGUI>().text = opt2Text;
            Button b2 = option2Button.GetComponent<Button>();
            b2.onClick.RemoveAllListeners();
            if (action2 != null) b2.onClick.AddListener(action2);
        }

        if (option3Button != null)
        {
            option3Button.SetActive(opt3Text != null);
            if (opt3Text != null)
            {
                option3Button.GetComponentInChildren<TextMeshProUGUI>().text = opt3Text;
                Button b3 = option3Button.GetComponent<Button>();
                b3.onClick.RemoveAllListeners();
                if (action3 != null) b3.onClick.AddListener(action3);
            }
        }
    }

    public bool CheckPlayerDeath()
    {
        if (player == null) return false;

        if (player.IsDead || player.Mental <= 0 || player.Sanity <= 0 || player.Sanity <= player.Madness)
        {
            if (!player.IsDead)
            {
                if (player.Mental <= 0) player.Die(DeathType.Mental_Zero);
                else if (player.Sanity <= 0) player.Die(DeathType.Sanity_Zero);
                else if (player.Sanity <= player.Madness) player.Die(DeathType.Madness_Over);
            }

            if (endingManager != null)
            {
                endingManager.Show_Ending(player);
            }
            else
            {
                if (eventIllustration != null) eventIllustration.gameObject.SetActive(false);
                if (bottomDialogueText != null) bottomDialogueText.gameObject.SetActive(false);

                dialogueText.text = "<color=red>[ GAME OVER ]</color>\n\n정신이 완전히 붕괴되어 차가운 대지 위로 쓰러졌습니다...";
                option1Button.SetActive(false);
                option2Button.SetActive(false);
                if (option3Button != null) option3Button.SetActive(false);
            }
            return true;
        }
        return false;
    }
}