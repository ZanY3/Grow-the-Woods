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
        Debug.Log("FLIES LAUNCHED!");
    }
}