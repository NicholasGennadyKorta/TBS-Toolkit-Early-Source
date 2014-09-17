using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour {

   
    public int playerNumb;
    public string name;
    public string classId;
    public string description;

    public Inventory inventory = new Inventory();
    public GUI gui = new GUI();
    public Stats stats = new Stats();

    [HideInInspector] public _Class unitClass = new _Class();
    [HideInInspector] public bool moved;

    GameObject leftHandItem, rightHandItem;

	// Use this for initialization
	void Start () {
        unitClass = ObjectPool.classDatabase.GetByID(classId);
        SetStats();
        FindChildWithName(transform);
      //  GetComponent<Animator>().Play("Idle", 0, UnityEngine.Random.Range(0.0f, 1.0f));

        EquipWeapon(0);
    
    }

    void SetStats()
    {
        stats.hp = Mathf.Clamp(stats.hp, 0, unitClass.hpCap);
        stats.currentHp = Mathf.Clamp(stats.currentHp, 1, stats.hp);
        stats.strength = Mathf.Clamp(stats.strength, 0, unitClass.strengthCap);
        stats.magic = Mathf.Clamp(stats.magic, 0, unitClass.strengthCap);
        stats.skill = Mathf.Clamp(stats.skill, 0, unitClass.skillCap);
        stats.speed = Mathf.Clamp(stats.speed, 0, unitClass.speedCap);
        stats.luck = Mathf.Clamp(stats.luck, 0, unitClass.luckCap);
        stats.defence = Mathf.Clamp(stats.defence, 0, unitClass.defenceCap);
        stats.resistance = Mathf.Clamp(stats.resistance, 0, unitClass.resistanceCap);
        stats.moveRange = Mathf.Clamp(stats.moveRange, unitClass.moveRange, unitClass.moveRangeCap);
    }

    void FindChildWithName(Transform _transform)
    {
        foreach (Transform child in _transform)
        {
            if (child.name == "LeftHandItem")
            {
                //child.gameObject.AddComponent<Weapon>();
               
            }
            if (child.name == "ItemRightHand")
            {
                rightHandItem = child.gameObject;
            }
            FindChildWithName(child);
        }
    }

	// Update is called once per frame
	void Update () 
    {
	    if (stats.currentHp <= 0)
        {
            GetComponent<Animator>().SetTrigger("dying");
            ObjectPool.units.Remove(this);
            transform.FindChild("HealthBarGUI").gameObject.SetActive(false);
            SetMoved(false);
        }
	}

    public void SetGridPosition(Vector2 gridPosition)
    {
        this.GetComponent<Mover>().gridPosition = gridPosition;
        transform.position = GridGraph.instance.pathNodes[(int)gridPosition.x, (int)gridPosition.y].position;
    }

    public void SetMoved(bool moved)
    {
        this.moved = moved;
        if (moved)
        {
            foreach (Transform child in transform)
            {
                if (child.renderer != null)
                {
                    Shader shader = Shader.Find("Diffuse");

                    for (int i = 0; i < child.renderer.materials.Length; ++i)
                    {
                        child.renderer.materials[i].shader = shader;
                        child.renderer.materials[i].SetColor("MainTex", Color.black);
                    }

                }
            }
        }
        else
        {
            foreach (Transform child in transform)
            {
                if (child.renderer != null)
                {
                    Shader shader = Shader.Find("Mobile/Unlit (Supports Lightmap)");
        
                    for (int i = 0; i < child.renderer.materials.Length; ++i)
                    {
                        child.renderer.materials[i].shader = shader;
                        child.renderer.materials[i].color = Color.gray;
                    }
        
                }
            }
        }
    }

    public void EquipWeapon(int i)
    {
        inventory.equipedWeapon = (Weapon)inventory.items[i];
        rightHandItem.GetComponent<MeshFilter>().mesh = inventory.equipedWeapon.mesh;
        rightHandItem.GetComponent<MeshFilter>().renderer.material = inventory.equipedWeapon.material;

        Item item = inventory.items[i];
        inventory.items.RemoveAt(i);
        inventory.items.Insert(0, item);
    }

    public void DequipWeapon()
    {
        inventory.equipedWeapon = null;
        rightHandItem.GetComponent<MeshFilter>().mesh = null;
        rightHandItem.GetComponent<MeshFilter>().renderer.material = null;
    }

    public void DiscardWeapon(int i)
    {
        if (inventory.items[i] == inventory.equipedWeapon)
        {
            inventory.equipedWeapon = null;
            rightHandItem.GetComponent<MeshFilter>().mesh = null;
            rightHandItem.GetComponent<MeshFilter>().renderer.material = null;
        }

        inventory.items.RemoveAt(i);
    }

    public void ConsumeItem(int i)
    {
        Consumable consumable = (Consumable)inventory.items[i];
        consumable.Consume(this);
    }

    [System.Serializable]
    public class Inventory
    {
        public List<Item> items;
        public Weapon equipedWeapon;
    }

    [System.Serializable]
    public class Outfit
    {
        public Weapon leftHandItem;
        public Weapon rightHandItem;
    }

    [System.Serializable]
    public class GUI
    {
        public Sprite profile;
    }

    [System.Serializable]
    public class Stats
    {
        public int level;
        public int xp;
        public float hp;
        public float currentHp;
        public int strength, magic, skill, speed, luck, defence, resistance, constitution;
        [HideInInspector] public int strengthBounus, magicBounus, skillBounus, speedBounus, luckBounus, defenceBounus, resistanceBounus;
        public float hpGrowthRate, strengthGrowthRate, magicGrowthRate, skillGrowthRate, speedGrowthRate, luckGrowthRate, defenceGrowthRate, resistanceGrowthRate;
        [HideInInspector] public int moveRange;
        [HideInInspector] public int moveRageBounus;
        [HideInInspector] public int atk, hit, avo, crit;
    }
}