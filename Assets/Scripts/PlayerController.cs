using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour

{
    [SerializeField]
    private float sensitivityX = 1f;



    [SerializeField]
    private float speed =5f;

    private Transform _playerTransform;
    private Animator _playerAnim;

    private float currentX;


	// Use this for initialization
	void Start ()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _playerTransform = transform;
        _playerAnim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        currentX += Input.GetAxis("Mouse X");

        _playerTransform.rotation = Quaternion.Euler(0f, currentX * sensitivityX, 0);

        if (Input.GetAxis("Vertical") > 0)
        {
            _playerTransform.Translate(Vector3.forward * speed * Time.deltaTime);
            if (_playerAnim)
            {
                _playerAnim.ResetTrigger("Idle");
                _playerAnim.ResetTrigger("Backward");
                _playerAnim.ResetTrigger("ToLeft");
                _playerAnim.ResetTrigger("ToRight");
                _playerAnim.SetTrigger("Forward");
            }
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            _playerTransform.Translate(Vector3.back * speed * Time.deltaTime);
            if (_playerAnim)
            {
                _playerAnim.ResetTrigger("Idle");
                _playerAnim.ResetTrigger("Forward");
                _playerAnim.ResetTrigger("ToLeft");
                _playerAnim.ResetTrigger("ToRight");
                _playerAnim.SetTrigger("Backward");
            }
                
        }
        else if (Input.GetAxis("Horizontal") > 0)
        {
            _playerTransform.Translate(Vector3.right * speed * Time.deltaTime);
            if (_playerAnim)
            {
                _playerAnim.ResetTrigger("Idle");
                _playerAnim.ResetTrigger("Forward");
                _playerAnim.ResetTrigger("ToLeft");
                _playerAnim.ResetTrigger("Backward");
                _playerAnim.SetTrigger("ToRight");
            }
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            _playerTransform.Translate(Vector3.left * speed * Time.deltaTime);
            if (_playerAnim)
            {
                _playerAnim.ResetTrigger("Idle");
                _playerAnim.ResetTrigger("Forward");
                _playerAnim.ResetTrigger("ToRight");
                _playerAnim.ResetTrigger("Backward");
                _playerAnim.SetTrigger("ToLeft");
            }
        }
        else
        {
            if (_playerAnim)
            {
                _playerAnim.ResetTrigger("ToRight");
                _playerAnim.ResetTrigger("Forward");
                _playerAnim.ResetTrigger("ToLeft");
                _playerAnim.ResetTrigger("Backward");
                _playerAnim.SetTrigger("Idle");
            }

        }
            
    }
}
