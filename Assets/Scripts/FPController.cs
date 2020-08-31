using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPController : MonoBehaviour
{
	#region Fields

	[SerializeField] float _speed = 0.1f;

	#endregion

	#region MonoBehaviour Methods

	void Start() 
	{
		
	}
	
	void Update() 
	{
		
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


	#endregion
}
