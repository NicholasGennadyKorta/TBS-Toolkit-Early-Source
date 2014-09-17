using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;


public class EditorPlayerDatabase : EditorWindow {

    List<Player> players = new List<Player>();
    List<string> playerNames = new List<string>();
    int currentPlayer = 0;
    bool intalized = false;
    Vector2 scrollPosition = Vector2.zero;

    [MenuItem("TRPG/Players")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(EditorPlayerDatabase));
    }

   

    void OnGUI()
    {
        if (!intalized)
        {
            Load();
            intalized = true;
        }

       
        title = "Players";
        playerNames[currentPlayer] = players[currentPlayer].id;

        Debug.Log(currentPlayer);

        EditorGUILayout.BeginVertical("BOX");
        currentPlayer = EditorGUILayout.Popup(currentPlayer, playerNames.ToArray());
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load")) Load();
        if (GUILayout.Button("Save")) Save();
        if (GUILayout.Button("+")) Add();
        if (GUILayout.Button("-")) Remove();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        players[currentPlayer].id = EditorGUILayout.TextField("ID", players[currentPlayer].id);
        players[currentPlayer].name = EditorGUILayout.TextField("Name", players[currentPlayer].name);
        players[currentPlayer].teamNumber = EditorGUILayout.IntField("Team Number", players[currentPlayer].teamNumber);
        players[currentPlayer].type = EditorGUILayout.IntField("Type", players[currentPlayer].type);
        players[currentPlayer].color = EditorGUILayout.ColorField("Color", players[currentPlayer].color);
        EditorGUILayout.EndScrollView();
    }

    void Save()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<Player>));
        using (var stream = File.OpenWrite("Assets/Resources/TRPG/Databases/PlayerDatabase.xml"))
        {
            serializer.Serialize(stream,players);
        }
    }

    void Load()
    {
        players.Clear();
        playerNames.Clear();

        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Player>));
            using (var stream = File.OpenRead("Assets/Resources/TRPG/Databases/PlayerDatabase.xml"))
            {
                var other = (List<Player>)(serializer.Deserialize(stream));
                players.AddRange(other);
            }
        }
        catch { }

        if (players.Count == 0)
        {
            Player player = new Player();
            player.id = "Player0";
            players.Add(player);
           
        }

        for (int i = 0; i < players.Count; ++i)
        {
            playerNames.Add(players[i].id);
        }

        currentPlayer = 0;
    }

    void Remove()
    {
        players.RemoveAt(currentPlayer);
        playerNames.RemoveAt(currentPlayer);

        if (players.Count == 0)
        {
            Debug.Log("POOP");
            Player player = new Player();
            player.id = "Player" + players.Count;
            players.Add(player);
            playerNames.Add("");
            currentPlayer = 0;
        }
    }

    void Add()
    {

        Player player = new Player();
        player.id = "Player" + players.Count;
        players.Add(player);
        currentPlayer = players.Count - 1;
        playerNames.Add("");
    }
}
