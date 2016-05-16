using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// ShowExploration 3
// Use this to show the explored nodes in A*
public class Pathfinder /* : MonoBehaviour */ {

    public GridMap gridMap;

    private Node startingNode, targetNode;
    private List<Node> foundPath;

    public GameObject marker;

    public Pathfinder()
    {
        startingNode = null;
        targetNode = null;
        gridMap = null;
        foundPath = null;
    }

    public Pathfinder(GridMap sourceGrid)
    {
        startingNode = null;
        targetNode = null;
        foundPath = null;

        gridMap = sourceGrid;
        UpdateGrid();
    }

    public List<Node> FindPath(Node start, Node target)
    {
        // Update the members with the given parameters
        targetNode = target;
        startingNode = start;

        // Current found path
        foundPath = new List<Node>();

        // Open/Closed set
        List<Node> openSet = new List<Node>();
        //We need to make value existence checks, a hashed set is O(1) for those operations
        HashSet<Node> closedSet = new HashSet<Node>();

        Node currentNode = null;

        openSet.Add(startingNode);


        // The efficiency of this section can be improved if need be. Substitute the closed list with a stack
        while(openSet.Count > 0)
        {
            currentNode = openSet[0];

            // For each node in the open set
            for(int i=0; i<openSet.Count; i++)
            {
                // If the cost of an open node is less than the current node's, explore that node
                if(openSet[i].fCost < currentNode.fCost)
                {
                    if(!currentNode.Equals(openSet[i]))
                    {
                        currentNode = openSet[i];
                    }
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // ShowExploration 4
            // Use this to show the explored nodes in A*
            // Instantiate(marker, new Vector3(currentNode.x, currentNode.y, 0f), Quaternion.identity);

            // Another possible improvement point
            if (currentNode.Equals(targetNode))
            {
                foundPath = RetracePath(startingNode, currentNode);
                // Maybe a bool for the while to avoid using break?
                break;
            }


           
            foreach(Node neighbour in GetNeighbours(currentNode))
            {
                if (!closedSet.Contains(neighbour))
                {
                    // Uppdate the gCost calling the GetDistance static method from gridmap
                    float newGCost = currentNode.gCost + Node.GetDistance(currentNode, neighbour);

                    // If the new cost is lower or the open set doesnt have this neighbour (unexpanded node)
                    if ((newGCost < neighbour.gCost) || !openSet.Contains(neighbour))
                    {
                        // Calculate new values for the g and h costs
                        neighbour.gCost = newGCost;
                        neighbour.hCost = Node.GetHCost(neighbour, target);
                        // Set the current node as its parent
                        neighbour.parentNode = currentNode;
                        // Add the neighbour to the open set to explore if it hasn't already
                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }

                }                    
            }
        }
        
        if (foundPath.Count == 0)
            foundPath = null;

        return foundPath;
    }


    // Takes the start and end nodes, and retraces the path node-parent from the start to the end (must be called after the algorithm has finished, so that the parents are optimal)
    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();

        Node currentNode = endNode;


        // Iterate through the parents chain (which at this point are the best parents for each node, since the A* must be finished by the call) from the end node to the start node

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parentNode;
        }

        // Reverse the path's order (we want it from start to end)
        path.Reverse();

        return path;
    }

    // Gets the neighbours of a node. Our code is simpler, since we don't have to search in a third dimension.
    private List<Node> GetNeighbours(Node n)
    {
        List<Node> neighboursList = new List<Node>();

        int x, y;

        // Auxiliary node to hold the position of the to-be neighbour.
        Node auxNode;

        // We only check the directly adjacent nodes, not the diagonals

        // First check, horizontal axis
        y = 0;
        for(x = -1; x < 2; x += 2)
        {
            auxNode = new Node();

            auxNode.x = n.x + x;
            auxNode.y = n.y + y;

            Node searchNeighbour = GetNeighbourNode(auxNode, n);

            if (searchNeighbour != null)
                neighboursList.Add(searchNeighbour);
        }

        // Second check, vertical axis
        x = 0;
        for (y = -1; y < 2; y += 2)
        {
            // Auxiliary node to hold the position of the to-be neighbour.
            auxNode = new Node();

            auxNode.x = n.x + x;
            auxNode.y = n.y + y;

            Node searchNeighbour = GetNeighbourNode(auxNode, n);

            if (searchNeighbour != null)
                neighboursList.Add(searchNeighbour);

        }

        // Return the list of found neighbours
        return neighboursList;
    }

    // Returns a neighbour node to the n node in the position specified by neighbour
    private Node GetNeighbourNode(Node neighbour, Node n)
    {
        Node foundNode = null;

        Node possibleNeighbour = gridMap.GetNode(neighbour.x, neighbour.y);

        if(possibleNeighbour != null)
            if (possibleNeighbour.isWalkable)
                foundNode = possibleNeighbour;


        return foundNode;
    }

    
    public void UpdateGrid()
    {
        gridMap.updateMap();
    }


}
