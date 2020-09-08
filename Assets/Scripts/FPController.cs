using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPController : MonoBehaviour
{
	#region Fields

	[SerializeField] float _moveSpeed = 0.1f;
	[SerializeField] float _jumpForce = 300f;
	[SerializeField] float _mouseXSensitivity, _mouseYSensitivity;

	[SerializeField] GameObject _theCam;

	Rigidbody _theRB;
	CapsuleCollider _capsule;

	Quaternion _cameraRot, _characterRot;

	#endregion

	#region MonoBehaviour Methods

	void Start() 
	{
		_theRB = GetComponent<Rigidbody>();
		_capsule = GetComponent<CapsuleCollider>();
		_cameraRot = _theCam.transform.localRotation;
		_characterRot = transform.localRotation;
	}
	
	void Update() 
	{
		if (Input.GetKeyDown("space") && IsGrounded())
			_theRB.AddForce(0f, _jumpForce, 0f);
	}

	void FixedUpdate()
	{
		float yRot = Input.GetAxis("Mouse X") * _mouseXSensitivity;
		float xRot = Input.GetAxis("Mouse Y") * _mouseYSensitivity;

		_cameraRot *= Quaternion.Euler(-xRot, 0, 0);
		_characterRot *= Quaternion.Euler(0, yRot, 0);

		transform.localRotation = _characterRot;
		_theCam.transform.localRotation = _cameraRot;

		float x = Input.GetAxis("Horizontal");
		float z = Input.GetAxis("Vertical");

		transform.position += new Vector3(x * _moveSpeed, 0f, z * _moveSpeed);
	}
	#endregion

	#region Public Methods


	#endregion

	#region Private Methods

	bool IsGrounded()
	{
		RaycastHit hitInfo;

		if(Physics.SphereCast(transform.position,_capsule.radius,Vector3.down,out hitInfo, (_capsule.height / 2) - _capsule.radius + 0.1f))
		{
			return true;
		}
		return false;
	}
	#endregion
}
