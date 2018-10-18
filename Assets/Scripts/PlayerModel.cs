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
    private Transform _bulletSpawner;

    private Image _fillHealthBar;

    private Animator _anim;

    [SyncVar(hook ="OnHealthChange")]
    private float _currentHealth;

    [HideInInspector]
    public Vector3 TargetPoint { get; private set; }

    private Image _aim;

    private Camera _mainCamera;

    private Color _normalAimColor;

    private Canvas _canvas;

    public override void OnStartLocalPlayer()
    {
        gameObject.name = "Local";
        _currentHealth = _maxHealth;
    }

    void Start ()
    {
        _mainCamera = Camera.main;
        _anim = GetComponent<Animator>();
        _canvas = GetComponent<Canvas>();
        _aim = GameObject.FindWithTag("Aim").GetComponent<Image>();
        _normalAimColor = _aim.color;
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
        //if (_fillHealthBar)
        //   _fillHealthBar.fillAmount = _currentHealth / _maxHealth;
	}

    public void FixedUpdate()
    {

        var raycastStartPoint = new Vector3(_mainCamera.pixelWidth / 2, _mainCamera.pixelHeight / 2, 0);
        var ray = _mainCamera.ScreenPointToRay(raycastStartPoint);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            _aim.color = Color.red;
        else if (!_aim.color.Equals(_normalAimColor))
            _aim.color = _normalAimColor;            
    }

    public Quaternion GetRotation()
    {
            var raycastStartPoint = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            var ray = _mainCamera.ScreenPointToRay(raycastStartPoint);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);

            return Quaternion.LookRotation(hit.point - _bulletSpawner.position, Vector3.up);
    }

    public void Damage (float damage)
    {
        if (!isServer)
            return;

        if (_currentHealth <= 0)
            return;

        _currentHealth -= damage;

        OnHealthChange(_currentHealth);

        if (_currentHealth <= 0)
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
        if(_canvas)
            _canvas.transform.LookAt(Camera.main.transform, Vector3.up);
    }

    private void OnHealthChange(float health)
    {
        _fillHealthBar.fillAmount = health / _maxHealth;
    }
}
