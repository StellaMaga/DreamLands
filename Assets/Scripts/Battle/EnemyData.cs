using UnityEngine;

namespace TextGame.Core
{
    public enum SpecialGimmickType
    {
        None,
        DoppelgangerCurse,
        Bleeding,
        Terror
    }

    [CreateAssetMenu(fileName = "NewEnemy", menuName = "TextGame/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("기본 정보")]
        public string enemyId;
        public string enemyName;

        [Header("스탯")]
        [Range(1, 100)] public int detection = 40;
        public int maxHp = 2;
        public int damage = 5;

        [Header("특수 기믹")]
        public SpecialGimmickType gimmick = SpecialGimmickType.None;

        [Header("상황별 고유 지문")]
        [TextArea(2, 4)] public string encounterText;  // 조우 지문
        [TextArea(2, 4)] public string ambushText;     // 탐지 실패(기습) 시 지문
        [TextArea(2, 4)] public string attackText;     // 적이 플레이어를 공격할 때 지문
        [TextArea(2, 4)] public string hitText;        // 적이 플레이어에게 피격당할 때 지문
        [TextArea(2, 4)] public string deathText;      // 적 처치(승리) 시 지문

        public EnemyData CreateInstance()
        {
            return Instantiate(this);
        }

        public void TriggerGimmick(Player player, CoC_Result result)
        {
            switch (gimmick)
            {
                case SpecialGimmickType.DoppelgangerCurse:
                    player.Sanity -= 5;
                    player.Madness += 3;
                    break;
            }
        }
    }
}