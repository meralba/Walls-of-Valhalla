﻿using UnityEngine;
using System;
using System.Collections.Generic;       //Allows us to use Lists.
using Random = UnityEngine.Random;      //Tells Random to use the Unity Engine random number generator.

    public class BoardManager : MonoBehaviour
    {
        // Using Serializable allows us to embed a class with sub properties in the inspector.
        [Serializable]
        public class Count
        {
            public int minimum;             //Minimum value for our Count class.
            public int maximum;             //Maximum value for our Count class.


            //Assignment constructor.
            public Count(int min, int max)
            {
                minimum = min;
                maximum = max;
            }
        }


        public int columns = 20;
        public int rows = 20;
        public Count wallCount = new Count(5, 9);
        public Count foodCount = new Count(1, 5);
        public GameObject player;
        public GameObject exit;
        public GameObject[] floorTiles;
        public GameObject[] wallTiles;
        public GameObject[] healthTiles;
        public GameObject[] enemyTiles;
        public GameObject[] outerWallTiles;

        private GameObject board;
        private List<Vector3> gridPositions = new List<Vector3>();


        void InitialiseList()
        {
            gridPositions.Clear();

            //Loop through x axis (columns).
            for (int x = 1; x < columns - 1; x++)
            {
                //Within each column, loop through y axis (rows).
                for (int y = 1; y < rows - 1; y++)
                {
                    //At each index add a new Vector3 to our list with the x and y coordinates of that position.
                    gridPositions.Add(new Vector3(x, y, 0f));
                }
            }
        }


        void boardSetup()
        {
            if (board != null)
                Destroy(board);

                
            board = new GameObject("Board");

            int x, y;
            int floorLowerLimit, floorUpperLimit;
            int extWLowerLimit, extUpperLimit;


        /*
        int width, slice;

        width = floorTiles.Length / GameManager.instance.stages;
        slice = GameManager.instance.level / GameManager.instance.levelsPerStage;

        lowerLimit = width * slice;
        upperLimit = lowerLimit + width;
        */

        floorLowerLimit = (floorTiles.Length / GameManager.instance.stages) * ((GameManager.instance.level / GameManager.instance.levelsPerStage) % GameManager.instance.levelsPerStage);
        floorUpperLimit = floorLowerLimit + (floorTiles.Length / GameManager.instance.stages);

        extWLowerLimit = (outerWallTiles.Length / GameManager.instance.stages) * ((GameManager.instance.level / GameManager.instance.levelsPerStage) % GameManager.instance.levelsPerStage);
        extUpperLimit = extWLowerLimit + (outerWallTiles.Length / GameManager.instance.stages);


        for (x = -1; x < columns + 1; x++)
            {
                for (y = -1; y < rows + 1; y++)
                {
                    // Here, we instantiate a tile, in the x,y,0 coordinates, and set its parent to be the boardInstance
                    if (x == -1 || x == columns || y == -1 || y == rows)
                        (Instantiate(outerWallTiles[Random.Range(extWLowerLimit, extUpperLimit)], new Vector3(x, y, 0f), Quaternion.identity) as GameObject).transform.SetParent(board.transform);
                    else
                        (Instantiate(floorTiles[Random.Range(floorLowerLimit, floorUpperLimit)], new Vector3(x, y, 0f), Quaternion.identity) as GameObject).transform.SetParent(board.transform);
                }
            }
        }


        //RandomPosition returns a random position from our list gridPositions.
        Vector3 RandomPosition()
        {
            //Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
            int randomIndex = Random.Range(0, gridPositions.Count);

            //Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
            Vector3 randomPosition = gridPositions[randomIndex];

            //Remove the entry at randomIndex from the list so that it can't be re-used.
            gridPositions.RemoveAt(randomIndex);

            //Return the randomly selected Vector3 position.
            return randomPosition;
        }


        //LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
        void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
        {
            //Choose a random number of objects to instantiate within the minimum and maximum limits
            int objectCount = Random.Range(minimum, maximum + 1);

            int lowerLimit, upperLimit;


            //Instantiate objects until the randomly chosen limit objectCount is reached
            for (int i = 0; i < objectCount; i++)
            {
                //Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
                Vector3 randomPosition = RandomPosition();

                //Choose a random tile from tileArray and assign it to tileChoice

                lowerLimit = (tileArray.Length / GameManager.instance.stages) * ((GameManager.instance.level / GameManager.instance.levelsPerStage) % GameManager.instance.levelsPerStage);
                upperLimit = lowerLimit + (tileArray.Length / GameManager.instance.stages);

                GameObject tileChoice = tileArray[Random.Range(lowerLimit, upperLimit)];

                //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
                (Instantiate(tileChoice, randomPosition, Quaternion.identity) as GameObject).transform.SetParent(board.transform);
            }
        }


        //SetupScene initializes our level and calls the previous functions to lay out the game board
        public void SetupScene(int level)
        {
            //Creates the outer walls and floor.
            this.boardSetup();

            //Reset our list of gridpositions.
            InitialiseList();

            //Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
            LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);

            //Instantiate a random number of food tiles based on minimum and maximum, at randomized positions.
            LayoutObjectAtRandom(healthTiles, foodCount.minimum, foodCount.maximum);

            //Determine number of enemies based on current level number, based on a logarithmic progression
            int enemyCount = level / 2 + 1 ;

            //Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
            LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);

            //Instantiate the exit tile in the upper right hand corner of our game board
            (Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity) as GameObject).transform.SetParent(board.transform);

            //Instantiate the Player in the 0,0 position of the board (lower left corner)
            (Instantiate(player, new Vector3(0, 0, 0f), Quaternion.identity) as GameObject).transform.SetParent(board.transform);
        }
    }
