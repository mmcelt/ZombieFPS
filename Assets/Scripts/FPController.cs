using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPController : MonoBehaviour
{
	#region Fields

	[SerializeField] float _moveSpeed = 0.1f;
	[SerializeField] float _jumpForce = 300f;
	[SerializeField] float _mouseXSensitivity, _mouseYSensitivity;
	[SerializeField] float _minX = -80, _maxX = 80;

	[SerializeField] GameObject _theCam;
	[SerializeField] Animator _theAnim;
	[SerializeField] AudioSource[] _footsteps;
	[SerializeField] AudioSource _jumping, _landing, _ammoPickup, _healthPickup, _emptyChamber, _hurt, _death, _reload;

	Rigidbody _theRB;
	CapsuleCollider _capsule;

	Quaternion _cameraRot, _characterRot;

	bool _cursorIsLocked = true, _lockCursor = true;
	float _x, _z;

	public bool _shotFired;

	[Header("Inventory")]
	int _ammo;
	[SerializeField] int _maxAmmo = 50;
	int _ammoClip;
	[SerializeField] int _maxAmmoClip = 10;

	[Header("Health")]
	[SerializeField] int _maxHealth = 100;
	int _currentHealth;

	#endregion

	#region MonoBehaviour Methods

	void Start() 
	{
		_theRB = GetComponent<Rigidbody>();
		_capsule = GetComponent<CapsuleCollider>();
		_cameraRot = _theCam.transform.localRotation;
		_characterRot = transform.localRotation;
		_currentHealth = _maxHealth;
	}
	
	void Update() 
	{
		if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
		{
			_theRB.AddForce(0f, _jumpForce, 0f);
			_jumping.Play();
			if(_theAnim.GetBool("walking"))
				CancelInvoke("PlayFootstepAudio");
		}

		if (Input.GetKeyDown(KeyCode.F))
			_theAnim.SetBool("arm", !_theAnim.GetBool("arm"));

		if (Input.GetMouseButtonDown(0) && _theAnim.GetBool("arm") && !_shotFired && _ammoClip > 0)
		{
			_theAnim.SetTrigger("fire");
			_shotFired = true;
			//_shot.Play();
			_ammoClip--;

			Debug.Log("Ammo available: " + _ammo);
			Debug.Log("Ammo in clip: " + _ammoClip);
		}

		if (Input.GetMouseButtonDown(0) && _theAnim.GetBool("arm") && !_shotFired && _ammoClip <= 0)
			_emptyChamber.Play();

		if (Input.GetKeyDown(KeyCode.R) && _theAnim.GetBool("arm"))
		{
			_theAnim.SetTrigger("reload");

			int ammoNeededToFillClip = _maxAmmoClip - _ammoClip;
			int ammoAvailable = ammoNeededToFillClip < _ammo ? ammoNeededToFillClip : _ammo; //could use Mathf.Min(ammoNeededToFillClip, _ammo) instead
			_ammo -= ammoAvailable;
			_ammoClip += ammoAvailable;

			Debug.Log("Ammo Left: " + _ammo);
			Debug.Log("Ammo in clip: " + _ammoClip);

			_reload.Play();
		}

		if (_x != 0 || _z != 0)
		{
			if (!_theAnim.GetBool("walking"))
			{
				_theAnim.SetBool("walking", true);
				InvokeRepeating("PlayFootstepAudio", 0, 0.4f);

			}
		}
		else if (_theAnim.GetBool("walking"))
		{
			_theAnim.SetBool("walking", false);
			CancelInvoke("PlayFootstepAudio");
		}
	}

	void FixedUpdate()
	{
		float yRot = Input.GetAxis("Mouse X") * _mouseXSensitivity;
		float xRot = Input.GetAxis("Mouse Y") * _mouseYSensitivity;

		_cameraRot *= Quaternion.Euler(-xRot, 0, 0);
		_characterRot *= Quaternion.Euler(0, yRot, 0);

		_cameraRot = ClampRotationAroundXAxis(_cameraRot);

		transform.localRotation = _characterRot;
		_theCam.transform.localRotation = _cameraRot;

		_x = Input.GetAxis("Horizontal") * _moveSpeed;
		_z = Input.GetAxis("Vertical") * _moveSpeed;

		transform.position += transform.forward * _z + _theCam.transform.right * _x;//new Vector3(x * _moveSpeed, 0f, z * _moveSpeed);

		UpdateCursorLock();
	}

	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("Ammo") && _ammo < _maxAmmo)
		{
			_ammo = Mathf.Clamp(_ammo + 20, 0, _maxAmmo);

			Debug.Log("Ammo: " + _ammo);

			_ammoPickup.Play();
			Destroy(other.gameObject);
		}
		else if (other.gameObject.CompareTag("MedKit") && _currentHealth < _maxHealth)
		{
			_currentHealth = Mathf.Clamp(_currentHealth + 10, 0, _maxHealth);

			Debug.Log("Health: " + _currentHealth);

			_healthPickup.Play();
			Destroy(other.gameObject);
		}
		else if (other.gameObject.CompareTag("Lava"))
		{
			_currentHealth = Mathf.Clamp(_currentHealth -= 40, 0, _maxHealth);
			_hurt.Play();

			Debug.Log("Health: " + _currentHealth);

			if (_currentHealth == 0)
			{
				Debug.Log("YOU DIED!");
				_death.Play();
			}
		}
		else if (IsGrounded())
		{
			_landing.Play();
			if(_theAnim.GetBool("walking"))
				InvokeRepeating("PlayFootstepAudio", 0, 0.4f);
		}
	}
	#endregion

	#region Public Methods

	public void SetCursorLock(bool value)
	{
		_lockCursor = value;
		if (!_lockCursor)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

	public void UpdateCursorLock()
	{
		if (_lockCursor)
			InternalLockUpdate();
	}

	public void InternalLockUpdate()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
			_cursorIsLocked = false;
		else if (Input.GetMouseButtonUp(0))
			_cursorIsLocked = true;

		if (_cursorIsLocked)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else if (!_cursorIsLocked)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}
	#endregion

	#region Private Methods

	bool IsGrounded()
	{
		RaycastHit hitInfo;

		if(Physics.SphereCast(transform.position,_capsule.radius, Vector3.down, out hitInfo, (_capsule.height / 2) - _capsule.radius + 0.1f))
		{
			return true;
		}
		return false;
	}

	Quaternion ClampRotationAroundXAxis(Quaternion q)
	{
		//convert Quaternion to Euler angle
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1.0f;
		float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

		angleX = Mathf.Clamp(angleX, _minX, _maxX);

		//convert Euler back to Quaternion
		q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

		return q;
	}

	void PlayFootstepAudio()
	{
		AudioSource audioSource = new AudioSource();
		int n = Random.Range(1, _footsteps.Length);
		audioSource = _footsteps[n];
		audioSource.Play();
		_footsteps[n] = _footsteps[0];
		_footsteps[0] = audioSource;
	}
	#endregion
}
