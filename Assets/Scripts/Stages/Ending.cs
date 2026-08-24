using System;
using UnityEngine;
using TMPro;
using TextGame.Core;

namespace TextGame.Stages
{
    public class Ending : MonoBehaviour
    {
        [Header("UI 연결")]
        [SerializeField] private TMP_Text titleText;       // 엔딩 제목 UI
        [SerializeField] private TMP_Text bodyText;        // 엔딩 본문 UI
        [SerializeField] private TMP_Text statsText;       // 플레이어 스탯 UI
        [SerializeField] private GameObject endingPanel;   // 엔딩 화면 패널

        public void Show_Ending(Player player, string clearReason = "")
        {
            if (endingPanel != null)
                endingPanel.SetActive(true);

            // 1. 스테이지 클리어인 경우
            if (clearReason == "Stage1_Clear")
            {
                if (titleText != null)
                    titleText.text = Game_System.Green("[ 다음 경계로의 전진 (STAGE CLEAR) ]");

                if (bodyText != null)
                    bodyText.text = $"축하합니다! [{player.Name}]은(는) 얕은 꿈의 모든 시련을 이겨내고 생존했습니다.";

                if (statsText != null)
                    statsText.text = "";

                return;
            }

            // 2. 사망 엔딩인 경우
            if (titleText != null)
                titleText.text = Game_System.Red("[ DEAD END : 몽상가의 죽음 ]");

            string bodyResult = "";

            switch (player.CauseOfDeath)
            {
                case DeathType.Mental_Zero:
                    bodyResult = Game_System.Red("[ 데드엔딩 : 껍데기의 균열 ]\n") +
                                 $"망상가들의 무차별적인 정신적 공격에 [{player.Name}]의 껍데기가 균열을 일으킵니다.\n" +
                                 "정신력이 바닥나며 자아가 산산조각 났습니다.";
                    break;

                case DeathType.Sanity_Zero:
                    bodyResult = Game_System.Red("[ 데드엔딩 : 영원한 악몽 ]\n") +
                                 "이성을 집어삼킨 광기가 눈을 멀게 합니다.\n" +
                                 "'당신은 완전히 미쳐버려, 자신이 누구인지도 모른 채 영원히 악몽을 배회합니다.'";
                    break;

                case DeathType.Madness_Over:
                    bodyResult = Game_System.Red("[ 데드엔딩 : 광기의 침식 ]\n") +
                                 $"이성 수치가 광기 아래로 떨어지며 [{player.Name}]의 의식이 악몽 속으로 침몰합니다.\n" +
                                 "'환각과 실재의 경계가 무너지며 스스로가 광기 그 자체가 되었습니다.'";
                    break;

                case DeathType.Flower_Trap:
                    bodyResult = Game_System.Red("[ 데드엔딩 : 짐승의 자양분 ]\n") +
                                 "인사도 없이 꽃을 꺾으려 한 무례함에 대지가 진노합니다.\n" +
                                 $"[{player.Name}]의 몸은 꽃의 뿌리에 옭아매여 차가운 대지의 자양분으로 전락했습니다.";
                    break;

                case DeathType.Give_Up:
                    bodyResult = Game_System.Red("[ 데드엔딩 : 공허 속 침몰 ]\n") +
                                 $"[{player.Name}]은(는) 더 이상 나아갈 의지를 잃어버렸습니다.\n" +
                                 "스스로 닻을 잘라낸 당신은 보랏빛 안개 너머 공허 속으로 서서히 침몰합니다.";
                    break;

                case DeathType.Suicide:
                    bodyResult = Game_System.Red("[ 데드엔딩 : 자살 충동으로 인한 사망 ]\n") +
                                 "강렬하게 밀려드는 죽음의 충동을 견디지 못하고 스스로 생을 마감했습니다...";
                    break;

                default:
                    bodyResult = Game_System.Red("[ 데드엔딩 : 존재의 소멸 ]\n") +
                                 "꿈의 세계 자체가 붕괴하며 당신의 존재 흔적을 지워버립니다.";
                    break;
            }

            if (bodyText != null)
                bodyText.text = bodyResult;

            // 남은 흔적 (스탯 출력)
            if (statsText != null)
            {
                statsText.text = $"[ 당신이 남긴 마지막 흔적 ]\n" +
                                 $"- 남은 정신력 : {player.Mental} | 이성 : {player.Sanity} | 의지 : {player.Will} | 광기 : {player.Madness}";
            }
        }
    }
}