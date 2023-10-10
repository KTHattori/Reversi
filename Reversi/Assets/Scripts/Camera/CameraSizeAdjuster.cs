using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CameraSizeAdjuster : MonoBehaviour
{
    /// <summary>
    /// 基準となるアスペクト比
    /// </summary>
    [SerializeField]
    private Vector2Int _baseAspectRatio = new Vector2Int(9, 16);

    /// <summary>
    /// 基準となるカメラサイズ
    /// </summary>
    [SerializeField]
    private float _baseCameraSize = 5;

    /// <summary>
    /// 基準となる視野角
    /// </summary>
    [SerializeField, Range(1, 179)]
    private float _baseCameraFOV = 60;

    /// <summary>
    /// カメラコンポーネント
    /// </summary>
    private Camera _camera;

    /// <summary>
    /// アスペクト比を得るためのプロパティ
    /// </summary>
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

    /// <summary>
    /// カメラサイズの調整
    /// </summary>
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

    /// <summary>
    /// 平行投影カメラサイズの調整
    /// </summary>
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

    /// <summary>
    /// 透視投影カメラサイズの調整
    /// </summary>
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
