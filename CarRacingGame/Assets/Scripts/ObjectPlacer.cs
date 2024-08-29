using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] private List<GameObject> _placedObjects = new List<GameObject>();

    public int PlaceObject(GameObject prefab, Vector3 gridPos)
    {
        GameObject newGameObject = Instantiate(prefab);
        newGameObject.transform.position = gridPos;
        _placedObjects.Add(newGameObject);
        return _placedObjects.Count - 1;
    }
}
