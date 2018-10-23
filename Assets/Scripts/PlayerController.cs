using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour

{
    [SerializeField]
    private float sensitivityX = 1f;

    [SerializeField]
    private float _sensitivityY = 1f;

    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    private Transform _firePoint;

    [SerializeField]
    private GameObject _bulletPrefab;

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

    private float currentX;

    private float currentY;

    private Transform _playerTransform;

    private Animator _playerAnim;

    private PlayerModel _model;

    private NetworkAnimator _netAnim;

    private Vector3 _playerLastPosition;

    private float _currentSpeedX;

    private float _currentSpeedZ;

Vector3 targetPos;
    Quaternion rotation;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _playerTransform = transform;
        _playerAnim = GetComponent<Animator>();
        _model = GetComponent<PlayerModel>();
        _playerLastPosition = transform.position;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _playerTransform = transform;
        _playerAnim = GetComponent<Animator>();
        _model = GetComponent<PlayerModel>();
    }

    public override void PreStartClient()
    {
        base.PreStartClient();
        _netAnim = GetComponent<NetworkAnimator>();
        _netAnim.SetParameterAutoSend(0, true);
        _netAnim.SetParameterAutoSend(1, true);
        _netAnim.SetParameterAutoSend(2, true);
        _netAnim.SetParameterAutoSend(3, true);
        _netAnim.SetParameterAutoSend(4, true);
        _netAnim.SetParameterAutoSend(5, true);
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        currentX += Input.GetAxis("Mouse X");
        currentY += Input.GetAxis("Mouse Y");

        _playerTransform.rotation = Quaternion.Euler(0f, currentX * sensitivityX, 0);

        _currentSpeedX = (_playerTransform.position.x - _playerLastPosition.x) / Time.deltaTime;
        _currentSpeedZ = (_playerTransform.position.z - _playerLastPosition.z) / Time.deltaTime;

        _playerLastPosition = _playerTransform.position;

        _playerAnim.SetFloat("XSpeed", _currentSpeedX);
        _playerAnim.SetFloat("ZSpeed", _currentSpeedZ);

        if (Input.GetButtonDown("Fire1"))
            Fire();

        if (Input.GetAxis("Vertical") > 0)
        {
            _playerTransform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            _playerTransform.Translate(Vector3.back * speed * Time.deltaTime);
        }
        else if (Input.GetAxis("Horizontal") > 0)
        {
            _playerTransform.Translate(Vector3.right * speed * Time.deltaTime);
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            _playerTransform.Translate(Vector3.left * speed * Time.deltaTime);
        }       
    }

    private void LateUpdate()
    {
        if (isLocalPlayer)
        {
            var dir = new Vector3(0, _distanceToPlayerY, -_distanceToPlayerZ);
            var rot = Quaternion.Euler(-Mathf.Clamp(currentY * _sensitivityY, -_maxAngleOfCameraDiviation, _maxAngleOfCameraDiviation), _playerTransform.rotation.eulerAngles.y, 0f);
            Camera.main.transform.position = (_playerTransform.position + rot * dir);
            Camera.main.transform.LookAt(_playerTransform.position + Vector3.up * _offsetCameraY);
        }
    }

    private void Fire()
    {
        rotation = _model.GetRotation();
        CmdFire(rotation);
    }

    [Command]
    private void CmdFire(Quaternion _rotation)
    {
        if (isClient)
        {
            var bullet = Instantiate(_bulletPrefab, _firePoint.position, _rotation);
            NetworkServer.Spawn(bullet);
        }
    }
}
