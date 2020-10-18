using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
	#region Fields

	[SerializeField] AudioSource _shot;
	[SerializeField] FPController _fpc;

	#endregion

	#region MonoBehaviour Methods

	void Start() 
	{
		
	}
	#endregion

	#region Public Methods

	public void Fire()
	{
		_shot.Play();
	}

	public void ResetTrigger()
	{
		_fpc._shotFired = false;
	}
	#endregion

	#region Private Methods


	#endregion
}
