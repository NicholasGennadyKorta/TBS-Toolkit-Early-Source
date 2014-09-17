using UnityEngine;
using System.Collections;

[System.Serializable]
public class Consumable : Item
{
    public int hpRestoreAmount;
    public statsIncreases statIncreases = new statsIncreases();
  
    public Consumable()
    {
        itemType = ItemType.CONSUMABLE;
    }

    public void Consume(Unit unit)
    {
        unit.stats.currentHp += hpRestoreAmount;
        if (unit.stats.currentHp > unit.stats.hp)
            unit.stats.currentHp = unit.stats.hp;

        currentDurability--;
    }

    [System.Serializable]
    public class statsIncreases
    {
        public int xp;
        public int hp,strength,magic,skill,speed,luck,defence,resistance,constitution;
        public float hpGrowthRate, strengthGrowthRate, magicGrowthRate, skillGrowthRate, speedGrowthRate, luckGrowthRate, defenceGrowthRate, resistanceGrowthRate;
        public int moveRange;
    }
}
