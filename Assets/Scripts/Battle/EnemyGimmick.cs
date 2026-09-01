using UnityEngine;

namespace TextGame.Core
{
    // 기믹이 발동할 전투 단계 타이밍
    public enum GimmickTriggerTime
    {
        OnSanityFail,    // 이성 판정 실패 시 (조우/기습 공포)
        OnAttackHit,     // 적 공격 적중 시 (플레이어 거부 방어 실패)
        OnEnemyDamaged,  // 플레이어 공격 적중 시 (적 피격 반사 피해 등)
        OnDeath          // 적 사망/처치 시 (자폭, 유언 등)
    }

    [CreateAssetMenu(fileName = "NewGimmick", menuName = "TextGame/Enemy Gimmick")]
    public class EnemyGimmick : ScriptableObject
    {
        [Header("발동 조건")]
        public GimmickTriggerTime triggerTime = GimmickTriggerTime.OnSanityFail;

        [Header("출력 메시지")]
        public string gimmickName = "특수 기믹";
        [TextArea(2, 3)]
        public string effectDescription = "기괴한 영향으로 피해를 입습니다.";

        [Header("스탯 변동 수치 (양수: 증가, 음수: 감소)")]
        public int mentalChange = 0;   // 정신력
        public int sanityChange = 0;   // 이성
        public int madnessChange = 0;  // 광기
        public int willChange = 0;     // 의지

        // 기믹 발동 및 효과 텍스트 반환
        public string Execute(Player player)
        {
            player.Mental += mentalChange;
            player.Sanity += sanityChange;
            player.Madness += madnessChange;
            player.Will += willChange;

            // 해로운 효과는 빨간색, 이로운 효과는 초록색으로 표시
            bool isDebuff = (mentalChange < 0 || sanityChange < 0 || madnessChange > 0 || willChange < 0);
            string colorTag = isDebuff ? "#FF5555" : "#55FF55";

            return $"<color={colorTag}>[{gimmickName}] {effectDescription}</color>";
        }
    }
}