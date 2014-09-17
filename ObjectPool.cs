using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public static List<Unit> units = new List<Unit>();
    public static NoneTouchCursor noneTouchCursor;
    public static PlayerDatabase playersDatabase = GameObject.Find("Database").GetComponent<PlayerDatabase>();
    public static MapDatabase mapDatabase = GameObject.Find("Database").GetComponent<MapDatabase>();
    public static ObjectDatabase objectDatabase = GameObject.Find("Database").GetComponent<ObjectDatabase>();
    public static ClassDatabase classDatabase  = GameObject.Find("Database").GetComponent<ClassDatabase>();
    public static AudioDatabase audioDatabase = GameObject.Find("Database").GetComponent<AudioDatabase>();
    public static GUICommandMenu guiCommandMenu = GameObject.Find("CommandMenu").GetComponent<GUICommandMenu>();
    public static GUIItemMenu guiItemMenu = GameObject.Find("ItemMenu").GetComponent<GUIItemMenu>();
    public static GameLoop gameLoop;

   
}
