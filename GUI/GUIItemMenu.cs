using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIItemMenu : MonoBehaviour
{
    Canvas canvas;
    public GameObject actionMenu;
    GameObject background;
    public GameObject[] items = new GameObject[8];
    GameObject handCursor;
    GameObject equipIcon;
    AudioClip handCursorMoveAudio = new AudioClip();
    public float handCusorPositionX,handCursorPositionXOffset;
    float originalHandCursorPositionX, originalHandCursorPositionY;
    float handCursorSpeed = 40;

    // Use this for initialization
    void Start()
    {

        canvas = GetComponent<Canvas>();

        foreach (Transform t in transform)
        {
            if (t.name == "Item0")
                items[0] = t.gameObject;
            if (t.name == "Item1")
                items[1] = t.gameObject;
            if (t.name == "Item2")
                items[2] = t.gameObject;
            if (t.name == "Item3")
                items[3] = t.gameObject;
            if (t.name == "Item4")
                items[4] = t.gameObject;
            if (t.name == "Item5")
                items[5] = t.gameObject;
            if (t.name == "Item6")
                items[6] = t.gameObject;
            if (t.name == "Item7")
                items[7] = t.gameObject;
            if (t.name == "HandCursor")
                handCursor = t.gameObject;
            if (t.name == "Background")
                background = t.gameObject;
            if (t.name == "ActionMenu")
                actionMenu = t.gameObject;
            if (t.name == "EquipIcon")
                equipIcon = t.gameObject;
        }

        handCusorPositionX = handCursor.GetComponent<RectTransform>().position.x;
        originalHandCursorPositionX = handCusorPositionX;
        handCursorMoveAudio = ObjectPool.audioDatabase.GetSoundEffect("HandCursorMove");
    }

    // Update is called once per frame
    void Update()
    {

        if (canvas != null)
        {
            if (GameLoop.phase == "items")
            {
                canvas.enabled = true;

                if (!GameLoop.chooseItemAction)
                    handCursor.GetComponent<RectTransform>().position = new Vector3(handCusorPositionX + handCursorPositionXOffset, items[GameLoop.currentItemMenuItem].GetComponent<RectTransform>().position.y, 0);
                else
                    handCursor.GetComponent<RectTransform>().position = new Vector3(handCusorPositionX + handCursorPositionXOffset, items[GameLoop.currentItemMenuActionItem].GetComponent<RectTransform>().position.y, 0);

                handCusorPositionX -= Time.deltaTime * handCursorSpeed;
                if (handCusorPositionX <= originalHandCursorPositionX + handCursorPositionXOffset - 5)
                    handCursorSpeed = -40;
                else if (handCusorPositionX >= originalHandCursorPositionX + handCursorPositionXOffset + 10)
                    handCursorSpeed = 40;


                actionMenu.SetActive(GameLoop.chooseItemAction);
                if (GameLoop.unitSelected.inventory.equipedWeapon != null)
                    equipIcon.SetActive(true);
                else
                    equipIcon.SetActive(false);

                int iter = 0;
                for (int i = 0; i < GameLoop.unitSelected.inventory.items.Count; ++i)
                {

                    Item item = GameLoop.unitSelected.inventory.items[i];
                    items[iter].SetActive(true);
                    items[iter].GetComponent<Image>().sprite = item.icon;
                    items[iter].GetComponentsInChildren<Text>()[0].text = item.name;
                    items[iter].GetComponentsInChildren<Text>()[1].text = item.currentDurability.ToString();
                    iter++;
                }

                for (int i = iter; i < 8; ++i)
                {
                    items[i].SetActive(false);
                }

                if (iter > 0)
                {
                    if (GameLoop.unitSelected.inventory.items[GameLoop.currentItemMenuItem].itemType == Item.ItemType.CONSUMABLE)
                    {
                        actionMenu.transform.FindChild("Item0").GetComponent<Text>().text = "Consume";
                        actionMenu.transform.FindChild("Item1").GetComponent<Text>().text = "Discard";
                    }

                    if (GameLoop.unitSelected.inventory.items[GameLoop.currentItemMenuItem].itemType == Item.ItemType.WEAPON)
                    {
                        if (GameLoop.unitHovered.inventory.equipedWeapon == GameLoop.unitSelected.inventory.items[GameLoop.currentItemMenuItem])
                            actionMenu.transform.FindChild("Item0").GetComponent<Text>().text = "Dequip";
                        else
                            actionMenu.transform.FindChild("Item0").GetComponent<Text>().text = "Equip";
                        actionMenu.transform.FindChild("Item1").GetComponent<Text>().text = "Discard";
                    }
                }

                GameLoop.itemMenuItemCount = iter;
            }
            else
            {
                canvas.enabled = false;
            }
        }
    }

    public string GetCurrentActionItem()
    {
        return actionMenu.transform.FindChild("Item" + GameLoop.currentItemMenuActionItem).GetComponent<Text>().text;
    }
}
