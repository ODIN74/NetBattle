using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField]
    [Range(1f, 5f)]
    private float _distanceToPlayerY = 1f;

    [SerializeField]
    [Range(1f,5f)]
    private float _distanceToPlayerZ = 1f;

    [SerializeField]
    [Range(1f, 5f)]
    private float _offsetCameraY = 1f;

    [SerializeField]
    private float _maxAngleOfCameraDiviation = 10f;

    [SerializeField]
    private float _sensitivityY = 1f;

    private Transform _playerTransform;

    private Transform _cameraTransform;

    private float _currentY;

    private void Start ()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _cameraTransform = transform;
    }

    private void Update()
    {
        _currentY += Input.GetAxis("Mouse Y");
    }

     private void LateUpdate ()
    {
        var dir = new Vector3(0, _distanceToPlayerY, -_distanceToPlayerZ);
        var rot = Quaternion.Euler(-Mathf.Clamp(_currentY * _sensitivityY, -_maxAngleOfCameraDiviation, _maxAngleOfCameraDiviation), _playerTransform.rotation.eulerAngles.y, 0f);

        _cameraTransform.position = (_playerTransform.position + rot * dir);

        _cameraTransform.LookAt(_playerTransform.position + Vector3.up * _offsetCameraY);
        
    }
}
