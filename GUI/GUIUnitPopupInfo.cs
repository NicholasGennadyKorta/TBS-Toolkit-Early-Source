using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIUnitPopupInfo : MonoBehaviour
{
    Canvas canvas;
    Image profile;
    Text player, name;

	void Start () {
       canvas = GetComponent<Canvas>();

        foreach (Transform t in transform)
        {
            if (t.name == "Profile")
                profile = t.GetComponent<Image>();
            if (t.name == "Player")
                player = t.GetComponent<Text>();
            if (t.name == "Name")
                name = t.GetComponent<Text>();
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (canvas != null)
        {
            if (GameLoop.unitHovered != null && GameLoop.phase != "moving" && GameLoop.phase != "choose" && GameLoop.phase != "items")
            {
                canvas.enabled = true;
                profile.sprite = GameLoop.unitHovered.gui.profile;
                player.text = ObjectPool.playersDatabase.players[GameLoop.unitHovered.playerNumb].name;
                player.color = ObjectPool.playersDatabase.players[GameLoop.unitHovered.playerNumb].color;
                name.text = GameLoop.unitHovered.name;
            }
            else
                canvas.enabled = false;
        }
	}
}
