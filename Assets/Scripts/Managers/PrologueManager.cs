using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using TextGame.Core;

namespace TextGame.Core
{
    public class PrologueManager : MonoBehaviour
    {
        [Header("UI 요소 연결")]
        [SerializeField] private GameObject storyPanel;         // StoryPanel (프롤로그 패널)
        [SerializeField] private GameObject mainGamePanel;      // MainGamePanel (메인 게임 패널)
        [SerializeField] private TextMeshProUGUI dialogueText;  // 스토리 본문 텍스트 UI
        [SerializeField] private Button nextButton;             // 다음 버튼
        [SerializeField] private TextMeshProUGUI nextButtonText; // 다음 버튼 글자

        [Header("게임 매니저 연결")]
        [SerializeField] private GameManager gameManager;

        private List<DialogueNode> currentDialogues;
        private int currentIndex = 0;
        private DreamInfo selectedDream;

        void Start()
        {
            // 1. 초기 자각몽 성향 추첨 및 프롤로그 대사 생성
            selectedDream = Game_System.Lucid_dream();
            currentDialogues = Prologue.Prologue_Dialogues(selectedDream);

            // 2. 패널 초기 상태 설정
            if (storyPanel != null) storyPanel.SetActive(true);
            if (mainGamePanel != null) mainGamePanel.SetActive(false);

            // 3. 버튼 리스너 등록
            if (nextButton != null)
            {
                nextButton.onClick.RemoveAllListeners();
                nextButton.onClick.AddListener(OnClick_Next);
            }

            ShowCurrentNode();
        }

        // 대사 출력 함수
        private void ShowCurrentNode()
        {
            if (currentDialogues == null || currentIndex >= currentDialogues.Count)
            {
                FinishPrologue();
                return;
            }

            DialogueNode node = currentDialogues[currentIndex];

            string formattedText = string.IsNullOrEmpty(node.Speaker)
                ? node.Text
                : $"{node.Speaker}\n{node.Text}";

            if (dialogueText != null)
            {
                dialogueText.text = formattedText;
            }

            if (nextButtonText != null)
            {
                nextButtonText.text = (currentIndex == currentDialogues.Count - 1) ? "완료 ▶" : "다음 ▶";
            }
        }

        // [다음 ▶] 버튼 클릭
        private void OnClick_Next()
        {
            currentIndex++;
            ShowCurrentNode();
        }

        // 프롤로그 종료 및 본 게임 진입
        private void FinishPrologue()
        {
            // 이름 입력 없이 기본 플레이어 생성 ("탐험가" 또는 원하는 기본 명칭)
            Player newPlayer = new Player("탐험가", selectedDream);

            if (storyPanel != null)
            {
                storyPanel.SetActive(false);
            }

            if (mainGamePanel != null)
            {
                mainGamePanel.SetActive(true);
            }

            if (gameManager != null)
            {
                gameManager.Game_Start(newPlayer);
            }
        }
    }
}