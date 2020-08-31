using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPController : MonoBehaviour
{
	#region Fields

	[SerializeField] float _speed = 0.1f;
	[SerializeField] float _jumpForce = 300f;

	Rigidbody _theRB;
	CapsuleCollider _capsule;

	#endregion

	#region MonoBehaviour Methods

	void Start() 
	{
		_theRB = GetComponent<Rigidbody>();
		_capsule = GetComponent<CapsuleCollider>();
	}
	
	void Update() 
	{
		if (Input.GetKeyDown("space") && IsGrounded())
			_theRB.AddForce(0f, _jumpForce, 0f);
	}

	void FixedUpdate()
	{
		float x = Input.GetAxis("Horizontal");
		float z = Input.GetAxis("Vertical");

		transform.position += new Vector3(x * _speed, 0f, z * _speed);
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
