using UnityEngine;

public class MazeDataGenerator
{
    public float placementThreshold;

    public MazeDataGenerator()
    {
        placementThreshold = 0.5f;
    }

    public int[,] FromDimensions(int rows, int cols, bool fillMaze = true)
    {
        int[,] maze = new int[rows, cols];

        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        for (int i = 0; i <= rMax; i++)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (i == 0 || j == 0 || i == rMax || j == cMax)
                {
                    maze[i, j] = 1;
                }
                else if (fillMaze)
                {
                    if (i % 2 == 0 && j % 2 == 0)
                    {
                        if (Random.value > placementThreshold)
                        {
                            maze[i, j] = 1;

                            int a = Random.value < 0.5f ? 0 : (Random.value < 0.5f ? -1 : 1);
                            int b = a != 0 ? 0 : (Random.value < 0.5f ? -1 : 1);
                            maze[i + a, j + b] = 1;
                        }
                    }
                }
                else
                {
                    maze[i, j] = 0;
                }
            }
        }

        maze = NoUnreachableCells(maze);

        return maze;
    }

    /*
        So many edge cases....I think these are in the right order,
        the orientation may be wrong though.
            1. Top left corner
            2. Top right corner
            3. Bottom right corner
            4. Bottom left corner
            5. Top row (excluding corners)
            6. Bottom row (excluding corners)
            7. Left side (excluding corners)
            8. Right side (excluding corners)
            9. Anything in the middle of the maze
    */
    public int[,] NoUnreachableCells(int[,] maze)
    {
        // Don't check the outermost edges of the maze
        // so start at 1 and don't include the upper bound
        for (int i = 1; i < maze.GetUpperBound(0); i++)
        {
            for (int j = 1; j < maze.GetUpperBound(1); j++)
            {
                if (maze[i, j] == 0)
                {
                    if (maze[i - 1, j] == 1 && maze[i + 1, j] == 1 && maze[i, j - 1] == 1 && maze[i, j + 1] == 1)
                    {
                        int randomIndex;

                        if (i == 1 && j == 1)
                        {
                            randomIndex = Random.Range(0, 2);
                            switch(randomIndex)
                            {
                                case 0:
                                    maze[i + 1, j] = 0;
                                    break;
                                case 1:
                                    maze[i, j + 1] = 0;
                                    break;
                            }
                        }
                        else if (i == 1 && j == maze.GetUpperBound(1) - 1)
                        {
                            randomIndex = Random.Range(0, 2);
                            switch(randomIndex)
                            {
                                case 0:
                                    maze[i + 1, j] = 0;
                                    break;
                                case 1:
                                    maze[i, j - 1] = 0;
                                    break;
                            }
                        }
                        else if (i == maze.GetUpperBound(0) - 1 && j == maze.GetUpperBound(1) - 1)
                        {
                            randomIndex = Random.Range(0, 2);
                            switch(randomIndex)
                            {
                                case 0:
                                    maze[i - 1, j] = 0;
                                    break;
                                case 1:
                                    maze[i, j - 1] = 0;
                                    break;
                            }
                        }
                        else if (i == maze.GetUpperBound(0) - 1 && j == 1)
                        {
                            randomIndex = Random.Range(0, 2);
                            switch(randomIndex)
                            {
                                case 0:
                                    maze[i - 1, j] = 0;
                                    break;
                                case 1:
                                    maze[i, j + 1] = 0;
                                    break;
                            }
                        }
                        else if (i == 1)
                        {
                            randomIndex = Random.Range(0, 3);
                            switch(randomIndex)
                            {
                                case 0:
                                    maze[i + 1, j] = 0;
                                    break;
                                case 1:
                                    maze[i, j - 1] = 0;
                                    break;
                                case 2:
                                    maze[i, j + 1] = 0;
                                    break;
                            }
                        }
                        else if (i == maze.GetUpperBound(0) - 1)
                        {
                            randomIndex = Random.Range(0, 3);
                            switch(randomIndex)
                            {
                                case 0:
                                    maze[i - 1, j] = 0;
                                    break;
                                case 1:
                                    maze[i, j - 1] = 0;
                                    break;
                                case 2:
                                    maze[i, j + 1] = 0;
                                    break;
                            }
                        }    
                        else if (j == 1)
                        {
                            randomIndex = Random.Range(0, 3);
                            switch(randomIndex)
                            {
                                case 0:
                                    maze[i, j + 1] = 0;
                                    break;
                                case 1:
                                    maze[i - 1, j] = 0;
                                    break;
                                case 2:
                                    maze[i + 1, j] = 0;
                                    break;
                            }
                        }
                        else if (j == maze.GetUpperBound(1) - 1)
                        {
                            randomIndex = Random.Range(0, 3);
                            switch(randomIndex)
                            {
                                case 0:
                                    maze[i, j - 1] = 0;
                                    break;
                                case 1:
                                    maze[i - 1, j] = 0;
                                    break;
                                case 2:
                                    maze[i + 1, j] = 0;
                                    break;
                            }
                        }
                        else
                        {
                            randomIndex = Random.Range(0, 4);
                            switch(randomIndex)
                            {
                                case 0:
                                    maze[i - 1, j] = 0;
                                    break;
                                case 1:
                                    maze[i + 1, j] = 0;
                                    break;
                                case 2:
                                    maze[i, j - 1] = 0;
                                    break;
                                case 3:
                                    maze[i, j + 1] = 0;
                                    break;
                            }
                        }
                    }
                }
            }
        }

        return maze;
    }
}
