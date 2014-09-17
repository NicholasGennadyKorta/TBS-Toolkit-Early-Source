using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIUnitHealthBar : MonoBehaviour {

    Quaternion originalRotation;
    Vector3 originalPosition;
    Unit unit;
    Image background, health;

	// Use this for initialization
	void Start () {
        unit = transform.parent.GetComponent<Unit>();
        background = transform.GetComponentsInChildren<Image>()[0];
        health = transform.GetComponentsInChildren<Image>()[1];
        health.color = ObjectPool.playersDatabase.players[unit.playerNumb].color;
        originalPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        transform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x,0, 0);
        health.fillAmount = unit.stats.currentHp / unit.stats.hp;
	}
}
