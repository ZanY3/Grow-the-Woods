using UnityEngine;

public class Grid : MonoBehaviour
{
    public static Grid Instance;

    [Header("Grid size")]
    [SerializeField] private int gridWidth = 5;
    [SerializeField] private int gridHeight = 5;

    [SerializeField] private Cell[] cells;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public bool IsExistEmptyCell()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            if (!cells[i].isOccupied && cells[i].isBuyied)
                return true;
        }

        return false;
    }

    public bool HasAdjacentPlants(Cell cell)
    {
        int index = System.Array.IndexOf(cells, cell);

        int x = index % gridWidth;
        int y = index / gridWidth;

        // LEFT
        if (x > 0)
        {
            if (cells[index - 1].isOccupied)
                return true;
        }

        // RIGHT
        if (x < gridWidth - 1)
        {
            if (cells[index + 1].isOccupied)
                return true;
        }

        // UP
        if (y > 0)
        {
            if (cells[index - gridWidth].isOccupied)
                return true;
        }

        // DOWN
        if (y < gridHeight - 1)
        {
            if (cells[index + gridWidth].isOccupied)
                return true;
        }

        return false;
    }

    public int CountAdjacentPlants(Cell cell)
    {
        int count = 0;

        int index = System.Array.IndexOf(cells, cell);

        int x = index % gridWidth;
        int y = index / gridWidth;

        // LEFT
        if (x > 0 && cells[index - 1].isOccupied)
            count++;

        // RIGHT
        if (x < gridWidth - 1 && cells[index + 1].isOccupied)
            count++;

        // UP
        if (y > 0 && cells[index - gridWidth].isOccupied)
            count++;

        // DOWN
        if (y < gridHeight - 1 && cells[index + gridWidth].isOccupied)
            count++;

        return count;
    }
}
