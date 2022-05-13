using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabyrinthGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    class Labyrinth
    {
        LabyrinthCell[,] labyrinth;

        Labyrinth(int height, int width, int cellSize)
        {
            labyrinth = new LabyrinthCell[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i == 0 && j == width - 1) // Top right angle cells
                    {
                        labyrinth[i, j] = new LabyrinthCell(cellSize, true, true, false, false);
                    }
                    else if (i == height - 1 && j == width - 1) // Bottom right angle cells
                    {
                        labyrinth[i, j] = new LabyrinthCell(cellSize, false, true, true, false);
                    }
                    else if (i == height - 1 && j == 0) // Bottom left angle cells
                    {
                        labyrinth[i, j] = new LabyrinthCell(cellSize, false, false, true, true);
                    }
                    else if (i == 0 && j == 0) // Top left angle cells
                    {
                        labyrinth[i, j] = new LabyrinthCell(cellSize, true, false, false, true);
                    }
                    else if (i == 0) // Top border cells
                    {
                        labyrinth[i, j] = new LabyrinthCell(cellSize, true, false, false, false);
                    }
                    else if (j == width - 1) // Right border cells
                    {
                        labyrinth[i, j] = new LabyrinthCell(cellSize, false, true, false, false);
                    }
                    else if (i == height - 1) // Bottom border cells
                    {
                        labyrinth[i, j] = new LabyrinthCell(cellSize, false, false, true, false);
                    }
                    else if (j == 0) // Left border cells
                    {
                        labyrinth[i, j] = new LabyrinthCell(cellSize, false, false, false, true);
                    }
                    else // Center cells
                    {
                        labyrinth[i, j] = new LabyrinthCell(cellSize, false, false, false, false);
                    }
                }
            }
        }
    }

    public class LabyrinthCell
    {
        Wall[,] walls;

        public LabyrinthCell(int size, bool isTopLocked, bool isRightLocked, bool isBottomLocked, bool isLeftLocked)
        {
            walls = new Wall[4, size];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    walls[i, j] = new Wall(WallType.closed);
                }
            }
            if (isTopLocked)
            {
                for (int i = 0; i < size; i++)
                {
                    walls[0, i].type = WallType.locked;
                }
            }
            if (isRightLocked)
            {
                for (int i = 0; i < size; i++)
                {
                    walls[1, i].type = WallType.locked;
                }
            }
            if (isBottomLocked)
            {
                for (int i = 0; i < size; i++)
                {
                    walls[2, i].type = WallType.locked;
                }
            }
            if (isLeftLocked)
            {
                for (int i = 0; i < size; i++)
                {
                    walls[3, i].type = WallType.locked;
                }
            }
        }
    }

    public class Wall
    {
        public WallType type;

        public Wall(WallType t)
        {
            type = t;
        }
    }

    public enum WallType
    {
        locked,
        open,
        closed
    }


    // TODO: to create ready labyrith firstly, than to think about random generation 
    // TODO: bag - player can tunnel through the walls 
}
