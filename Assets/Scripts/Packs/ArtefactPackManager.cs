using System.Collections.Generic;
using UnityEngine;

public class ArtefactPackManager : ItemPickerBase
{
    public static ArtefactPackManager Instance;

    [SerializeField] private ArtefactsManager artefactsManager;

    [Header("Artifacts")]
    [SerializeField] private List<ArtefactData> allArtifacts;
    [SerializeField] private int artifactsPerPack = 3;
    [SerializeField] private ArtefactVisualizer[] artifactSlots;
    [HideInInspector] public ArtefactData selectedArtifact;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        cardRects = new RectTransform[artifactSlots.Length];
        for (int i = 0; i < artifactSlots.Length; i++)
            cardRects[i] = artifactSlots[i].GetComponent<RectTransform>();
    }

    protected override void FillCards()
    {
        var pack = GenerateArtifactPack();
        for (int i = 0; i < artifactSlots.Length; i++)
        {
            bool active = i < pack.Count;
            artifactSlots[i].gameObject.SetActive(active);
            if (active)
            {
                artifactSlots[i].SetData(pack[i]);
            }
        }
    }

    protected override void OnSelected(int index)
    {
        selectedArtifact = artifactSlots[index].Data;
    }

    protected override void OnDeselected()
    {
        selectedArtifact = null;
    }

    protected override void OnConfirmed(int index)
    {
        artefactsManager.AddArtefact(selectedArtifact);
    }

    private List<ArtefactData> GenerateArtifactPack()
    {
        var pack = new List<ArtefactData>();
        var pool = new List<ArtefactData>(allArtifacts);

        for (int i = 0; i < artifactsPerPack && pool.Count > 0; i++)
        {
            int idx = Random.Range(0, pool.Count);
            pack.Add(pool[idx]);
            pool.RemoveAt(idx);
        }
        return pack;
    }
}
