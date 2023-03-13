using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define : MonoBehaviour
{
    public enum Weapon
    {
        환도,     // 환도Hwando
        월도,      // 월도Waldo
        창,      // 창Spear
        돌팔매,      // 돌팔매Stone
        승자총통,    // 승자총통Seungja
        현자총통,    // 현자총통Hyeonja
        천자총통,    // 천자총통Cheonja
        석회가루, // 석회가루LimePowder
        조총,    // 조총Jochong
        석궁,  // 석궁Seokgoong
        화차,     // 문종화차Hwacha
        각궁,   // 각궁Gakgoong
        철퇴,// MaceWarning
        편곤,   // 편곤Pyeongon
        
        방패,     // 방패Shield
        깃발,       // 깃발Flag
        신장대, // 신장대ShamanWand
        왜검, // 왜검JapanSword
        
        철퇴연계,       // 철퇴Mace
        편곤연계, // 편곤폭발PyeongonExplosion
        end,

        // 진화무기
        별운검 = 21,     // 환도
        협도,       // 월도
        은입사철퇴, // 철퇴
        야태도,     // 왜검
        당파,       // 창
        마상편곤,   // 편곤
        각궁_불화살,          
        승자총통_삼연자포,
        현자총통_비격진천뢰,
        천자총통_대장군전,
        철질려,     // 석회가루
        석궁_불화살,
        변이중화차, // 화차
        은입사철퇴연계,
        마상편곤연계,
        각궁_불화살연계,
        석궁_불화살연계,
        현자총통_비격진천뢰연계,
        upgradedWeaponEnd
    }

    public enum CharacterStat
    {
        공격크기,               // Size
        투사체속도,              // Speed
        넉백,              // Force
        데미지,             // Damage
        추가투사체확률, // ProjectilePercentage
        관통횟수,   // PenetrationCount
        지속시간,           // Duration
        방어도,            // Defense
        최대체력,              // MaxHP
        쿨타임감소,           // CoolTime
        이동속도,
        end,
        
        // 무기에 붙지 않는 옵션
        size,
        clashDamage,
    }

    public enum Enemy
    {
        NormalEnemy,
        Sword_infantry,
        Sword_Boss,
        Spear_Boss,
        Bow_Boss,
        Chair_Boss,
        Arrows,
        Healer,
        Ninja
    }

    public enum Exp
    {
        SmallExp,
    }

    public enum Character
    {
        Player,
        Follower1,
        Follower2,
    }

    public enum Mastery // 숙련도
    {
        도검,      // 도검
        둔기,       // 둔기
        개인화기,    // 개인화기
        대포,     // 대포
        방어구,      // 방어구
        end
    }

    public enum FloatingDmg
    {
        FloatingDmg,
    }

    public enum CharacterName
    {
        swordman,
        seung,
        flag,
        hwacha,
        spearman,
        shaman,
        stonesling,
        monk,
        bow,
        shieldman,
        mace,
        cheonja,
        flail,
        samurai,
        end
    }

    public enum QuestRewardStat
    {
        쓸만한,
        용감한,
        앙심깊은,
        칭송받는,
        선비,
        분노한,
        날렵한,
        악명높은,
        피에굶주린,
        건장한,
        투박한,
        명사수,
        복수귀,
        정예,
        괴팍한,
        책사,
        의병장,
        골목대장,
        큰손,
        구원자,
        굴지의,
        역전용사,
        영웅,
        미친,
        사또,
        도깨비,
        한량,
        협객,
        등용부위,
        선인,
        어사,
        해탈,
        능구렁이,
        적순부위,
        늙은,
        인간백정,
        천하장사,
        조방장,
        성인

    }

    public enum army_name
    {
        초소카베,
        가토,
        고니시,
        구로다,
        쿠바야카와,
        오토모,
        시나즈,
        소,
        우키다
    }
}
