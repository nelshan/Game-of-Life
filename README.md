# Creating the clone of Conway's Game of Life in the Unity engine
 Conway’s Game of Life is a cellular automaton devised by John Conway in 1970. It consists of a grid where cells evolve based on predefined rules. This project analyzes the Unity-based implementation of Conway’s Game of Life, discussing object pooling, performance optimizations, and the impact of computational complexity. Conway’s Game of Life is a well-known example of a zero-player game, meaning that once initialized, the system evolves autonomously. It has been widely used in mathematical research, artificial life studies, and computer science education. 
 
Conway’s Game of Life operates on a two-dimensional grid where each cell has two states: alive or dead. The state of each cell in the next generation is determined by the number of alive neighbors. The evolution follows simple rules:
- Any live cell with fewer than two live neighbors dies (underpopulation).
- Any live cell with two or three live neighbors survives (Survival).
- Any live cell with more than three live neighbors dies (overpopulation).
- Any dead cell with exactly three live neighbors becomes alive (reproduction).

These rules produce emergent behavior, with patterns forming dynamically, including oscillators, still lifes, and moving structures like gliders.

# Computational Complexity
The Game of Life belongs to the class of **Turing-complete** systems, meaning it can simulate a universal computational model. However, the performance of naïve implementations degrades exponentially with grid size due to the O(n²) complexity of neighbor counting for each tick. To put it simply, cell input rises with grid size, which in turn causes the computation time to rise.  

**Note: if you are increasing or decreasing the Width and Height of the grid, adjust the size of the Cell prefab.**

_(**O(n²)** (Big **O** notation) describes an **algorithm that scales quadratically** with input size. This means that as the input size **n** grows, the execution time increases proportionally to **n²** .)_

# System Design and Implementation
1) **Grid Representation**  
In this Unity-based implementation, the grid is represented by a two-dimensional array **Cell[,] grid**, where each cell can either be alive (represented by an instance of a **Cell** object) or dead (represented by a **null** value). The grid's dimensions are parameterized, allowing for easy adjustment to different screen sizes or desired simulation resolutions. The grid is initialized to a specified screen size (1920 x 1080), its current width and height are 192 and 108 respectively and cells are updated at each generation based on the rules of the Game of Life. This allows for scalable simulations with varying grid sizes, making the game flexible for different display requirements.  
_(A **2D array (grid[x, y])** stores cell states. A separate **boolean array (nextGen[x, y])** holds computed states for the next generation, ensuring synchronous updates.)_
   ```ruby
   private Cell[,] grid;
   private bool[,] nextGen;
   ```

3) **Object Pooling**  
To optimize memory usage and prevent frequent memory allocations during gameplay, object pooling is utilized for the **Cell** objects. A pool of pre-instantiated **Cell** objects is created at the start, and each cell is activated or deactivated based on the grid's current state. This reduces the need for instantiating and destroying **Cell** objects during every update cycle.
The object pool is initialized with a predefined number of **Cell** objects, and a **Queue** structure is used to efficiently manage the reuse of these objects. When a cell is no longer needed, it is returned to the pool, and when a new cell is required, it is retrieved from the pool. This ensures constant-time operations for retrieving and returning objects, significantly reducing the overhead of object instantiation and garbage collection.  
_(Instead of destroying and recreating game objects every generation, **inactive cells are stored in a queue (cellPool) and reused**, reducing memory allocations and garbage collection overhead.)_
   ```ruby
   private Queue<Cell> cellPool = new Queue<Cell>();
   ```

5) **Simulation Update**
The simulation progresses by computing the next state of each cell before applying changes to the grid. This is accomplished using a **bool[,] nextGen** array, which stores the future state of each cell. The **GameOfLifeUpdateGame()** method iterates through the grid, calculating the number of live neighbors for each cell and determining its next state according to the rules of the Game of Life.
Once all cells have been evaluated, the new state is applied to the grid in a single update, ensuring that all cells transition simultaneously. This approach preserves the integrity of the game rules and prevents any inconsistencies during state transitions.

6) **Performance Optimization**
The primary goal of the implementation is to ensure efficient performance, especially for larger grid sizes. By using object pooling, the need for frequent calls to Unity’s **Instantiate()** and **Destroy()** methods is eliminated. These methods are typically costly in terms of memory allocation and garbage collection, and by reusing **Cell** objects, the implementation minimizes memory fragmentation and reduces performance overhead.
Additionally, the use of a **Queue** structure for managing the object pool ensures constant-time operations for retrieving and returning cells, further enhancing performance. This is critical in ensuring that the game runs smoothly, even for large grids or long-running simulations.

7) **Graphical Representation**
The graphical representation of the grid is achieved through Unity’s **SpriteRenderer**. Each **Cell** is represented by a **GameObject** that uses a sprite to visually indicate whether it is alive or dead. Alive cells are typically displayed in a color (e.g., white), while dead cells are displayed in a neutral color (e.g., black or transparent).
By leveraging Unity’s built-in rendering system, the game can efficiently display large grids while maintaining smooth animations and transitions between generations. The sprite system also allows for easy customization of the appearance of the cells, enabling the use of different visual styles or effects to represent the Game of Life simulation.

8) **Game Loop and Update Frequency**
The game loop is controlled using Unity’s **InvokeRepeating()** function, which updates the grid at fixed intervals, ensuring that each generation is processed at a consistent rate. This allows players to observe the simulation in real-time and experience the evolution of the grid over successive generations.
 
