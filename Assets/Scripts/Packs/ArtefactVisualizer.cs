using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArtefactVisualizer : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text artefactName;
    [SerializeField] private TMP_Text description;
    private ArtefactData data;
    public ArtefactData Data => data;

    public void SetData(ArtefactData newData)
    {
        data = newData;
        VisualizeArtefact();
    }
    public void VisualizeArtefact()
    {
        icon.sprite = data.icon;
        artefactName.text = data.name;
        description.text = data.description;
    }
}
