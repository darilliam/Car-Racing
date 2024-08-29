using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewRoadPartsController : MonoBehaviour
{
    [SerializeField] private float previewYOffset = 0.06f;
    [SerializeField] private GameObject cellTag;
    private GameObject _previewObject;

    [SerializeField] Material previewMaterialPrefab;
    private Material _previewMaterialInstance;

    private Renderer _celTagRenderer;

    private void Start()
    {
        _previewMaterialInstance = new Material(previewMaterialPrefab);
        cellTag.SetActive(false);
        _celTagRenderer = cellTag.GetComponentInChildren<Renderer>();
    }

    public void StartShowingPreviewOfPlacement(GameObject prefab, Vector2Int size)
    {
        _previewObject = Instantiate(prefab);
        PreparePreview(_previewObject);
        PrepareCursor(size);
        cellTag.SetActive(true);
    }

    private void PrepareCursor(Vector2Int size)
    {
        if(size.x > 0 || size.y > 0)
        {
            cellTag.transform.localScale = new Vector3(size.x, 1, size.y);
            _celTagRenderer.material.mainTextureScale = size;
        }
    }

    private void PreparePreview(GameObject previewObject)
    {
        Renderer[] renderers = _previewObject.GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for(int i = 0; i < materials.Length; i++)
            {
                materials[i] = _previewMaterialInstance;
            }
            renderer.materials = materials;
        }
    }

    public void StopShowingPreview()
    {
        cellTag.SetActive(false);
        Destroy(_previewObject);
    }

    public void UpdatePosition(Vector3 pos, bool validity)
    {
        MovePreview(pos);
        MoveCellTag(pos);
        ApplyFeedback(validity);
    }

    private void ApplyFeedback(bool validity)
    {
        Color color = validity ? Color.white : Color.red;
        _celTagRenderer.material.color = color;
        color.a = 0.5f;
        _previewMaterialInstance.color = color;
    }

    private void MoveCellTag(Vector3 pos)
    {
        cellTag.transform.position = pos;
        Vector3 posE = cellTag.transform.position;
        posE.x += 5;
        cellTag.transform.position = posE;
    }

    private void MovePreview(Vector3 pos)
    {
        _previewObject.transform.position = new Vector3(pos.x, pos.y + previewYOffset, pos.z);
    }
}
