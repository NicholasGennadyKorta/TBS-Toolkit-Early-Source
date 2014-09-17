using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class _Class {

    public string id,name;
    public string description;
    public int hpCap, strengthCap, magicCap, skillCap, speedCap, luckCap, defenceCap, resistanceCap;
    public int moveRange,moveRangeCap;
    
    public List<_Class> promotins = new List<_Class>();
}
