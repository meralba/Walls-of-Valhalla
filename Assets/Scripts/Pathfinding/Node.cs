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

    public Node parentNode;
    public bool isWalkable = true;

    public static float GetDistance(Node n1, Node n2)
    {
        Vector2 difference = new Vector2(n2.x - n1.x, n2.y - n1.y);

        return difference.magnitude;
    }

    // Here we can change the heuristic function value of a node.
    // Let's start off with euclidean distance
    public static float GetHCost(Node n, Node target)
    {
        return GetDistance(n, target);
    }

}
