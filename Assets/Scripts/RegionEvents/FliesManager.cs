using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FliesManager : MonoBehaviour
{
    [Header("Chances to spawn parameters")]
    [SerializeField][Range(0, 100)] private int spawnChance;

    [SerializeField] private int spawnTimeMinInterval;
    [SerializeField] private int spawnTimeMaxInterval;

    private float tempInterval;
    public void GenerateRandomInterval()
    {
        tempInterval = Random.Range(spawnTimeMinInterval, spawnTimeMaxInterval);
    }
    private void Start()
    {
        GenerateRandomInterval();
    }
    private void Update()
    {
        if(tempInterval <= 0)
        {
            int randChance = Random.Range(0, 100);
            if (randChance >= spawnChance)
            {
                LaunchFlies();
            }
            else
            {
                GenerateRandomInterval();
            }
        }
        else
        {
            tempInterval -= Time.deltaTime;
        }
    }
    public void LaunchFlies()
    {
        Cell[] cells = Grid.Instance.GetCells();
        if(cells.Count() > 0)
        {
            List<Cell> cellsWithPlants = new List<Cell>();
            for(int i = 0; i < cells.Length; i++)
            {
                if(cells[i].isOccupied)
                {
                    cellsWithPlants.Add(cells[i]);
                }
            }
            Plant plant = cellsWithPlants[Random.Range(0, cellsWithPlants.Count)].GetComponentInChildren<Plant>();
            plant.fliesAlert.SetActive(true);
        }
        else
        {
            Debug.Log("No plants to attack(flies)");
        }

    }
}