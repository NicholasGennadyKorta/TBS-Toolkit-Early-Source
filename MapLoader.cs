using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System;

public class MapLoader : MonoBehaviour {

    public string mapFile = "Map00";
    private GameObject mapEviorment;

    public void Start()
    {
       LoadMap(mapFile);
    }

    public void LoadMap(string mapFile)
    {
        //Delete Map If already exists
        GameObject mapAlreadyExists = GameObject.Find("[map]");
        if (mapAlreadyExists != null)
            GameObject.DestroyImmediate(mapAlreadyExists);

       // Create our Parent
       GameObject rootMapObject = new GameObject();
       rootMapObject.name = "[map]";
       
       //Load In Our map Data
       var xmlFile = (TextAsset)Resources.Load("TRPG/Maps/" + mapFile + "/" + mapFile + "Data");
       XmlDocument xmlDocument = new XmlDocument();
       xmlDocument.LoadXml(xmlFile.text);
       XmlNodeList xmlNodeList;
       
       //Load Up Our map Eviorment
       xmlNodeList = xmlDocument.GetElementsByTagName("Map");
       string mapId = xmlNodeList[0].Attributes["id"].Value;
       int mapWidth = Int32.Parse(xmlNodeList[0].Attributes["width"].Value);
       int mapDepth = Int32.Parse(xmlNodeList[0].Attributes["depth"].Value);
       mapEviorment = (GameObject)GameObject.Instantiate(ObjectPool.objectDatabase.GetTerrain("Terrain0"));
       mapEviorment.name = "Enviorment";
       mapEviorment.transform.parent = rootMapObject.transform;
       
       //Load In A* Grid
       GameObject aStar = new GameObject();
       aStar.layer = 2;
       aStar.AddComponent<GridGraph>();
       aStar.transform.parent = rootMapObject.transform;
       aStar.name = "A*";
       aStar.transform.position += new Vector3(0,0,0);
       GridGraph gridGraph = aStar.GetComponent<GridGraph>();
       xmlNodeList = xmlDocument.GetElementsByTagName("AStar");
       GridGraph.instance.nodeSize = Int32.Parse(xmlNodeList[0].Attributes["nodeSize"].Value);
       GridGraph.instance.width = mapWidth;
       GridGraph.instance.depth = mapWidth;
       gridGraph.Intialize();
       
       
       //Load Our Units
       xmlNodeList = xmlDocument.GetElementsByTagName("Unit");
       GameObject units = new GameObject();
       units.transform.parent = rootMapObject.transform;
       units.name = "Units";
       for (int i = 0; i < xmlNodeList.Count; ++i)
       {
           int playerNumb = Int32.Parse(xmlNodeList[i].Attributes["playerNumber"].Value);
           string _class = xmlNodeList[i].Attributes["unit"].Value;
           int x = Int32.Parse(xmlNodeList[i].Attributes["x"].Value);
           int z = Int32.Parse(xmlNodeList[i].Attributes["z"].Value);

           GameObject unit = (GameObject)GameObject.Instantiate(ObjectPool.objectDatabase.GetUnit(_class));
           unit.layer = 2;
           unit.name = _class;
           unit.AddComponent<Seeker>();
           unit.AddComponent<Mover>();
          
           ObjectPool.units.Add(unit.GetComponent<Unit>());
           unit.transform.parent = units.transform;
           unit.name = unit.GetComponent<Unit>().name;
           unit.GetComponent<Unit>().playerNumb = playerNumb;
           unit.GetComponent<Unit>().GetComponent<Mover>().gridPosition = new Vector2(x, z);
           unit.transform.position = GridGraph.instance.pathNodes[(int)unit.GetComponent<Unit>().GetComponent<Mover>().gridPosition.x, (int)unit.GetComponent<Unit>().GetComponent<Mover>().gridPosition.y].position;

           XmlNodeList inventoryNode = xmlNodeList[i].ChildNodes;
           for (int j = 0; j < inventoryNode[0].ChildNodes.Count; ++j)
           {
               if (j == 8)
                   break;

               var itemType = inventoryNode[0].ChildNodes[j].Name;
               if (itemType == "Weapon")
                   unit.GetComponent<Unit>().inventory.items.Add(ObjectPool.objectDatabase.GetWeapon(inventoryNode[0].ChildNodes[j].Attributes["id"].Value));
               if (itemType == "Consumable")
                   unit.GetComponent<Unit>().inventory.items.Add(ObjectPool.objectDatabase.GetConsumable(inventoryNode[0].ChildNodes[j].Attributes["id"].Value));
           }
       }
       
       //Create our NoneTouch Cursor
       GameObject noneTouchCursor = GameObject.CreatePrimitive(PrimitiveType.Plane);
       noneTouchCursor.AddComponent<NoneTouchCursor>();
       ObjectPool.noneTouchCursor = noneTouchCursor.GetComponent<NoneTouchCursor>();
       noneTouchCursor.name = "Selector";
       noneTouchCursor.transform.parent = rootMapObject.transform;
       noneTouchCursor.layer = 2;
 
       
       //Create our Camera
       GameObject camera = new GameObject();
       camera.AddComponent<Camera>();
       camera.name = "MainCamera";
       camera.tag = "MainCamera";
       camera.AddComponent<GameCamera>();
       camera.AddComponent<AudioListener>();
       camera.AddComponent<AudioSource>();
       camera.GetComponent<GameCamera>().target = noneTouchCursor.transform;
       camera.transform.parent = rootMapObject.transform;
       camera.layer = 2;

       GameObject cameraGUI = new GameObject();
       cameraGUI.AddComponent<Camera>();
       cameraGUI.name = "GUICamera";
       cameraGUI.AddComponent<GameCamera>();
       cameraGUI.GetComponent<GameCamera>().target = noneTouchCursor.transform;
       cameraGUI.transform.parent = rootMapObject.transform;
       cameraGUI.layer = 2;
       cameraGUI.GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;
       cameraGUI.GetComponent<Camera>().cullingMask = (1 << LayerMask.NameToLayer("UI"));

       //Get Our Music
       xmlNodeList = xmlDocument.GetElementsByTagName("Music");
       var song = ObjectPool.audioDatabase.GetMusic(xmlNodeList[0].Attributes["normal"].Value);
       camera.GetComponent<AudioSource>().PlayOneShot(song);
       camera.GetComponent<AudioSource>().volume = 0.15f;

    }
}
