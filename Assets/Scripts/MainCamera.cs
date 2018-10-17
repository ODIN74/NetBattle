using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MainCamera : NetworkBehaviour
{
    [SerializeField]
    [Range(1f, 5f)]
    private float _distanceToPlayerY = 1f;

    [SerializeField]
    [Range(1f, 10f)]
    private float _distanceToPlayerZ = 1f;

    [SerializeField]
    [Range(1f, 5f)]
    private float _offsetCameraY = 1f;

    [SerializeField]
    private float _maxAngleOfCameraDiviation = 10f;

    [SerializeField]
    private float _sensitivityY = 1f;

    [HideInInspector]
    public NetworkInstanceId PlayerID { get; set; }
    public GameObject GameObject { get; internal set; }

    private Transform _playerTransform;

    private Transform _cameraTransform;

    private float _currentY;


    private void Start ()
    {
        _cameraTransform = transform;
    }

    private void Update()
    {
        _currentY += Input.GetAxis("Mouse Y");
    }

     private void LateUpdate ()
    {
        if (_playerTransform)
        {
            var dir = new Vector3(0, _distanceToPlayerY, -_distanceToPlayerZ);
            var rot = Quaternion.Euler(-Mathf.Clamp(_currentY * _sensitivityY, -_maxAngleOfCameraDiviation, _maxAngleOfCameraDiviation), _playerTransform.rotation.eulerAngles.y, 0f);

            _cameraTransform.position = (_playerTransform.position + rot * dir);

            _cameraTransform.LookAt(_playerTransform.position + Vector3.up * _offsetCameraY);
        }
        else
            _playerTransform = GameObject.Find("Local").transform;
    }
}
