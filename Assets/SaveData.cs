using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public Vector3 playerPosition = Vector3.zero;
    public string currentMapId = "";

    public List<string> completedArenas = new List<string>();
}