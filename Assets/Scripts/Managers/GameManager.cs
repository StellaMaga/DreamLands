using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TextGame.Core;
using TextGame.Stages;

public enum ExplorationState { Normal, InTheme }
public enum ThemeType { None, Desert, City }

public class GameManager : MonoBehaviour
{
    [Header("UI 연결")]
    public ScrollRect storyScrollRect;
    public Transform storyContentTransform; // Viewport > Content
    public Image backgroundImage;          // 전체 배경 이미지
    public GameObject statusPopupPanel;     // 상태 팝업 패널

    [Header("프리팹")]
    public GameObject textLogPrefab;
    public GameObject imageLogPrefab;

    [Header("하단 고정 버튼")]
    public GameObject button1;
    public GameObject button2;
    public GameObject button3;

    // 인스펙터 직렬화 경고(UAC1001) 방지
    [NonSerialized]
    public Player player;

    private Sprite currentBgSprite = null;

    [Header("테마 상태")]
    public ExplorationState currentState = ExplorationState.Normal;
    public ThemeType currentTheme = ThemeType.None;

    /// <summary>
    /// PrologueManager 등 외부에서 호출하는 진입점 (CS1061 해결)
    /// </summary>
    public void Game_Start(Player customPlayer)
    {
        Start_Game(customPlayer);
    }

    /// <summary>
    /// 1. 프롤로그 및 2. [메인: 얕은 꿈] 시작
    /// </summary>
    public void Start_Game(Player customPlayer)
    {
        this.player = customPlayer;
        currentState = ExplorationState.Normal;
        currentTheme = ThemeType.None;
        Stage_ShallowDream.Initialize();

        // 얕은 꿈 시작 배경 로드
        Sprite fogBg = Resources.Load<Sprite>("Backgrounds/Fog_Dream");
        Change_Background(fogBg);

        Append_Text("""
            <b>[ 메인 : 얕은 꿈 ]</b>
            사방이 짙은 안개로 가득 찬 낯선 대지 위에서 눈을 떴다.
            어디선가 스산한 바람이 불어오며, 당신의 여정이 시작됨을 알린다.
            """);

        Show_Main_UI();
    }

    /// <summary>
    /// 메인 대기 UI 출력 (외부 이벤트 종료 시 복귀점)
    /// </summary>
    public void Show_Main_UI()
    {
        if (CheckPlayerDeath()) return;
        if (statusPopupPanel != null) statusPopupPanel.SetActive(false);

        Show_Main_Buttons();
    }

    /// <summary>
    /// 기본 탐험 대기 버튼 노출 (상태 유지)
    /// </summary>
    public void Show_Main_Buttons()
    {
        Set_Buttons("1. 앞으로 나아간다", "2. 상태 확인", null,
            () => Stage_ShallowDream.Next_Turn(player, this),
            Show_Status_Popup,
            null
        );
    }

    /// <summary>
    /// 플레이어 사망 판정 (Player.cs의 IsDead 및 3대 기본 스탯 고갈 체크)
    /// </summary>
    public bool CheckPlayerDeath()
    {
        if (player == null) return false;

        // 명시적 Die() 호출 또는 3대 기본 스탯(정신력, 이성, 의지) 고갈 판정
        bool isZeroStats = (player.Mental <= 0 || player.Sanity <= 0 || player.Will <= 0);

        if (player.IsDead || isZeroStats)
        {
            Append_Text("<color=#E53935><b>더 이상 앞으로 나아갈 수 없습니다... 여정이 끝났습니다.</b></color>");
            Set_Buttons("처음으로 돌아가기", null, null, () =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
                );
            }, null, null);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 미니 이벤트 전용 로그 (한 줄 가볍게 누적)
    /// </summary>
    public void Append_Mini_Log(string text)
    {
        Append_Text($"<color=#B0BEC5>▶ {text}</color>");
    }

