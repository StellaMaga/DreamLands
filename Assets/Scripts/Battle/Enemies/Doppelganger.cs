using UnityEngine;

namespace TextGame.Core
{
    public class Doppelganger : Enemy
    {
        public Doppelganger()
        {
            Name = "도플갱어";
            Description = """
                반죽으로 어중간하게 인간을 따라한 듯한 괴물,
                보는 것만으로도 역겨움이 몰려온다.
                """;
            Ability = "[도플갱어의 저주: 이성 판정 실패 시 정신력 추가 감소]";
            Ego = 1;
            Detection = Game_System.Roll_3D6() * 4;
            HP = 1;
            Damage = 13;
            DeathMessage = """
                [ 데드엔딩 : 거울 속의 박제 ]
                도플갱어는 당신의 형태를 완벽히 집어삼켰습니다.
                진짜 당신이 누구였는지조차 잊혀진 채, 당신은 자리를 빼앗겼습니다.
                """;
        }

        // 도플갱어 특성
        public override void Doppelganger_Curse(Player player, CoC_Result result)
        {
            if (result == CoC_Result.Failure)
            {
                player.Mental -= 1;
                Debug.Log(Game_System.Red("[인간을 모방한 반죽은 보기만 해도 속이 울렁거렸다 (정신력 -1)]"));
            }
            else if (result == CoC_Result.Fumble)
            {
                player.Mental -= 7;
                Debug.Log(Game_System.Red("[한 세상에 같은 존재가 둘이나 존재해선 안 된다... (정신력 -7)]"));
            }
        }
    }
}