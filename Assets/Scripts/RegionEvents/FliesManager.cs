using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FliesManager : MonoBehaviour
{
    [SerializeField] private AudioClip fliesLaunchSound;

    [Header("Chances to spawn parameters")]
    [SerializeField][Range(0, 100)] private int spawnChance;

    [SerializeField] private int spawnTimeMinInterval;
    [SerializeField] private int spawnTimeMaxInterval;

    [SerializeField] private int minPlantsToAttackCount;
    [SerializeField] private int maxPlantsToAttackCount;

    [HideInInspector] public bool canLaunchFlies = true;

    private float tempInterval;
    private int plantsToAttack;
    public void GenerateRandomInterval()
    {
        tempInterval = Random.Range(spawnTimeMinInterval, spawnTimeMaxInterval);
        plantsToAttack = Random.Range(minPlantsToAttackCount, maxPlantsToAttackCount);
    }
    private void Start()
    {
        GenerateRandomInterval();
    }
    private void Update()
    {
        if(tempInterval <= 0 && InteractionManager.Instance.canStartEvents)
        {
            int randChance = Random.Range(0, 100);
            if (randChance < spawnChance && canLaunchFlies)
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
        List<Cell> cellsWithPlants = new List<Cell>();
        for(int i = 0; i < cells.Length; i++)
        {
            if(cells[i].isOccupied)
            {
                cellsWithPlants.Add(cells[i]);
            }
        }
        if (cellsWithPlants.Count >= plantsToAttack)
        {
            List<Cell> shuffled = cellsWithPlants.OrderBy(_ => Random.value).ToList();
            for (int i = 0; i < plantsToAttack; i++)
            {
                Plant plant = shuffled[i].GetComponentInChildren<Plant>();
                AudioManager.Instance.PlaySfxSound(fliesLaunchSound, 0.2f, 0.95f, 1.05f);
                plant.fliesAlert.SetActive(true);
            }
        }
        else
        {
            Debug.Log("No plants to attack(flies)");
        }
        GenerateRandomInterval();

    }
}