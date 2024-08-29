using UnityEngine;

public interface IBuilldingState
{
    void EnndState();
    void OnAction(Vector3Int gridPos);
    void UpdateState(Vector3Int gridPos);
}