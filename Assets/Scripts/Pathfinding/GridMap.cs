using UnityEngine;
using System.Collections;

public class GridMap {

    public LayerMask blockingLayer;

    // Max values for the x and y values in the position
    public int maxX, maxY, initX, initY;

    // How far are the nodes from each other. We can break it down into offsetx, offsety, etc. but we are going to assume all nodes are equidistant in all axes
    public float offset;

    public Node[ , ] grid;

    public Vector2 startNodePosition, targetNodePosition;

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

        maxX = tamX;
        maxY = tamY;

        grid = new Node[maxX, maxY];

        int x, y;

        for (x = initialX; x < maxX+initialX; x++)
        {
            for(y = initialY; y < maxY+initialY; y++)
            {
                Node node = new Node();

                node.x = x;
                node.y = y;

                grid[x, y] = node;
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

        for (x = 0; x < maxX; x++)
        {
            for (y = 0; y < maxY; y++)
            {
                // Check if there is a collider on the (x,y) point
                hit = Physics2D.OverlapPoint(new Vector2(x, y), this.blockingLayer);
                if (hit != null)
                    // If the collider is a trigger, it's walkable. The opposite is also true (not trigger->not walkable)
                    grid[x, y].isWalkable = hit.isTrigger;
                else
                    // If there is no collision, it is walkable
                    grid[x, y].isWalkable = true;

                // Update here the hcost if need be

            }
        }
    }

    public Node GetNode(int x, int y)
    {
        Node node = null;

        if(x >= 0 && x < maxX && y >= 0 && y < maxY )
        {
            node = grid[x, y];
        }

        return node;
    }


}
