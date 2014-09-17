using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System;

public class ClassDatabase : MonoBehaviour
{

    public List<_Class> classes = new List<_Class>();

    public _Class GetByID(string id)
    {
        for (int i = 0; i < classes.Count; ++i)
        {
            if (classes[i].id == id)
                return classes[i];
        }
        return null;
    }
}

