namespace ND
{
    public enum E_CharacterState
    { 
        Idle = 0,
        Run,
        Death,
        Attack
    }

    public enum E_HeroType
    { 
        Doctor = 0,
        Girl,
        Boy,
        Police,
        Soldier,
        NotHero
    }

    public enum E_MonsterType
    { 
        Zombie = 0,
        Soldier_A,
        Soldier_B,
        Soldier_C,
        NotMonster       
    }

    public enum E_UpgradeTargetType
    { 
        AllHero = 0,
        AttackerOnly,
        RangerOnly,
        HealerOnly,
        ForwardOnly,
        BackWardOnly,
        UserMoney,
        FreeReRoll
    }

    public enum E_HeroStatType
    { 
        Atk = 0,
        MaxHp,
        CurrentHp,
        Armor,
        Dps,
        Range,
        All,
        NotStat
    }
}