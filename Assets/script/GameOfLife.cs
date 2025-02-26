using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameOfLife : MonoBehaviour
{
    [Header("Press R to restart the Game of Life")]
    public int width = 192;  
    public int height = 108; 
    public GameObject cellPrefab;
    public int poolSize = 5000;  // Object pool size

    private Cell[,] grid;
    private bool[,] nextGen;
    private float cellSize = 0.1f;
    private float TimeInterval = 0.1f;

    private Queue<Cell> cellPool = new Queue<Cell>(); // Object pool
    private int generationCount = 0; // Total number of generations
    private int currentPopulation = 0; // Total live cells in current generation

    public Text infoText;

    void Start()
    {
        grid = new Cell[width, height];
        nextGen = new bool[width, height];

        InitializeCellPool();
        InitializeGrid();
        InvokeRepeating(nameof(GameOfLifeUpdateGame), TimeInterval, TimeInterval);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }

        // Display current generation and population count
        if (infoText != null)
        {
            infoText.text = "Generation: " + generationCount + "\nPopulation: " + currentPopulation;
        }
    }

    void InitializeCellPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject newCellObj = Instantiate(cellPrefab);
            newCellObj.SetActive(false);  // Initially deactivated
            Cell newCell = newCellObj.GetComponent<Cell>();
            cellPool.Enqueue(newCell);
        }
    }

    Cell GetCellFromPool(int x, int y)
    {
        if (cellPool.Count > 0)
        {
            Cell cell = cellPool.Dequeue();
            Vector3 position = new Vector3((x * cellSize) - (width * cellSize) / 2.01f, 
                                           (y * cellSize) - (height * cellSize) / 2.02f, 0);
            cell.transform.position = position;
            cell.gameObject.SetActive(true);
            return cell;
        }
        else
        {
            // If pool is exhausted, it create a new one (should not happen if pool size is sufficient)
            Debug.LogWarning("Pool exhausted, instantiating a new cell.");
            GameObject newCellObj = Instantiate(cellPrefab);
            Cell newCell = newCellObj.GetComponent<Cell>();
            return newCell;
        }
    }

    void ReturnCellToPool(Cell cell)
    {
        cell.gameObject.SetActive(false);  // Deactivate the cell
        cellPool.Enqueue(cell);  // Return the cell to the pool
    }

    void InitializeGrid()
    {
        float offsetX = (width * cellSize) / 2.01f;
        float offsetY = (height * cellSize) / 2.02f;
        currentPopulation = 0; // Reset population count

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool isAlive = Random.value > 0.7f; // 30% chance of being alive
                grid[x, y] = isAlive ? GetCellFromPool(x, y) : null;
                if (grid[x, y] != null)
                {
                    grid[x, y].SetState(true);
                    currentPopulation++;
                }
            }
        }

        Camera.main.transform.position = new Vector3(0, 0, -10);
        Camera.main.orthographicSize = (height * cellSize) / 2f;
    }

    void GameOfLifeUpdateGame()
    {
        generationCount++;  // Increment generation counter
        int newPopulation = 0;  // Count new alive cells

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbors = CountNeighbors(x, y);
                bool isAlive = grid[x, y] != null; // Check if the cell exists

                // Apply Conway's Game of Life rules
                nextGen[x, y] = isAlive ? (neighbors == 2 || neighbors == 3) : (neighbors == 3);

                if (nextGen[x, y]) newPopulation++; // Count new alive cells
            }
        }

        currentPopulation = newPopulation; // Update population count
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
                        grid[x, y] = GetCellFromPool(x, y);
                        grid[x, y].SetState(true);
                    }
                }
                else // Cell should be dead
                {
                    if (isAlive) // If cell was alive, destroy it
                    {
                        ReturnCellToPool(grid[x, y]);
                        grid[x, y] = null;
                    }
                }
            }
        }
    }

    void RestartGame()
    {
        // Cancel the current update loop
        CancelInvoke(nameof(GameOfLifeUpdateGame));

        // Return all active cells to the pool
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] != null)
                {
                    ReturnCellToPool(grid[x, y]);
                    grid[x, y] = null;
                }
            }
        }

        // Reset counters
        generationCount = 0;
        currentPopulation = 0;

        // Reinitialize the grid with a new random seed
        InitializeGrid();

        // Restart the simulation loop
        InvokeRepeating(nameof(GameOfLifeUpdateGame), TimeInterval, TimeInterval);
    }

    // Returns the current generation count
    public int GetGenerationCount()
    {
        return generationCount;
    }

    // Returns the current live cell population
    public int GetPopulationCount()
    {
        return currentPopulation;
    }
}