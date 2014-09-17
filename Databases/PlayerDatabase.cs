using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System;
using System.Xml.Serialization;

public class PlayerDatabase : MonoBehaviour {
    public List<Player> players = new List<Player>();

    void Awake()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<Player>));
        using (var stream = File.OpenRead("Assets/Resources/TRPG/Databases/PlayerDatabase.xml"))
        {
            var other = (List<Player>)(serializer.Deserialize(stream));

           players.Clear();
           players.AddRange(other);

        }
    }
}
