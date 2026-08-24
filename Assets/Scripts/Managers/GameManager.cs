using TextGame.Core;
using TextGame.Events.Shallow_Dream; // 올바른 네임스페이스로 수정
using TextGame.Stages;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI 화면 연결")]
    public TextMeshProUGUI statusText;   // 상단 플레이어 스탯 UI
    public TextMeshProUGUI dialogueText; // 중앙 지문/스토리 출력 UI
    public GameObject option1Button;     // 하단 선택지 버튼 1 오브젝트
    public GameObject option2Button;     // 하단 선택지 버튼 2 오브젝트
    public GameObject option3Button;     // 하단 선택지 버튼 3 오브젝트 (선택 사항)

    [Header("엔딩 매니저 연결")]
    public TextGame.Stages.Ending endingManager; // 엔딩 UI 스크립트 연결용

    // [탐험 시스템 진행 변수들]
    private Player player;
    private int event_Count = 0;          // 현재 탐험 회차 (턴 수)
    private int cheshire_Count = 0;       // 체셔캣 조우 횟수
    private int cheshireChance = 13;      // 체셔캣 등장 확률 (%)
    private bool met_WhiteRabbit = false; // 흰 토끼 메인 이벤트 달성 여부

    void Start()
    {
        if (player == null)
        {
            // 테스트용 예외 처리
        }
    }

    // [PrologueManager로부터 Player 객체를 받으며 시작]
    public void Game_Start(Player customPlayer)
    {
        this.player = customPlayer;

        Status_UI();
        Show_Main_UI();
    }

    // [메인 탐험 대기 화면]
    public void Show_Main_UI()
    {
        if (CheckPlayerDeath()) return;

        dialogueText.text = $"[ 제 1장 : 얕은 꿈 탐험 중... ]\n" +
                            $"현재 이벤트 진행: {event_Count}회차\n\n" +
                            $"[1. 앞으로 나아간다]\n" +
                            $"[2. 잠시 눈을 감는다 (상태 확인)]";

        Set_Buttons("1. 앞으로 나아간다", "2. 상태 확인", null, Click_Advance, Click_Status, null);
    }

    // [1번 버튼 클릭 시 턴 진행]
    void Click_Advance()
    {
        Status_UI();

        // Shallow_Dream 스테이지로부터 현재 회차에 맞는 EventStep을 받아옴
        EventStep currentStep = Shallow_Dream.Next_Event(
            player,
            ref event_Count,
            ref cheshire_Count,
            ref cheshireChance,
            ref met_WhiteRabbit
        );

        // 받아온 EventStep 실행
        Execute_EventStep(currentStep);
    }

    // [EventStep 실행 및 버튼 세팅]
    public void Execute_EventStep(EventStep step)
    {
        dialogueText.text = $"{step.Title}\n\n{step.BodyText}";

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

    // [상태 확인]
    void Click_Status()
    {
        dialogueText.text = $"[{player.Name} 상세 스탯]\n\n" +
                            $"MT(정신력): {player.Mental} | Will(의지): {player.Will}\n" +
                            $"SAN(이성): {player.Sanity} | Madness(광기): {player.Madness}\n" +
                            $"Ego(자아): {player.Ego} | Starlight(별빛): {player.Starlight}";
        Set_Buttons("돌아가기", null, null, Show_Main_UI, null, null);
    }

    // [사망 여부 검사 및 엔딩 연동]
    public bool CheckPlayerDeath()
    {
        // 정신력이 0 이하이거나, 이성이 0 이하이거나, 광기가 이성을 넘어서면 사망 처리
        if (player.IsDead || player.Mental <= 0 || player.Sanity <= 0 || player.Sanity <= player.Madness)
        {
            // 사망 원인이 지정되지 않았을 시 자동 지정
            if (!player.IsDead)
            {
                if (player.Mental <= 0) player.Die(DeathType.Mental_Zero);
                else if (player.Sanity <= 0) player.Die(DeathType.Sanity_Zero);
                else if (player.Sanity <= player.Madness) player.Die(DeathType.Madness_Over);
            }

            // Ending 스크립트 연결 시 엔딩 화면 활성화
            if (endingManager != null)
            {
                endingManager.Show_Ending(player);
            }
            else
            {
                dialogueText.text = "<color=red>[ GAME OVER ]</color>\n\n정신이 완전히 붕괴되어 차가운 대지 위로 쓰러졌습니다...";
                option1Button.SetActive(false);
                option2Button.SetActive(false);
                if (option3Button != null) option3Button.SetActive(false);
            }
            return true;
        }
        return false;
    }

    // [상단 스탯 UI 갱신]
    void Status_UI()
    {
        if (player != null)
            statusText.text = $"[{player.Name}] MT: {player.Mental} | Will: {player.Will} | SAN: {player.Sanity} | Madness: {player.Madness}";
    }

    // [버튼 세팅 함수 (3개 지원)]
    void Set_Buttons(string opt1Text, string opt2Text, string opt3Text,
                     UnityEngine.Events.UnityAction action1,
                     UnityEngine.Events.UnityAction action2,
                     UnityEngine.Events.UnityAction action3)
    {
        // 1번 버튼
        option1Button.SetActive(opt1Text != null);
        if (opt1Text != null)
        {
            option1Button.GetComponentInChildren<TextMeshProUGUI>().text = opt1Text;
            Button b1 = option1Button.GetComponent<Button>();
            b1.onClick.RemoveAllListeners();
            if (action1 != null) b1.onClick.AddListener(action1);
        }

        // 2번 버튼
        option2Button.SetActive(opt2Text != null);
        if (opt2Text != null)
        {
            option2Button.GetComponentInChildren<TextMeshProUGUI>().text = opt2Text;
            Button b2 = option2Button.GetComponent<Button>();
            b2.onClick.RemoveAllListeners();
            if (action2 != null) b2.onClick.AddListener(action2);
        }

        // 3번 버튼 (오브젝트가 연결되어 있을 때만 동작)
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
}