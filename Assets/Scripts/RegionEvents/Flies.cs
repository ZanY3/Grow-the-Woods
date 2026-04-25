using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Flies : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
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
        if (timerToDestroy > 0)
        {
            timerImg.fillAmount = timerToDestroy / startTimeToDestroy;
            timerToDestroy -= Time.deltaTime;
        }
        else
        {
            DestroyPlant();
        }
        if (timerToDestroy >= startTimeToDestroy)
        {
            timerToDestroy = startTimeToDestroy;
            gameObject.SetActive(false);
        }
    }
    public void DestroyPlant()
    {
        Cell cell = plant.GetComponentInParent<Cell>();
        cell.isOccupied = false;
        plant.GetComponent<Plant>().fliesAlert.SetActive(false);
        Debug.Log("FLIES HAS DESTROYED PLANT");
        Destroy(plant);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        //tooltip on
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //tooltip off
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        timerToDestroy += 0.5f;
    }
}
