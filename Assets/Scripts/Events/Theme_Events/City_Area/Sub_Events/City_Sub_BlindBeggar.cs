using System;
using UnityEngine;
using TextGame.Core;

namespace TextGame.Events.Theme.City.SubEvents
{
    public static class City_Sub_BlindBeggar
    {
        public static EventStep GetEvent(Player player, GameManager mgr, string locationDesc = "길모퉁이 구석")
        {
            EventStep step = new EventStep();

            step.Title = "[서브 조우 : 눈먼 거지]";
            step.BodyText = $"{locationDesc}, 해진 붕대로 두 눈을 감은 사내가 웅크려 있습니다.\n" +
                            "그의 앞 녹슨 양철 그릇 안에는 난생처음 보는 진귀한 보물이 형언할 수 없는 빛을 뿜어내고 있습니다.";

            // 1번 선택지: 보물 강탈
            step.Option1Text = "진귀한 보물을 빼앗는다";
            step.OnOption1 = (p, m) =>
            {
                p.Luck = 0;
                p.Mental -= 5;
                m.Append_Text("\n타오르는 탐욕에 눈이 멀어 사내의 숨통을 끊고 보물을 취했으나, 손에 닿은 보물은 허공으로 녹아 사라졌습니다.\n" +
                               "\"스스로의 행운을 흩뿌리다니, 참으로 기괴한 녀석이군.\" 뒤를 돌아보자 거지는 흔적도 없었습니다.\n" +
                               "<color=red>[행운이 0이 되었습니다.] [정신력 -5]</color>\n");

                City_Area.HasClearedSubEvent = true;
                City_Area.Show_Theme_Buttons(p, m);
            };

            // 2번 선택지: 적선 (별빛 보유량 체크)
            if (player.Starlight >= 7)
            {
                step.Option2Text = "별빛을 조금 덜어준다 (별빛 -7)";
                step.OnOption2 = (p, m) =>
                {
                    player.Starlight -= 7;
                    player.Luck += 7;
                    m.Append_Text("\n자기만족에 불과할지라도, 품에 지닌 별빛을 덜어 그릇에 놓아주었습니다.\n" +
                                   "\"...당신의 앞날에 행운이 깃들길.\" 사내의 축복과 함께 영혼에 온기가 돕니다.\n" +
                                   "<color=#81D4FA>[별빛 -7] [행운 +7]</color>\n");

                    City_Area.HasClearedSubEvent = true;
                    City_Area.Show_Theme_Buttons(p, m);
                };
            }
            else
            {
                step.Option2Text = "<color=#757575>[적선할 별빛이 부족합니다]</color>";
                step.OnOption2 = null; // 비활성화
            }

            // 3번 선택지: 지나치기
            step.Option3Text = "외면하고 지나간다";
            step.OnOption3 = (p, m) =>
            {
                m.Append_Text("\n누군가에게 연민을 베풀 여유는 없습니다. 그저 묵묵히 갈 길을 재촉합니다.\n");
                City_Area.Show_Theme_Buttons(p, m);
            };

            return step;
        }
    }
}