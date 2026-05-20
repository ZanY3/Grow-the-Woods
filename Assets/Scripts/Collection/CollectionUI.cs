using System.Collections.Generic;
using UnityEngine;

public class CollectionUI : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform gridParent;

    public void ChangeBookState(bool state)
    {
        gameObject.SetActive(state);
        if (!state) return;

        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        List<PlantData> plants = CollectionManager.Instance.GetAllPlants();

        if (plants == null) return;

        foreach (var plant in plants)
        {
            if (plant == null) continue;

            GameObject newSlot = Instantiate(slotPrefab, gridParent);

            bool isUnlocked = CollectionManager.Instance.IsPlantUnlocked(plant.id);

            CollectionSlot slotScript = newSlot.GetComponent<CollectionSlot>();
            if (slotScript != null)
            {
                slotScript.Setup(plant, isUnlocked);
            }
            
        }
    }
}
