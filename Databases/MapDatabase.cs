using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapDatabase : MonoBehaviour {

    public List<Map> maps = new List<Map>();

    public Map GetByID(string id)
    {
        for (int i = 0; i < maps.Count; ++i)
        {
            if (maps[i].id == id)
                return maps[i];
        }
        return null;
    }
}
