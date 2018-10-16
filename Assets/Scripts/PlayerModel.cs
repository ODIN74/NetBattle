using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerModel : NetworkBehaviour, IDamagable
{
    [SerializeField]
    private float _maxHealth = 100f;

    [SerializeField]
    private float _destroyDelay = 3f;

    [SerializeField]
    private float _offsetY = 80f;

    private Image _fillHealthBar;

    private Animator _anim;

    [SyncVar]
    private float _currentHealth;

    [HideInInspector]
    public float GetHealth { get { return _currentHealth; } private set { _currentHealth = value; } }

    [HideInInspector]
    public Vector3 TargetPoint { get; private set; }

    private Image _aim;

    private Camera _mainCamera;

    private MainCamera _mainCameraController;

    private Transform _rayPoint;

    private Color _normalAimColor;

    private Canvas _canvas;

    public override void OnStartLocalPlayer()
    {
        gameObject.name = "Local";
    }

    void Start ()
    {
        _mainCamera = Camera.main;
        _mainCameraController = _mainCamera.GetComponent<MainCamera>();
        _anim = GetComponent<Animator>();
        _canvas = GetComponentInParent<Canvas>();
        _aim = GameObject.FindWithTag("Aim").GetComponent<Image>();
        _normalAimColor = _aim.color;
        GetHealth = _maxHealth;
        _rayPoint = _mainCamera.GetComponentInChildren<Transform>();
        var _images = GetComponentsInChildren<Image>();
        foreach(var obj in _images)
        {
            if(obj.type != Image.Type.Filled)
            {
                continue;
            }
            else
            {
                _fillHealthBar = obj;
                break;
            }
        }
        if (_fillHealthBar)
            _fillHealthBar.fillAmount = GetHealth / _maxHealth;
	}

    public void FixedUpdate()
    {

        var raycastStartPoint = new Vector3(_mainCamera.pixelWidth / 2, _mainCamera.pixelHeight / 2, 0);
        var ray = _mainCamera.ScreenPointToRay(raycastStartPoint);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        if (hit.collider != null && (1 << hit.collider.gameObject.layer) == LayerMask.NameToLayer("Player"))
            _aim.color = Color.red;
        else if (!_aim.color.Equals(_normalAimColor))
            _aim.color = _normalAimColor;            
    }

    public Vector3 GetTargetPoint()
    {
        var raycastStartPoint = new Vector3(Screen.width / 2, Screen.height / 2 + _offsetY, 0);
        var ray = _mainCamera.ScreenPointToRay(raycastStartPoint);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        return hit.point;
    }

    public void Damage (float damage)
    {
        if (GetHealth <= 0)
            return;

        GetHealth -= damage;
        if(_fillHealthBar)
            _fillHealthBar.fillAmount = GetHealth / _maxHealth;

        if (GetHealth <= 0)
            Death();
    }

    public void Death()
    {
        if (_anim)
        {
            _anim.ResetTrigger("Forward");
            _anim.ResetTrigger("Backward");
            _anim.ResetTrigger("ToRight");
            _anim.ResetTrigger("ToLeft");
            _anim.ResetTrigger("Idle");
            _anim.SetTrigger("Die");
            Destroy(gameObject, _destroyDelay);
        }
        else
            Destroy(gameObject);
    }

    private void LateUpdate()
    {
        _canvas.transform.LookAt(Camera.main.transform, Vector3.up);
    }
}
