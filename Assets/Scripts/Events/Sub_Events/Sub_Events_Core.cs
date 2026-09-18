using System;
using TextGame.Core;

namespace TextGame.Events.Shallow_Dream
{
    // [1장 얕은 꿈 무작위 서브 이벤트 추첨소]
    public partial class Sub_Events
    {
        private static System.Random rand = new System.Random();

        public static EventStep Random_Event(Player player)
        {
            int eventIndex = rand.Next(1, 14);

            switch (eventIndex)
            {
                case 1: return Sub_Event_01_Floret();
                case 2: return Sub_Event_02_Butterfly(player);
                case 3: return Sub_Event_03_Road_Of_Past(player);
                case 4: return Sub_Event_04_entireness_orb(player);
                case 5: return Sub_Event_05_Three_Trees(player);
                case 6: return Sub_Event_06_Subway(player);
                case 7: return Sub_Event_07_Lost_Name(player);
                case 8: return Sub_Event_08_Blood_Bathtub(player);
                case 9: return Sub_Event_09_Violin(player);
                case 10: return Sub_Event_10_Picture(player);
                case 11: return Sub_Event_11_Kraken(player);
                case 12: return Sub_Event_12_Smoker(player);
                case 13: return Sub_Event_13_Suicide_Theory();
                default: return Sub_Event_01_Floret();
            }
        }
    }
}