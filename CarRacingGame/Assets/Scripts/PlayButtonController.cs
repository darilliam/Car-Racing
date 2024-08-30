using UnityEngine;
using UnityEngine.UI;

public class PlayButtonController : MonoBehaviour
{
    private bool _isPlayButtonPressed = false;
    [SerializeField] private Button playButton;
    [SerializeField] private GameObject canvas;
    [SerializeField] private PlacementController placementController;
    [SerializeField] private SwitchCameraController switchCameraController;
    [SerializeField] private ObjectPlacer objectPlacer;

    public void OnPlayButtonPressed()
    {
        _isPlayButtonPressed = true;
        canvas.SetActive(false);
        placementController.StopPlacement();
        switchCameraController.CarCameraActive();
    }

    public bool GetPlayButtonState() => _isPlayButtonPressed;

    private void Update()
    {
        CheckArrayAndSetButton();
    }

    private void CheckArrayAndSetButton()
    {
        if (objectPlacer.GetPlaceObjectsCount() == 0)
        {
            playButton.interactable = false;
        }
        else
        {
            playButton.interactable = true;
        }
    }
}

