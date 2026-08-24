using UnityEngine;

namespace TextGame.Core
{
    public class Battle_Event
    {
        private Player player;
        private Enemy enemy;
        private GameManager manager;

        public Battle_Event(Player player, EnemyType enemyType, GameManager manager = null)
        {
            this.player = player;
            this.enemy = EnemyDatabase.GetEnemy(enemyType); // Factory 적용
            this.manager = manager;
        }

        public Enemy GetEnemy() => enemy;

        // 1. [탐지(DET) 판정]
        public void Detection_Check()
        {
            string message;
            bool isSuccess = Battle_System.DET_Check(player, enemy, out message);

            if (isSuccess)
            {
                EventStep step = new EventStep(
                    title: $"[탐지 성공 - {enemy.Name}]",
                    bodyText: $"{message}\n\n" +
                              $"기척을 먼저 감지했습니다! 어둠 속에서 {enemy.Name}의 형체가 선명하게 보입니다.\n" +
                              $"적이 당신을 알아채지 못한 지금, 어떻게 행동하시겠습니까?",
                    opt1: "[간섭(IFN) 판정]",
                    opt2: "[도망치기]",
                    onOption1: (p, mgr) => Interference_Check(),
                    onOption2: (p, mgr) => Escape_Battle()
                );
                manager.Execute_EventStep(step);
            }
            else
            {
                EventStep step = new EventStep(
                    title: $"[탐지 실패! - {enemy.Name}]",
                    bodyText: $"{message}\n\n" +
                              $"기척을 느끼지 못했습니다! 어둠 속에서 갑작스럽게 {enemy.Name}이(가) 튀어나옵니다.\n" +
                              $"기습적인 조우로 인해 강렬한 공포가 들이닥칩니다!",
                    opt1: "이성(SAN) 판정 진행 ▶",
                    opt2: null,
                    onOption1: (p, mgr) => Sanity_Check()
                );
                manager.Execute_EventStep(step);
            }
        }

        // 2. [이성(SAN) 판정]
        public void Sanity_Check()
        {
            string sanMessage;
            CoC_Result result = Battle_System.SAN_Check(player, out sanMessage);

            // 적의 고유 이성 실패 패널티 발동 (예: 도플갱어 저주)
            if (result == CoC_Result.Failure || result == CoC_Result.Fumble)
            {
                enemy.Doppelganger_Curse(player, result);
            }

            // 사망 체크
            if (manager.CheckPlayerDeath()) return;

            bool isSuccess = (result == CoC_Result.Jackpot || result == CoC_Result.Success);

            if (isSuccess)
            {
                EventStep step = new EventStep(
                    title: $"[이성 판정 진행]",
                    bodyText: $"{sanMessage}\n\n" +
                              $"미지의 존재가 주는 섬뜩한 공포 속에서도 당신은 마음을 다잡았습니다.\n" +
                              $"정신을 가다듬고 적에게 맞설 준비를 합니다.",
                    opt1: "[간섭(IFN) 판정]",
                    opt2: "[도망치기]",
                    onOption1: (p, mgr) => Interference_Check(),
                    onOption2: (p, mgr) => Escape_Battle()
                );
                manager.Execute_EventStep(step);
            }
            else
            {
                EventStep step = new EventStep(
                    title: $"[이성 판정 진행]",
                    bodyText: $"{sanMessage}\n\n" +
                              $"기괴한 광경에 정신이 흔들립니다!\n" +
                              $"공포에 휩싸인 당신에게 {enemy.Name}의 공격이 사정없이 밀려듭니다.",
                    opt1: "[거부(REJ) 판정 진행 ▶]",
                    opt2: null,
                    onOption1: (p, mgr) => Rejection_Check()
                );
                manager.Execute_EventStep(step);
            }
        }

