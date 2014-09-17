using UnityEngine;
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;

[Serializable()]
public class Item {
    public string id;
    public string name;
      [XmlIgnore]
    public Sprite icon;
    public string description;
    public int worth;
    public int durability;
    [HideInInspector] public int currentDurability;

    [HideInInspector] public enum ItemType {
        CONSUMABLE,
        WEAPON
    }
    [HideInInspector] public ItemType itemType;
}
