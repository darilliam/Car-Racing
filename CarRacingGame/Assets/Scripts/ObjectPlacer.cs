using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] private List<GameObject> placedObjects = new List<GameObject>();
    [SerializeField] private PlayButtonController playButtonController;

    public int PlaceObject(GameObject prefab, Vector3 gridPos)
    {
        GameObject newGameObject = Instantiate(prefab);
        newGameObject.transform.position = gridPos;
        placedObjects.Add(newGameObject);
        playButtonController.CheckArrayAndSetButton();
        return placedObjects.Count - 1;
    }

    public void RemoveObject(int gameObjectIndex)
    {
        if(placedObjects.Count <= gameObjectIndex || placedObjects[gameObjectIndex] == null) return;

        Destroy(placedObjects[gameObjectIndex]);
        placedObjects[gameObjectIndex] = null;
        playButtonController.CheckArrayAndSetButton();
    }

    public int GetPlaceObjectsCount() => placedObjects.FindAll(obj => obj != null).Count;
}
