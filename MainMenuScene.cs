using UnityEngine;
using System.Collections;

public class MainMenuScene : MonoBehaviour {

    GUIStyle guiStyle = new GUIStyle();
    float labelAlpha = 0;
    float labelState;
    string content = "";

	// Use this for initialization
	void Start () {
        guiStyle.normal.textColor = Color.white;
        guiStyle.fontSize = 30;
	}
	
    void OnGUI()
    {
        if (labelState == 0)
        {
            labelAlpha += 0.35f * Time.deltaTime;
            if (labelAlpha > 1)
                labelState = 1;
        }

        if (labelState == 1)
        {
            labelAlpha -= 0.35f * Time.deltaTime;
            if (labelAlpha < 0)
                labelState = 0;
        }

        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, labelAlpha);
        GUI.Label(new Rect(Screen.width / 2 - 80, Screen.height * 0.7f, 500, 500), content,guiStyle);
    }
}
