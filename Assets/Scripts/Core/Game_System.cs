using System;
using UnityEngine;

namespace TextGame.Core
{
    // [자각몽 성향 정보 구조]
    public struct DreamInfo
    {
        public string Title;
        public string Description;
        public string StatType;
        public int BonusValue;
        public string StatType2;
        public int BonusValue2;
        public bool IsDiceBonus;   // 악 성향을 판별
    }

    // [크툴루 성공 등급]
    public enum CoC_Result
    {
        Jackpot, // 잭팟(1~5)
        Success, // 성공 (스탯 이하)
        Failure, // 실패
        Fumble   // 펌블(96~100)
    }

    // [사망 원인 목록]
    public enum DeathType
    {
        None,            // 생존 중
        Mental_Zero,     // 정신력 고갈
        Sanity_Zero,     // 이성 고갈
        Madness_Over,    // 광기에 침식됨 (이성 <= 광기)
        Flower_Trap,     // [서브 이벤트 1] 작은 꽃 꺾으려다 사망
        Suicide,         // [서브이벤트 13-1] 자살 충동으로 사망
        Give_Up          // 의지 상실 (포기)
    }

    public class Game_System
    {
        private static System.Random rand = new System.Random();

        // [주사위 함수들]
        public static int Roll_D6() => UnityEngine.Random.Range(1, 7);
        public static int Roll_3D6() => Roll_D6() + Roll_D6() + Roll_D6();
        public static int Roll_3D6_Times5() => Roll_3D6() * 5;
        public static int Roll_D20() => UnityEngine.Random.Range(1, 21);
        public static int Roll_D100() => UnityEngine.Random.Range(1, 101);

        // [스탯 조건 검사]
        public static bool Has_Stat(int playerStat, int requiredStat)
        {
            return playerStat >= requiredStat;
        }

        // [UI 선택지용 텍스트 생성기]
        public static string FormatOption(string optionText, string statName, int playerStat, int requiredStat)
        {
            if (playerStat >= requiredStat)
            {
                return $"{optionText} <color=green>[{statName} {requiredStat} 이상 가능]</color>";
            }
            else
            {
                return $"{optionText} <color=red>[{statName} {requiredStat} 필요 (현재: {playerStat})]</color>";
            }
        }

        // [스탯 판정]
        public static bool Stat_Check(string statName, int trial, int statValue, out string resultMessage)
        {
            bool isSuccess = trial <= statValue;
            string statusText = isSuccess ? Green("성공") : Red("실패");

            resultMessage = $"시련 [{trial}] | 판정({statName}) [{statValue}]\n" +
                            $"결과 -> [{statusText}]";

            return isSuccess;
        }

        // [D&D 방식 판정 (주사위 + 스탯/10 + 행운/10)]
        public static bool DD_Check(int dc, string statName, int statValue, Player player, out string resultMessage)
        {   
            // dc = Difficulty Class (난이도 기준치)
            // statValue = 해당 스탯 값
            // player = 플레이어 객체 (행운 수치 확인용)
            // resultMessage = 판정 결과 메시지

            int dice = Roll_D20();                  // D20 주사위 굴림
            int statModifier = statValue / 10;      // 스탯 보정치 계산 (스탯 값 / 10)
            int luckModifier = player.Luck / 10;    // 행운 보정치 계산 (행운 값 / 10)
            int totalRoll = dice + statModifier + luckModifier; 

            bool isSuccess = (dice == 20) || (dice != 1 && totalRoll >= dc);
            string statusText = isSuccess ? Green("성공") : Red("실패");

            resultMessage = $"목표 [{dc}] | 판정 [{totalRoll}] (D20🎲[{dice}] + {statName}[{statModifier}] + 행운[{luckModifier}])\n" +
                            $"결과 ➔ [{statusText}]";

            return isSuccess;
        }

        // [크툴루(CoC) 방식 통합 판정]
        public static CoC_Result CoC_Check(int statValue, int risk = 0)
        {
            int dice = Roll_D100();

            int target = statValue;

            if (risk == 1) target = statValue / 2;
            else if (risk == 2) target = statValue / 5;

            if (dice <= 5) return CoC_Result.Jackpot;
            if (dice >= 96) return CoC_Result.Fumble;

            if (dice <= target)
            {
                return CoC_Result.Success;
            }
            return CoC_Result.Failure;
        }

        // [자각몽 정보 목록]
        public static DreamInfo Lucid_dream()
        {
            DreamInfo[] dreams = new DreamInfo[9] {
                new DreamInfo { Title = "질서 선", Description = "숨막힐 정도로 질서정연한 신전으로 들어가고 있었다."},
                new DreamInfo { Title = "중립 선", Description = "무한의 형상을 띄고 있는 황금빛 길 위를 걷고 있었다."},
                new DreamInfo {Title = "혼돈 선", Description = "정상이 보이지 않는 산을 오르고 있었다."},

                new DreamInfo { Title = "질서 중립", Description = "수많은 책들이 꽂혀진 도서관을 돌아다니고 있었다."},
                new DreamInfo { Title = "진 중립", Description = "무수한 별의 바다 위를 걷고 있었다."},
                new DreamInfo { Title = "혼돈 중립", Description = "정체불명의 무언가를 쫓으며 동시에 쫓기고 있었다."},

                new DreamInfo { Title = "질서 악", Description = "끝없는 심해로 천천히 가라앉고 있었다."},
                new DreamInfo { Title = "중립 악", Description = "시산혈해와 금은보화로 이루어진 공간을 만끽하고 있었다."},
                new DreamInfo { Title = "혼돈 악", Description = "무게를 잊은 듯 자유롭게 허공을 누비고 있었다."}
            };

            int index = rand.Next(0, dreams.Length);
            return dreams[index];
        }

        // [특정 NPC 전용 색]
        public static string ColorSpeaker(string speaker)
        {
            if (string.IsNullOrEmpty(speaker)) return "";

            string cleanName = speaker.Replace("[", "").Replace("]", "").Trim();

            switch (cleanName)
            {
                case "흰토끼":
                case "흰 토끼":
                    return "<color=#FFC0CB>[흰 토끼]</color>";

                case "체셔캣":
                case "체셔 캣":
                    return "<color=#ADD8E6>[체셔 캣]</color>";

                case "화로의 여인":
                case "여인":
                    return "<color=#E67E22>[화로의 여인]</color>";

                default:
                    return $"<color=white>[{cleanName}]</color>";
            }
        }

        // 지문 공용 색상 함수
        public static string ColorText(string text, string colorNameOrHex)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return $"<color={colorNameOrHex}>{text}</color>";
        }

        public static string Green(string text) => ColorText(text, "green");
        public static string Red(string text) => ColorText(text, "red");
        public static string Cyan(string text) => ColorText(text, "cyan");
        public static string Yellow(string text) => ColorText(text, "yellow");
        public static string Purple(string text) => ColorText(text, "#9B59B6");
    }
}