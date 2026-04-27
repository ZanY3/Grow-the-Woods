using UnityEngine;

public class ArtefactsManager : MonoBehaviour
{
    [SerializeField] private ArtefactSlot[] slots;
    [SerializeField] private ArtefactData[] allArtefacts;

    private void Start()
    {
        int randNum = Random.Range(0, allArtefacts.Length);
        for (int i = 0; i < randNum; i++)
        {
            if (!slots[i].isOccupied)
            {
                slots[i].SetArtefact(allArtefacts[randNum]);
            }
        }
    }

}
