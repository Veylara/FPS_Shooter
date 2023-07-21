using UnityEngine;
using System.Collections;

public class AimBehaviourBasic : GenericBehaviour
{
	public string aimButton = "Aim", shoulderButton = "Aim Shoulder";    
	public Texture2D crosshair;                                           
	public float aimTurnSmoothing = 0.15f;                                
	public Vector3 aimPivotOffset = new Vector3(0f, 0.15f,  0f);         
	public Vector3 aimCamOffset   = new Vector3(-1.0f, 0.15f, -2.23f);         

	private int aimBool;                                                  
	private bool aim;                                                     

	void Start ()
	{
		aimBool = Animator.StringToHash("Aim");
	}

	void Update ()
	{
		float aimInputValue = Input.GetAxisRaw(aimButton);

		if (Input.GetAxisRaw(aimButton) != 0 && !aim)
		{
			StartCoroutine(ToggleAimOn());
		}
		else if (aim && Input.GetAxisRaw(aimButton) == 0)
		{
			StartCoroutine(ToggleAimOff());
		}

		canSprint = !aim;

		if (aim && Input.GetButtonDown (shoulderButton))
		{
			aimCamOffset.x = aimCamOffset.x * (-1);
			aimPivotOffset.x = aimPivotOffset.x * (-1);
		}

		behaviourManager.GetAnim.SetBool (aimBool, aim);
	}

	private IEnumerator ToggleAimOn()
	{

		yield return new WaitForSeconds(0.05f);
		if (behaviourManager.GetTempLockStatus(this.behaviourCode) || behaviourManager.IsOverriding(this))
		{
			yield return false;
		}
		else
		{
			aim = true;
			int signal = 1;
			aimCamOffset.x = Mathf.Abs(aimCamOffset.x) * signal;
			aimPivotOffset.x = Mathf.Abs(aimPivotOffset.x) * signal;
			yield return new WaitForSeconds(0.1f);
			behaviourManager.GetAnim.SetFloat(speedFloat, 0);
			behaviourManager.OverrideWithBehaviour(this);
		}
	    yield return null; 
	}

	private IEnumerator ToggleAimOff()
	{
		aim = false;
		yield return new WaitForSeconds(0.3f);
		behaviourManager.GetCameraScript.ResetTargetOffsets();
		behaviourManager.GetCameraScript.ResetMaxVerticalAngle();
		yield return new WaitForSeconds(0.05f);
		behaviourManager.RevokeOverridingBehaviour(this);
	    yield return null; 
	}

	public override void LocalFixedUpdate()
	{
		if(aim)
		{
			behaviourManager.GetCameraScript.SetTargetOffsets (aimPivotOffset, aimCamOffset);
		}
	}

	public override void LocalLateUpdate()
	{
		AimManagement();
	}

	void AimManagement()
	{
		Rotating();
	}

	void Rotating()
	{
		Vector3 forward = behaviourManager.playerCamera.TransformDirection(Vector3.forward);
		forward.y = 0.0f;
		forward = forward.normalized;

		Quaternion targetRotation =  Quaternion.Euler(0, behaviourManager.GetCameraScript.GetH, 0);

		float minSpeed = Quaternion.Angle(transform.rotation, targetRotation) * aimTurnSmoothing;

		behaviourManager.SetLastFacingDirection(forward);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, minSpeed * Time.deltaTime);

	}

	void OnGUI () 
	{
		if (crosshair)
		{
			float mag = behaviourManager.GetCameraScript.GetCurrentPivotMagnitude(aimPivotOffset);
			if (mag < 0.05f)
				GUI.DrawTexture(new Rect(Screen.width / 2 - (crosshair.width * 0.5f),
										 Screen.height / 2 - (crosshair.height * 0.5f),
										 crosshair.width, crosshair.height), crosshair);
		}
	}
}
