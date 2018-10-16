using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletModel : MonoBehaviour
{
    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _bulletLifetime = 10f;

    [SerializeField]
    private float _damage = 10f;

    private Transform _bulletTransform;

    private bool _isHitted;

    private Vector3 _targetPosition;

    private ParticleSystem _particle;

    public void Initialize(Vector3 targetPosition)
    {

        //_bulletTransform.position = _firePoint.position;
        //_bulletTransform.rotation = _firePoint.rotation;
        //Instantiate(gameObject, _bulletTransform.position, _bulletTransform.rotation);
        _particle = GetComponent<ParticleSystem>();
        _targetPosition = targetPosition;
        _isHitted = false;
        StartCoroutine(BulletDestroyer(_bulletLifetime));
    }

    private void FixedUpdate()
    {

        if (_isHitted)
            return;

        Vector3 finalPos = transform.position + (_targetPosition - transform.position).normalized * _speed * Time.fixedDeltaTime;
        RaycastHit hit;
        if (Physics.Linecast(transform.position, finalPos, out hit))
        {
            if ((1 << hit.collider.gameObject.layer) != LayerMask.NameToLayer("Player"))
            {
                _isHitted = true;
                transform.position = hit.point;

                Destroy(gameObject);
            }

            IDamagable d = hit.collider.GetComponent<IDamagable>();
            if (d != null)
            {
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
