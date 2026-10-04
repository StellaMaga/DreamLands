using System;
using UnityEngine;

namespace TextGame.Core
{
    /// <summary>
    /// 삽화 이미지가 배치될 위치
    /// </summary>
    public enum IllustrationPosition
    {
        None,       // 이미지 없음 (순수 텍스트만)
        Top,        // [삽화] -> [텍스트]
        Middle,     // [상단 텍스트] -> [삽화] -> [하단 텍스트] (고양이 상인 스타일)
        Bottom      // [텍스트] -> [삽화]
    }

    /// <summary>
    /// 모든 화면(전투, 휴식, 탐색, 메인/서브 스토리)이 공유하는 공용 UI 렌더링 데이터 단위
    /// </summary>
    public class EventStep
    {
        public string Title;
        public string BodyText;
        public string BottomText; // Middle 배치 시 삽화 아래에 들어갈 텍스트
        public string Option1Text;
        public string Option2Text;
        public string Option3Text;

        public Action<Player, GameManager> OnOption1;
        public Action<Player, GameManager> OnOption2;
        public Action<Player, GameManager> OnOption3;

        // 중앙 삽화 카드 스프라이트 및 배치 위치
        public Sprite illustration;
        public IllustrationPosition imgPos = IllustrationPosition.None;

        // (선택) 전체 화면 배경 스프라이트가 필요한 경우
        public Sprite bgImage;

        public EventStep(string title, string bodyText,
                         string opt1 = "다음 ▶", string opt2 = null, string opt3 = null,
                         Action<Player, GameManager> onOption1 = null,
                         Action<Player, GameManager> onOption2 = null,
                         Action<Player, GameManager> onOption3 = null,
                         Sprite illustration = null,
                         IllustrationPosition imgPos = IllustrationPosition.None,
                         string bottomText = null,
                         Sprite bgImage = null)
        {
            Title = title;
            BodyText = bodyText;
            Option1Text = opt1;
            Option2Text = opt2;
            Option3Text = opt3;
            OnOption1 = onOption1;
            OnOption2 = onOption2;
            OnOption3 = onOption3;
            this.illustration = illustration;
            this.imgPos = imgPos;
            this.BottomText = bottomText;
            this.bgImage = bgImage;
        }

        // 연속 대화(멀티 다이얼로그) 체인 생성 헬퍼
        public static EventStep Multi_Step(string title, DialogueNode[] dialogues, Action<Player, GameManager> onComplete,
                                          Sprite illustration = null, IllustrationPosition imgPos = IllustrationPosition.None)
        {
            return Step_Helper(title, dialogues, 0, onComplete, illustration, imgPos);
        }

        private static EventStep Step_Helper(string title, DialogueNode[] dialogues, int index, Action<Player, GameManager> onComplete,
                                            Sprite illustration = null, IllustrationPosition imgPos = IllustrationPosition.None)
        {
            var node = dialogues[index];
            bool isLast = index >= dialogues.Length - 1;

            string formattedBody = string.IsNullOrEmpty(node.Speaker)
                ? node.Text
                : $"{node.Speaker}\n{node.Text}";

            return new EventStep(
                title: title,
                bodyText: formattedBody,
                opt1: isLast ? "확인" : "다음 ▶",
                opt2: null,
                opt3: null,
                onOption1: (player, manager) =>
                {
                    if (isLast)
                    {
                        onComplete?.Invoke(player, manager);
                    }
                    else
                    {
                        manager.Execute_EventStep(Step_Helper(title, dialogues, index + 1, onComplete, illustration, imgPos));
                    }
                },
                illustration: illustration,
                imgPos: imgPos
            );
        }
    }
}