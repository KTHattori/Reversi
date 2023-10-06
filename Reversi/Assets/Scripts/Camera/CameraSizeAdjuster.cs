using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CameraSizeAdjuster : MonoBehaviour
{
    [SerializeField] private Vector2Int _baseAspectRatio = new Vector2Int(9, 16);
    [SerializeField] private float _baseCameraSize = 5;
    [SerializeField, Range(1, 179)] private float _baseCameraFOV = 60;
    private Camera _camera;

    private float BaseAspect => _baseAspectRatio.x / (float)_baseAspectRatio.y;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void Start()
    {
        AdjustCameraSize();
    }

#if UNITY_EDITOR
    private void Update()
    {
        AdjustCameraSize();
    }
#endif

    private void AdjustCameraSize()
    {
        if (_camera.orthographic)
        {
            AdjustOrthographicCameraSize();
        }
        else
        {
            AdjustPerspectiveCameraSize();
        }
    }

    private void AdjustOrthographicCameraSize()
    {
        if (_camera.aspect < BaseAspect)
        {
            // letterboxing
            var baseHorizontalSize = _baseCameraSize * BaseAspect;
            var verticalSize = baseHorizontalSize / _camera.aspect;
            _camera.orthographicSize = verticalSize;
        }
        else
        {
            // pillarboxing
            _camera.orthographicSize = _baseCameraSize;
        }
    }

    private void AdjustPerspectiveCameraSize()
    {
        if (_camera.aspect < BaseAspect)
        {
            // letterboxing
            var baseVerticalSize = Mathf.Tan(_baseCameraFOV * 0.5f * Mathf.Deg2Rad);
            var baseHorizontalSize = baseVerticalSize * BaseAspect;
            var verticalSize = baseHorizontalSize / _camera.aspect;
            var verticalFov = Mathf.Atan(verticalSize) * Mathf.Rad2Deg * 2;
            _camera.fieldOfView = verticalFov;
        }
        else
        {
            // pillarboxing
            _camera.fieldOfView = _baseCameraFOV;
        }
    }
}
