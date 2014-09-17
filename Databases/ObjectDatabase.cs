using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ObjectDatabase : MonoBehaviour {
    public List<GameObject> units = new List<GameObject>();
    public List<GameObject> terrain = new List<GameObject>();
    public List<Weapon> weapons = new List<Weapon>();
    public List<Consumable> consumables = new List<Consumable>();

    public void Awake()
    {
        for (int i = 0; i < weapons.Count; ++i)
            weapons[i].currentDurability = weapons[i].durability;
        for (int i = 0; i < consumables.Count; ++i)
            consumables[i].currentDurability = consumables[i].durability;
    }

    public GameObject GetUnit(string name)
    {
        for (int i = 0; i < units.Count; ++i)
        {
            if (units[i].name == name)
                return units[i];
        }
        return null;
    }

    public GameObject GetTerrain(string name)
    {
        for (int i = 0; i < terrain.Count; ++i)
        {
            if (terrain[i].name == name)
                return terrain[i];
        }
        return null;
    }

    public Weapon GetWeapon(string id)
    {
        for (int i = 0; i < weapons.Count; ++i)
        {
            if (weapons[i].id == id)
                return weapons[i];
        }
        return null;
    }

    public Consumable GetConsumable(string id)
    {
        for (int i = 0; i < consumables.Count; ++i)
        {
            if (consumables[i].id == id)
                return consumables[i];
        }
        return null;
    }

}

