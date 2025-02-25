using UnityEngine;

public class GameOfLife : MonoBehaviour
{
    public int width = 192;  
    public int height = 108; 
    public GameObject cellPrefab;

    private Cell[,] grid;
    private bool[,] nextGen;
    private float cellSize = 0.1f;  
    private float TimeInterval = 0.1f; 

    void Start()
    {
        grid = new Cell[width, height];
        nextGen = new bool[width, height];

        InitializeGrid();
        InvokeRepeating(nameof(GameOfLifeUpdateGame), TimeInterval, TimeInterval);
    }

    

    void InitializeGrid()
    {
        float offsetX = (width * cellSize) / 2.01f;
        float offsetY = (height * cellSize) / 2.02f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool isAlive = Random.value > 0.7f; // 30% chance of being alive at start
                grid[x, y] = isAlive ? CreateCell(x, y) : null;
            }
        }

        Camera.main.transform.position = new Vector3(0, 0, -10);
        Camera.main.orthographicSize = (height * cellSize) / 2f;
    }

    Cell CreateCell(int x, int y)
    {
        Vector3 position = new Vector3((x * cellSize) - (width * cellSize) / 2.01f, 
                                       (y * cellSize) - (height * cellSize) / 2.02f, 
                                       0);
        GameObject newCell = Instantiate(cellPrefab, position, Quaternion.identity, transform);
        newCell.transform.localScale = new Vector3(cellSize, cellSize, 1);
        return newCell.GetComponent<Cell>();
    }

    void GameOfLifeUpdateGame()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbors = CountNeighbors(x, y);
                bool isAlive = grid[x, y] != null; // Check if the cell exists

                nextGen[x, y] = isAlive ? (neighbors == 2 || neighbors == 3) : (neighbors == 3);
            }
        }

        ApplyNextGeneration();
    }

    int CountNeighbors(int x, int y)
    {
        int count = 0;
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                int nx = x + dx, ny = y + dy;
                if (nx >= 0 && ny >= 0 && nx < width && ny < height && grid[nx, ny] != null)
                {
                    count++;
                }
            }
        }
        return count;
    }

    void ApplyNextGeneration()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool isAlive = grid[x, y] != null;

                if (nextGen[x, y]) // Cell should be alive
                {
                    if (!isAlive) // If cell was dead, create it
                    {
                        grid[x, y] = CreateCell(x, y);
                    }
                }
                else // Cell should be dead
                {
                    if (isAlive) // If cell was alive, destroy it
                    {
                        Destroy(grid[x, y].gameObject);
                        grid[x, y] = null;
                    }
                }
            }
        }
    }

}

    // void Update()

    // {

    //     if (Input.GetKeyDown(KeyCode.R))

    //     {

    //         RestartGame();

    //     }

    // }
        // void RestartGame()

    // {

    //     for (int x = 0; x < width; x++)

    //     {

    //         for (int y = 0; y < height; y++)

    //         {

    //             grid[x, y].SetState(Random.value > 0.7f);  // Reset instead of recreating

    //         }

    //     }

    // }