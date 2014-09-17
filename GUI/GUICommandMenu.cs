using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUICommandMenu : MonoBehaviour
{
    Canvas canvas;
    GameObject background;
    public GameObject[] items = new GameObject[8];
    GameObject handCursor;
    float handCusorPositionX;
    float originalHandCursorPositionX;
    float handCursorSpeed = 40;

	// Use this for initialization
	void Start () {

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
        }

        handCusorPositionX = handCursor.GetComponent<RectTransform>().position.x;
        originalHandCursorPositionX = handCusorPositionX;
    }
	
	// Update is called once per frame
	void Update () {
       
        if (canvas != null)
        {
            if (GameLoop.phase == "choose")
            {
                canvas.enabled = true;
                handCursor.GetComponent<RectTransform>().position = new Vector3(handCusorPositionX, items[GameLoop.currentCommandMenuItem].GetComponent<RectTransform>().position.y, 0);

                handCusorPositionX -= Time.deltaTime * handCursorSpeed;
                if (handCusorPositionX <= originalHandCursorPositionX - 5)
                    handCursorSpeed = -40;
                else if (handCusorPositionX >= originalHandCursorPositionX + 10)
                    handCursorSpeed =40;

                if (!GameLoop.unitSelected)
                {
                    for (int i = 0; i < items.Length; ++i)
                    {
                        if (i < 4)
                            items[i].SetActive(true);
                        else
                            items[i].SetActive(false);
                    }

                    GameLoop.commandMenuItemCount = 4;

                    items[0].GetComponentsInChildren<Text>()[0].text = "Units";
                    items[1].GetComponentsInChildren<Text>()[0].text = "Options";
                    items[2].GetComponentsInChildren<Text>()[0].text = "Save";
                    items[3].GetComponentsInChildren<Text>()[0].text = "End";

                    background.GetComponent<RectTransform>().localScale = new Vector3(background.GetComponent<RectTransform>().localScale.x, 0.5f, background.GetComponent<RectTransform>().localScale.z);
                }
                else
                {
                    for (int i = 0; i < items.Length; ++i)
                    {
                        if (i < 5)
                            items[i].SetActive(true);
                        else
                            items[i].SetActive(false);
                    }

                    GameLoop.commandMenuItemCount = 5;

                    items[0].GetComponentsInChildren<Text>()[0].text = "Attack";
                    items[1].GetComponentsInChildren<Text>()[0].text = "Skills";
                    items[2].GetComponentsInChildren<Text>()[0].text = "Items";
                    items[3].GetComponentsInChildren<Text>()[0].text = "Trade";
                    items[4].GetComponentsInChildren<Text>()[0].text = "Wait";

                    background.GetComponent<RectTransform>().localScale = new Vector3(background.GetComponent<RectTransform>().localScale.x, 0.625f, background.GetComponent<RectTransform>().localScale.z);
                }
            }
            else
            {
                canvas.enabled = false;
            }

        }
	}

    public string GetCurrentMenuItem()
    {
        return items[GameLoop.currentCommandMenuItem].GetComponentsInChildren<Text>()[0].text;
    }
}