        // 3. [거부(REJ) 판정]
        public void Rejection_Check()
        {
            CoC_Result result;
            bool isSuccess = Battle_System.REJ_Check(player, enemy, out result);

            if (manager.CheckPlayerDeath()) return;

            if (isSuccess)
            {
                EventStep step = new EventStep(
                    title: $"[거부 성공! ({result})]",
                    bodyText: $"'자신의 꿈을 둘러 외부의 간섭을 거절한다.'\n\n" +
                              $"강한 의지로 방어벽을 세워 {enemy.Name}의 공격을 완벽히 튕겨냈습니다!\n" +
                              $"적의 빈틈이 보입니다.",
                    opt1: "[간섭(IFN) 판정]",
                    opt2: "[도망치기]",
                    onOption1: (p, mgr) => Interference_Check(),
                    onOption2: (p, mgr) => Escape_Battle()
                );
                manager.Execute_EventStep(step);
            }
            else
            {
                EventStep step = new EventStep(
                    title: $"[거부 실패... ({result})]",
                    bodyText: $"'자신의 꿈을 둘러 외부의 간섭을 거절한다.'\n\n" +
                              $"적의 기습적인 공격을 막아내지 못했습니다!\n" +
                              $"{Game_System.Red($"[정신력 -{enemy.Damage}]")} (남은 정신력: {player.Mental})\n\n" +
                              $"비틀거리면서도 전열을 가다듬습니다.",
                    opt1: "[간섭(IFN) 판정]",
                    opt2: "[도망치기]",
                    onOption1: (p, mgr) => Interference_Check(),
                    onOption2: (p, mgr) => Escape_Battle()
                );
                manager.Execute_EventStep(step);
            }
        }

        // 4. [간섭(IFN) 판정]
        public void Interference_Check()
        {
            CoC_Result result;
            int damage = Battle_System.IFN_Check(player, enemy, out result);

            if (damage > 0)
            {
                if (enemy.HP <= 0)
                {
                    Win_Battle();
                    return;
                }

                string successMsg = (result == CoC_Result.Jackpot)
                    ? $"{Game_System.Green("[대성공!]")} 꿈의 법칙을 강력하게 비틀어 {enemy.Name}에게 큰 충격을 주었습니다! (데미지: 2)"
                    : $"{Game_System.Green("[간섭 성공]")} 현실을 비틀어 {enemy.Name}에게 타격을 입혔습니다. (데미지: 1)";

                EventStep step = new EventStep(
                    title: $"[간섭 성공! ({result})]",
                    bodyText: $"'결국 이 모든 것이 꿈이라면 나도 간섭할 수 있지 않을까?'\n\n" +
                              $"{successMsg}\n" +
                              $"분노한 {enemy.Name}이(가) 반격을 준비합니다!",
                    opt1: "[거부(REJ) 판정 진행 ▶]",
                    opt2: null,
                    onOption1: (p, mgr) => Rejection_Check()
                );
                manager.Execute_EventStep(step);
            }
            else
            {
                EventStep step = new EventStep(
                    title: $"[간섭 실패... ({result})]",
                    bodyText: $"'결국 이 모든 것이 꿈이라면 나도 간섭할 수 있지 않을까?'\n\n" +
                              $"꿈에 간섭하려 했으나 집중이 흐트러져 실패했습니다!\n" +
                              $"공격권이 넘어가 {enemy.Name}이(가) 들이닥칩니다!",
                    opt1: "[거부(REJ) 판정 진행 ▶]",
                    opt2: null,
                    onOption1: (p, mgr) => Rejection_Check()
                );
                manager.Execute_EventStep(step);
            }
        }

        // 승리 및 도망
        private void Win_Battle()
        {
            int starlight = Game_System.Roll_D6();
            int will = Game_System.Roll_D6();

            player.Starlight += starlight;
            player.Will += will;

            EventStep step = new EventStep(
                title: $"[전투 승리! - {enemy.Name}]",
                bodyText: $"{enemy.Name}의 형체가 흐릿해지더니 안개 속으로 산산이 부서져 사라집니다.\n\n" +
                          $"{Game_System.Green($"[전리품 획득] 별빛 +{starlight} | 의지 +{will}")}",
                opt1: "탐험 계속하기 ▶",
                opt2: null,
                onOption1: (p, mgr) => mgr.Show_Main_UI()
            );
            manager.Execute_EventStep(step);
        }

        public void Escape_Battle()
        {
            EventStep step = new EventStep(
                title: "[전투 이탈]",
                bodyText: $"{enemy.Name}(으)로부터 무사히 가로질러 안개 속으로 도망쳤습니다.",
                opt1: "탐험 계속하기 ▶",
                opt2: null,
                onOption1: (p, mgr) => mgr.Show_Main_UI()
            );
            manager.Execute_EventStep(step);
        }
    }
}