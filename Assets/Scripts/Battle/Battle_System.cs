namespace TextGame.Core
{
    public static class Battle_System
    {
        // 1. [탐지 판정]
        public static bool DET_Check(Player player, EnemyData enemy, out string message)
        {
            return Game_System.Stat_Check("탐지(DET)", enemy.detection, player.Detection, out message);
        }

        // 2. [이성 판정]
        public static CoC_Result SAN_Check(Player player, out string message)
        {
            CoC_Result result = Game_System.CoC_Check(player.Sanity, 0);

            switch (result)
            {
                case CoC_Result.Jackpot:
                    player.Will += 1;
                    message = Game_System.Green("[이성 판정 : 잭팟] 공포를 완벽하게 극복했습니다! (의지 +1)");
                    break;

                case CoC_Result.Success:
                    message = Game_System.Cyan("[이성 판정 : 성공] 마음을 차분히 가다듬었습니다.");
                    break;

                case CoC_Result.Failure:
                    int sanLoss = Game_System.Roll_D6();
                    player.Sanity -= sanLoss;
                    message = Game_System.Red($"[이성 판정 : 실패] 끔찍한 기운에 마음이 흔들립니다. (이성 -{sanLoss})");
                    break;

                case CoC_Result.Fumble:
                    int fumbleSan = Game_System.Roll_D6();
                    int fumbleMad = Game_System.Roll_D6();
                    player.Sanity -= fumbleSan;
                    player.Madness += fumbleMad;
                    message = Game_System.Purple($"[이성 판정 : 펌블] 미쳐버릴 것 같은 충격이 덮쳐옵니다! (이성 -{fumbleSan}, 광기 +{fumbleMad})");
                    break;

                default:
                    message = "";
                    break;
            }
            return result;
        }

        // 3. [거부 판정]
        public static bool REJ_Check(Player player, EnemyData enemy, out CoC_Result result)
        {
            result = Game_System.CoC_Check(player.Rejection, 0);

            if (result == CoC_Result.Jackpot || result == CoC_Result.Success)
            {
                return true;
            }

            player.Mental -= enemy.damage;
            return false;
        }

        // 4. [간섭 판정]
        public static int IFN_Check(Player player, EnemyData enemy, out CoC_Result result)
        {
            result = Game_System.CoC_Check(player.Interference, 0);
            if (result == CoC_Result.Jackpot) { enemy.maxHp -= 2; return 2; }
            if (result == CoC_Result.Success) { enemy.maxHp -= 1; return 1; }
            return 0;
        }
    }
}