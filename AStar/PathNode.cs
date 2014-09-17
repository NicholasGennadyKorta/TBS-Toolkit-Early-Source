using UnityEngine;
using System.Collections;

public struct PathNode
{
    public bool isWalkable;
    public bool isAttackable;
    public int cost;
    public Vector3 position;
    public Vector3 rotation;
    public Vector2 gridNodeIndex;
    public GameObject cell;

    public static bool operator ==(PathNode one, PathNode two)
    {
        if (one.position == two.position)
            return true;
        return false;
    }

    public static bool operator !=(PathNode one, PathNode two)
    {
        if (one.position == two.position)
            return false;
        return true;
    }
}