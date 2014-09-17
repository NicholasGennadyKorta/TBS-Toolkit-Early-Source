using UnityEngine;
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;

[Serializable()]
public class Weapon : Item {

    [XmlIgnore] public Mesh mesh;
    [XmlIgnore] public Material material;
    [HideInInspector] public string meshName;
    public int weaponType;
    public int weaponLevel;
    public int might;
    public float hit;
    public float criticle;
    public int range;
    public int weight;
    public int weaponExperiance;
    public bool attackDiagonally;

    public Weapon()
    {
        itemType = ItemType.WEAPON;
    }
}
