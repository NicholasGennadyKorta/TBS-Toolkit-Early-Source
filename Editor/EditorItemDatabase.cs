using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;
using System.Xml.Serialization;
using System.IO;

[CustomEditor(typeof(ObjectDatabase))]
public class EditorItemDatabase : Editor {

    ObjectDatabase myTarget;

    public override void OnInspectorGUI()
    {
        myTarget = (ObjectDatabase)target;
        DrawDefaultInspector();

        if (GUILayout.Button("Save"))
            Save();

       if (GUILayout.Button("Load"))
           Load();
    }

    void Save()
    {
        foreach (Weapon weapon in myTarget.weapons)
        {
            //weapon.StoreMeshData();
        }

        XmlSerializer serializer = new XmlSerializer(typeof(List<Weapon>));
        using (var stream = File.OpenWrite("Assets/Resources/TRPG/Databases/ItemDatabase.xml"))
        {
            serializer.Serialize(stream, myTarget.weapons);
        }
    }

    void Load()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<Weapon>));
        using (var stream = File.OpenRead("Assets/Resources/TRPG/Databases/ItemDatabase.xml"))
        {
            var other = (List<Weapon>)(serializer.Deserialize(stream));

            myTarget.weapons.Clear();
            myTarget.weapons.AddRange(other);

        }
    }

}
