using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BulletModel : NetworkBehaviour
{
    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _bulletLifetime = 10f;

    [SerializeField]
    private int _damage = 10;

    private Transform _bulletTransform;

    private bool _isHitted = false;

    public Vector3 targetPosition;

    public void Start()
    {
        StartCoroutine(BulletDestroyer(_bulletLifetime));
    }

    private void FixedUpdate()
    {
        if (_isHitted)
            return;

        var finalPos = transform.position + transform.forward.normalized * _speed * Time.fixedDeltaTime;

        RaycastHit hit;
        if (Physics.Linecast(transform.position, finalPos, out hit))
        {
            if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
            {
                _isHitted = true;
                transform.position = hit.point;
                Destroy(gameObject);
            }

            IDamagable d = hit.collider.gameObject.GetComponent<IDamagable>();
                if (d != null)
                {
                    _isHitted = true;
                    transform.position = hit.point;
                    d.Damage(_damage);
                    Destroy(gameObject);
                }
        }
        else
        {
            transform.position = finalPos;
        }

    }

    private IEnumerator BulletDestroyer(float bulletLifetime)
    {
        yield return new WaitForSeconds(bulletLifetime);
        Destroy(gameObject);
        StopCoroutine("BulletDestroyer");
    }
}
