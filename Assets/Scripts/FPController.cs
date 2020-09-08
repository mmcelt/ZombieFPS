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

	Rigidbody _theRB;
	CapsuleCollider _capsule;

	Quaternion _cameraRot, _characterRot;

	bool _cursorIsLocked = true, _lockCursor = true;

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
		if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
			_theRB.AddForce(0f, _jumpForce, 0f);

		if (Input.GetKeyDown(KeyCode.F))
			_theAnim.SetBool("arm", !_theAnim.GetBool("arm"));

		if (Input.GetMouseButtonDown(0))
			_theAnim.SetBool("fire", true);
		else if (Input.GetMouseButtonUp(0))
			_theAnim.SetBool("fire", false);
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

		float x = Input.GetAxis("Horizontal") * _moveSpeed;
		float z = Input.GetAxis("Vertical") * _moveSpeed;

		transform.position += _theCam.transform.forward * z + _theCam.transform.right * x;//new Vector3(x * _moveSpeed, 0f, z * _moveSpeed);

		UpdateCursorLock();
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
	#endregion
}
