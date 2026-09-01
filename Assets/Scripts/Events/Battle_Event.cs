using UnityEngine;

namespace TextGame.Core
{
    public class Battle_Event
    {
        private Player player;
        private EnemyData enemy;
        private GameManager manager;

        // [외부 진입점] Resources/Enemies 안의 적 중 하나를 무작위 추첨
        public static EventStep Random_Battle(Player player)
        {
            EnemyData[] allEnemies = Resources.LoadAll<EnemyData>("Enemies");

            if (allEnemies == null || allEnemies.Length == 0)
            {
                return new EventStep(
                    title: "[전투 조우]",
                    bodyText: """
                        안개 속에서 기척이 느껴졌으나 곧 사라졌습니다.
                        """,
                    opt1: "계속 나아간다 ▶",
                    opt2: null,
                    onOption1: (p, mgr) => mgr.Show_Main_UI()
                );
            }

            EnemyData selectedEnemyTemplate = allEnemies[Random.Range(0, allEnemies.Length)];
            EnemyData battleEnemy = selectedEnemyTemplate.CreateInstance();

            return new EventStep(
                title: $"[조우 : {battleEnemy.enemyName}]",
                bodyText: $$"""
                    {{battleEnemy.encounterText}}

                    짙은 안개 속에서 정체불명의 섬뜩한 기척이 당신을 향해 다가옵니다!
                    적이 당신을 인지하기 전에 먼저 기척을 살펴야 합니다.
                    """,
                opt1: "1. 기척을 탐지한다 [탐지(DET) 판정]",
                opt2: null,
                onOption1: (p, mgr) =>
                {
                    Battle_Event battle = new Battle_Event(p, battleEnemy, mgr);
                    battle.Detection_Check();
                }
            );
        }

        public Battle_Event(Player player, EnemyData enemyInstance, GameManager manager = null)
        {
            this.player = player;
            this.enemy = enemyInstance;
            this.manager = manager;
        }

        public EnemyData GetEnemy() => enemy;

        // 1. [탐지(DET) 판정]
        public void Detection_Check()
        {
            bool isSuccess = Battle_System.DET_Check(player, enemy, out string message);

            if (isSuccess)
            {
                EventStep step = new EventStep(
                    title: $"[탐지 성공 - {enemy.enemyName}]",
                    bodyText: $$"""
                        {{message}}

                        기척을 먼저 감지했습니다! 어둠 속에서 {{enemy.enemyName}}의 형체가 선명하게 보입니다.
                        적이 당신을 알아채지 못한 지금, 어떻게 행동하시겠습니까?
                        """,
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
                    title: $"[탐지 실패! - {enemy.enemyName}]",
                    bodyText: $$"""
                        {{message}}

                        {{enemy.ambushText}}
                        기습적인 조우로 인해 강렬한 공포가 들이닥칩니다!
                        """,
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
            CoC_Result result = Battle_System.SAN_Check(player, out string sanMessage);

            if (result == CoC_Result.Failure || result == CoC_Result.Fumble)
            {
                enemy.TriggerGimmick(player, result);
            }

            if (manager.CheckPlayerDeath()) return;

            bool isSuccess = (result == CoC_Result.Jackpot || result == CoC_Result.Success);

            if (isSuccess)
            {
                EventStep step = new EventStep(
                    title: $"[이성 판정 진행]",
                    bodyText: $$"""
                        {{sanMessage}}

                        미지의 존재가 주는 섬뜩한 공포 속에서도 당신은 마음을 다잡았습니다.
                        정신을 가다듬고 적에게 맞설 준비를 합니다.
                        """,
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
                    bodyText: $$"""
                        {{sanMessage}}

                        기괴한 광경에 정신이 흔들립니다!
                        공포에 휩싸인 당신에게 {{enemy.enemyName}}의 공격이 사정없이 밀려듭니다.
                        """,
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
            bool isSuccess = Battle_System.REJ_Check(player, enemy, out CoC_Result result);

            if (manager.CheckPlayerDeath()) return;

            if (isSuccess)
            {
                EventStep step = new EventStep(
                    title: $"[거부 성공! ({result})]",
                    bodyText: $$"""
                        '자신의 꿈을 둘러 외부의 간섭을 거절한다.'

                        강한 의지로 방어벽을 세워 {{enemy.enemyName}}의 공격을 완벽히 튕겨냈습니다!
                        적의 빈틈이 보입니다.
                        """,
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
                    bodyText: $$"""
                        '자신의 꿈을 둘러 외부의 간섭을 거절한다.'

                        {{enemy.attackText}}
                        적의 공격을 막아내지 못했습니다!
                        {{Game_System.Red($"[정신력 -{enemy.damage}]")}} (남은 정신력: {{player.Mental}})

                        비틀거리면서도 전열을 가다듬습니다.
                        """,
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
            int damage = Battle_System.IFN_Check(player, enemy, out CoC_Result result);

            if (damage > 0)
            {
                if (enemy.maxHp <= 0)
                {
                    Win_Battle();
                    return;
                }

                string successMsg = (result == CoC_Result.Jackpot)
                    ? $"{Game_System.Green("[대성공!]")} 꿈의 법칙을 강력하게 비틀어 {enemy.enemyName}에게 큰 충격을 주었습니다! (데미지: 2)"
                    : $"{Game_System.Green("[간섭 성공]")} 현실을 비틀어 {enemy.enemyName}에게 타격을 입혔습니다. (데미지: 1)";

                EventStep step = new EventStep(
                    title: $"[간섭 성공! ({result})]",
                    bodyText: $$"""
                        '결국 이 모든 것이 꿈이라면 나도 간섭할 수 있지 않을까?'

                        {{successMsg}}
                        {{enemy.hitText}}
                        분노한 {{enemy.enemyName}}이(가) 반격을 준비합니다!
                        """,
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
                    bodyText: $$"""
                        '결국 이 모든 것이 꿈이라면 나도 간섭할 수 있지 않을까?'

                        꿈에 간섭하려 했으나 집중이 흐트러져 실패했습니다!
                        공격권이 넘어가 {{enemy.enemyName}}이(가) 들이닥칩니다!
                        """,
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
                title: $"[전투 승리! - {enemy.enemyName}]",
                bodyText: $$"""
                    {{enemy.deathText}}

                    {{Game_System.Green($"[전리품 획득] 별빛 +{starlight} | 의지 +{will}")}}
                    """,
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
                bodyText: $$"""
                    {{enemy.enemyName}}(으)로부터 무사히 가로질러 안개 속으로 도망쳤습니다.
                    """,
                opt1: "탐험 계속하기 ▶",
                opt2: null,
                onOption1: (p, mgr) => mgr.Show_Main_UI()
            );
            manager.Execute_EventStep(step);
        }
    }
}