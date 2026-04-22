using UnityEngine;
using UnityEngine.UI;

public class Flies : MonoBehaviour
{
    [SerializeField] private GameObject plant;
    [SerializeField] private Image timerImg;
    [SerializeField] private float timerToDestroy = 0;

    private float startTimeToDestroy;
    private void Start()
    {
        startTimeToDestroy = timerToDestroy;
    }
    private void Update()
    {
        if(timerToDestroy > 0)
        {
            timerImg.fillAmount = timerToDestroy;
            timerToDestroy -= Time.deltaTime;
        }
        else
        {
            
        }
    }
}
