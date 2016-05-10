using UnityEngine;
using System.Collections;

public class Node {

    public int x, y;

    public float hCost, gCost;

    public float fCost
    {
        get
        {
            return gCost+hCost;
        }
    }

    public Node ParentNode;
    public bool isWalkable = true;
   
}
