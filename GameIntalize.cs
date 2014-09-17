using UnityEngine;
using System.Collections;

public class GameIntalize : MonoBehaviour {

    public GameObject database;
	void Awake () {
        GameObject newDatabase = Instantiate(database) as GameObject;
        newDatabase.name = "Database";
        DontDestroyOnLoad(newDatabase);
        DontDestroyOnLoad(GameObject.Find("GameGUI"));
        Application.LoadLevel(1);
	}
}