    /// <summary>
    /// 서브/메인/테마 이벤트 실행 (배경 체크 + 삽화 누적 + 지문 누적 + 선택지 교체)
    /// </summary>
    public void Execute_EventStep(EventStep step)
    {
        if (step == null) return;

        // 배경 변경 규칙 적용
        if (step.bgImage != null)
        {
            Change_Background(step.bgImage);
        }

        // 본문 상단 삽화 출력 (소문자 필드 반영)
        if (step.illustration != null && step.imgPos == IllustrationPosition.Top)
        {
            Append_Image(step.illustration);
        }

        // 지문 누적
        string content = "";
        if (!string.IsNullOrEmpty(step.Title))
        {
            content += $"<b>{step.Title}</b>\n";
        }
        content += step.BodyText;

        // 본문 중간 삽화 출력 (소문자 필드 반영)
        if (step.illustration != null && step.imgPos == IllustrationPosition.Middle)
        {
            Append_Text(content);
            Append_Image(step.illustration);
            if (!string.IsNullOrEmpty(step.BottomText))
            {
                Append_Text(step.BottomText);
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(step.BottomText))
            {
                content += $"\n\n{step.BottomText}";
            }
            Append_Text(content);
        }

        // 선택지 버튼 바인딩
        Set_Buttons(
            step.Option1Text, step.Option2Text, step.Option3Text,
            () => { step.OnOption1?.Invoke(player, this); Status_UI(); },
            () => { step.OnOption2?.Invoke(player, this); Status_UI(); },
            () => { step.OnOption3?.Invoke(player, this); Status_UI(); }
        );
    }

    /// <summary>
    /// 배경 변경 처리 (동일한 배경이면 갱신 생략)
    /// </summary>
    public void Change_Background(Sprite newBg)
    {
        if (newBg == null || newBg == currentBgSprite) return;

        currentBgSprite = newBg;
        if (backgroundImage != null)
        {
            backgroundImage.sprite = newBg;
            backgroundImage.color = Color.white;
        }
    }

    /// <summary>
    /// 텍스트 로그 한 줄 인스턴스화
    /// </summary>
    public void Append_Text(string text)
    {
        if (string.IsNullOrEmpty(text)) return;
        GameObject obj = Instantiate(textLogPrefab, storyContentTransform);
        TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();
        if (tmp != null) tmp.text = text;
        Scroll_To_Bottom();
    }

    /// <summary>
    /// 삽화 로그 인스턴스화
    /// </summary>
    public void Append_Image(Sprite sprite)
    {
        if (sprite == null || imageLogPrefab == null) return;
        GameObject obj = Instantiate(imageLogPrefab, storyContentTransform);
        Image img = obj.GetComponent<Image>();
        if (img != null) img.sprite = sprite;
        Scroll_To_Bottom();
    }

    public void Scroll_To_Bottom()
    {
        StartCoroutine(ScrollToBottomRoutine());
    }

    private IEnumerator ScrollToBottomRoutine()
    {
        yield return new WaitForEndOfFrame();
        Canvas.ForceUpdateCanvases();
        if (storyScrollRect != null)
        {
            storyScrollRect.verticalNormalizedPosition = 0f;
        }
    }

    public void Set_Buttons(string b1, string b2, string b3,
                             UnityEngine.Events.UnityAction a1,
                             UnityEngine.Events.UnityAction a2,
                             UnityEngine.Events.UnityAction a3)
    {
        Setup_Button(button1, b1, a1);
        Setup_Button(button2, b2, a2);
        Setup_Button(button3, b3, a3);
    }

    private void Setup_Button(GameObject btnObj, string label, UnityEngine.Events.UnityAction action)
    {
        if (btnObj == null) return;
        bool active = !string.IsNullOrEmpty(label);
        btnObj.SetActive(active);

        if (active)
        {
            TextMeshProUGUI txt = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = label;

            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                if (action != null) btn.onClick.AddListener(action);
            }
        }
    }

    public void Status_UI()
    {
        // HUD 슬라이더 및 스탯 수치 갱신 로직
    }

    public void Show_Status_Popup()
    {
        if (statusPopupPanel != null)
        {
            statusPopupPanel.SetActive(!statusPopupPanel.activeSelf);
        }
    }
}