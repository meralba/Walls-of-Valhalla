﻿using UnityEngine;
using System.Collections;

public class GridMap {

    public LayerMask blockingLayer;

    // Max values for the x and y values in the position
    public int maxX, maxY, initX, initY;

    // How far are the nodes from each other. We can break it down into offsetx, offsety, etc. but we are going to assume all nodes are equidistant in all axes
    public float offset;

    public Node[ , ] grid;

    private Pathfinder pathFinder;

    /*
     * Params
     * tamX : size of the grid (horizontal)
     * tamY : size of the grid (vertical)
     * offset : separation between nodes, *see commentary above the definition
     * blockLayer : layer that contains the movement blocking objects
     * */
    public GridMap(int initialX, int initialY,int tamX, int tamY, float offset, LayerMask bL)
    {
        this.blockingLayer = bL;

        initX = initialX;
        initY = initialY;

        maxX = tamX+initX;
        maxY = tamY+initY;

        // tam = max - init

        // The correspondence between world value->map value is map=world - init

        grid = new Node[maxX-initX, maxY-initY];

        int x, y;

        for (x = initX; x < maxX; x++)
        {
            for(y = initY; y < maxY; y++)
            {
                Node node = new Node();

                node.x = x;
                node.y = y;

                grid[x-initX, y-initY] = node;
            }
        }

        // Once all the postions are initialized, update the isWalkable property of every node
        updateMap();
    }

    // Iterates through every node, casts a ray into it and detects if there is an object
    // that obstructs the movement, changing the node's property 
    public void updateMap()
    {
        int x, y;

        Collider2D hit = null;

        for (x = initX; x < maxX; x++)
        {
            for (y = initY; y < maxY; y++)
            {
                // Check if there is a collider on the (x,y) point
                hit = Physics2D.OverlapPoint(new Vector2(x, y), this.blockingLayer);

                if (hit != null && hit.GetComponent<Collider2D>().tag != "Player")
                    // If the collider is a trigger, it's walkable. The opposite is also true (not trigger->not walkable)
                    grid[x-initX, y-initY].isWalkable = hit.isTrigger;
                else
                    // If there is no collision, it is walkable
                    grid[x - initX, y - initY].isWalkable = true;

                // Update here the hcost if need be

            }
        }
    }


    // Accessors to the nodes.
    public Node GetNode(int x, int y)
    {
        Node node = null;

        // Transpose the position according to the initial x and y values provided while creating the class
        x -= initX;
        y -= initY;

        // Check the values are within the limits provided. Since we substracted the initial values, we can check the lower limit using 0
        if(x >= 0 && x < (maxX-initX) && y >= 0 && y < (maxY-initY) )
            node = grid[x, y];

        return node;
    }

    public Node GetNode(Transform t)
    {
        int x = Mathf.FloorToInt(t.position.x);
        int y = Mathf.FloorToInt(t.position.y);

        return GetNode(x, y);
    }

    public Node GetNode(GameObject go)
    {
        return GetNode(go.transform);
    }
}
