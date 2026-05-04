using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Flies : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject plant;
    [SerializeField] private Image timerImg;
    [SerializeField] private float timerToDestroy = 0;

    [Space]
    [SerializeField] private AudioClip fliesClickSound;
    [SerializeField] private AudioClip fliesEndSound;
    [SerializeField] private AudioClip fliesFailSound;

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
            AudioManager.Instance.PlaySfxSound(fliesEndSound, 0.65f, 0.95f, 1.05f);
            timerToDestroy = startTimeToDestroy;
            gameObject.SetActive(false);
        }
    }
    public void DestroyPlant()
    {
        AudioManager.Instance.PlaySfxSound(fliesFailSound, 0.65f, 0.9f, 1.1f);
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
        AudioManager.Instance.PlaySfxSound(fliesClickSound, 0.8f, 0.9f, 1.1f);
        timerToDestroy += 0.5f;
    }
}
