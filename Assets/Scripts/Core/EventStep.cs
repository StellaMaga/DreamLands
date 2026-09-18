using System;
using UnityEngine;

namespace TextGame.Core
{
    /// <summary>
    /// 모든 화면(전투, 휴식, 탐색, 메인/서브 스토리)이 공유하는 공용 UI 렌더링 데이터 단위
    /// </summary>
    public class EventStep
    {
        public string Title;
        public string BodyText;
        public string Option1Text;
        public string Option2Text;
        public string Option3Text;

        public Action<Player, GameManager> OnOption1;
        public Action<Player, GameManager> OnOption2;
        public Action<Player, GameManager> OnOption3;

        // 배경 스프라이트 (null이면 기본 배경 유지)
        public Sprite bgImage;

        public EventStep(string title, string bodyText,
                         string opt1 = "다음 ▶", string opt2 = null, string opt3 = null,
                         Action<Player, GameManager> onOption1 = null,
                         Action<Player, GameManager> onOption2 = null,
                         Action<Player, GameManager> onOption3 = null,
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
            this.bgImage = bgImage;
        }

        // 연속 대화(멀티 다이얼로그) 체인 생성 헬퍼
        public static EventStep Multi_Step(string title, DialogueNode[] dialogues, Action<Player, GameManager> onComplete, Sprite bgImage = null)
        {
            return Step_Helper(title, dialogues, 0, onComplete, bgImage);
        }

        private static EventStep Step_Helper(string title, DialogueNode[] dialogues, int index, Action<Player, GameManager> onComplete, Sprite bgImage = null)
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
                        manager.Execute_EventStep(Step_Helper(title, dialogues, index + 1, onComplete, bgImage));
                    }
                },
                bgImage: bgImage
            );
        }
    }
}