using System.Collections.Generic;
using UnityEngine;
using TextGame.Core;

namespace TextGame.Events.Mini
{
    public static class Common_Mini_Events
    {
        private static readonly List<MiniEvent> miniEventPool = new();

        /// <summary>
        /// 공용 미니 이벤트 풀 초기화
        /// </summary>
        public static void Initialize()
        {
            miniEventPool.Clear();

            // ==========================================
            // 1. 단순 분위기 묘사 지문 (스탯 변화 없음)
            // ==========================================
            miniEventPool.Add(new("안개는 여전히 자욱하다. 어디선가 메마른 풀잎 냄새가 바람을 타고 흐른다."));
            miniEventPool.Add(new("이 장소는 도대체 어떻게 만들어졌을까? 걸어도 걸어도 끝이 보이지 않는다."));
            miniEventPool.Add(new("안개 너머로 기괴한 형상의 그림자가 스쳐 지나갔지만, 눈을 깜빡이자 흔적도 없이 사라졌다."));
            miniEventPool.Add(new("발밑에서 바스락거리는 소리가 났다. 메마른 나뭇가지가 밟혀 부서진 소리였다."));
            miniEventPool.Add(new("멀리서 종소리 같은 공명이 낮게 울려 퍼진다. 어디서 시작된 소리인지 가늠할 수 없다."));
            miniEventPool.Add(new("잠시 걸음을 멈추고 주위를 둘러보았지만, 잿빛 풍경만이 끝없이 펼쳐져 있다."));
            miniEventPool.Add(new("폐허의 틈새로 부는 바람이 마치 누군가의 낮은 한숨 소리처럼 들린다."));
            miniEventPool.Add(new("머리 위로 먹구름이 낮게 깔려 있다. 비는 내리지 않지만 공기가 눅눅하게 젖어 있다."));
            miniEventPool.Add(new("어디선가 익숙한 멜로디가 귓가를 맴돌다 이내 사라졌다. 옛 기억의 조각일까?"));
            miniEventPool.Add(new("손을 뻗어 안개를 쥐어보려 했지만, 차가운 수증기만이 손가락 사이로 빠져나갔다."));

            // ==========================================
            // 2. 이로운 지문 (스탯/재화 증가)
            // ==========================================
            miniEventPool.Add(new(
                $"길을 걷다 무심코 내려다본 바닥에 새하얀 네잎클로버가 피어 있었다. {Game_System.Green("[별빛 +1]")}",
                player => player.Starlight += 1
            ));

            miniEventPool.Add(new(
                $"돌 틈 사이에서 맑고 시원한 샘물이 솟아난다. 목을 축이자 가슴이 상쾌해진다. {Game_System.Green("[정신력 +2]")}",
                player => player.Mental += 2
            ));

            miniEventPool.Add(new(
                $"바람을 타고 은은하고 달콤한 꽃향기가 번져와 지친 마음을 잠시 달래준다. {Game_System.Green("[이성 +1]")}",
                player => player.Sanity += 1
            ));

            miniEventPool.Add(new(
                $"흙먼지 속에 반쯤 파묻힌 희미한 결정을 발견했다. 손에 쥐자 따스한 온기가 감돈다. {Game_System.Green("[별빛 +2]")}",
                player => player.Starlight += 2
            ));

            miniEventPool.Add(new(
                $"가볍게 스트레칭을 하며 호흡을 가다듬었다. 몸이 조금 가벼워진 기분이다. {Game_System.Green("[체력 +3]")}",
                player => player.Sanity += 3
            ));

            // ==========================================
            // 3. 해로운 지문 (스탯 감소 / 위험)
            // ==========================================
            miniEventPool.Add(new(
                $"차디찬 삭풍이 옷깃을 매섭게 파고든다. 뼛속까지 시린 한기에 몸이 떨린다. {Game_System.Red("[정신력 -1]")}",
                player => player.Mental -= 1
            ));

            miniEventPool.Add(new(
                $"안개 속에서 알 수 없는 속삭임이 귓전을 파고든다. 두통이 지끈거린다. {Game_System.Red("[이성 -1]")}",
                player => player.Sanity -= 1
            ));

            miniEventPool.Add(new(
                $"돌부리에 발이 걸려 앞으로 비틀거렸다. 발목이 욱신거린다. {Game_System.Red("[체력 -2]")}",
                player => player.Sanity -= 2
            ));

            miniEventPool.Add(new(
                $"주머니에 작은 구멍이 뚫려 있었다! 희미한 빛 조각 하나가 바닥으로 흘러내려 흩어졌다. {Game_System.Red("[별빛 -1]")}",
                player =>
                {
                    if (player.Starlight > 0) player.Starlight -= 1;
                }
            ));
        }

        /// <summary>
        /// 무작위 미니 이벤트 추첨 (중복 허용)
        /// </summary>
        public static MiniEvent Get_Random(Player player)
        {
            if (miniEventPool.Count == 0)
            {
                Initialize();
            }

            int randomIndex = Random.Range(0, miniEventPool.Count);
            return miniEventPool[randomIndex];
        }
    }
}