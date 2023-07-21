using UnityEngine;
using System.Collections.Generic;

public class BasicBehaviour : MonoBehaviour
{
	public Transform playerCamera;                        
	public float turnSmoothingFactor  = 0.06f;                   
	public float sprintFieldOfView  = 100f;                        
	public string sprintButton = "Sprint";               

	private float horizontalAxis;                                      
	private float verticalAxis;                                     
	private int currentBehaviour;                         
	private int defaultBehaviour;                         
	private int behaviourLocked;                         
	private Vector3 lastFacingDirection;                        
	private Animator animatorController;                               
	private ThirdPersonOrbitCamBasic cameraScript;          
	private bool isSprinting ;                                 
	private bool changedFieldOfView;                             
	private int horizontalInputFloat;                                  
	private int verticalInputFloat ;                                   
	private List<GenericBehaviour> registeredBehaviours;           
	private List<GenericBehaviour> overridingBehaviours;  
	private Rigidbody rBody;                             
	private int groundedBool;                             
	private Vector3 colExtents;                           

	public float GetH { get { return horizontalAxis; } }
	public float GetV { get { return verticalAxis; } }

	public ThirdPersonOrbitCamBasic GetCameraScript { get { return cameraScript; } }

	public Rigidbody GetRigidBody { get { return rBody; } }

	public Animator GetAnim { get { return animatorController; } }

	public int GetDefaultBehaviour {  get { return defaultBehaviour; } }

	void Awake ()
	{
		registeredBehaviours = new List<GenericBehaviour> ();
		overridingBehaviours = new List<GenericBehaviour>();
		animatorController = GetComponent<Animator> ();
		horizontalInputFloat = Animator.StringToHash("H");
		verticalInputFloat = Animator.StringToHash("V");
		cameraScript = playerCamera.GetComponent<ThirdPersonOrbitCamBasic> ();
		rBody = GetComponent<Rigidbody> ();

		groundedBool = Animator.StringToHash("Grounded");
		colExtents = GetComponent<Collider>().bounds.extents;
	}

	void Update()
	{
		horizontalAxis = Input.GetAxis("Horizontal");
		verticalAxis = Input.GetAxis("Vertical");

		animatorController.SetFloat(horizontalInputFloat, horizontalAxis, 0.1f, Time.deltaTime);
		animatorController.SetFloat(verticalInputFloat, verticalAxis, 0.1f, Time.deltaTime);

		isSprinting = Input.GetButton (sprintButton);

		if(IsSprinting())
		{
			changedFieldOfView = true;
			cameraScript.SetFieldOfView(sprintFieldOfView);
		}
		else if(changedFieldOfView)
		{
			cameraScript.ResetFieldOfView();
			changedFieldOfView = false;
		}
		animatorController.SetBool(groundedBool, IsGrounded());
	}

	void FixedUpdate()
	{
		bool isAnyBehaviourActive = false;
		if (behaviourLocked > 0 || overridingBehaviours.Count == 0)
		{
			foreach (GenericBehaviour behaviour in registeredBehaviours)
			{
				if (behaviour.isActiveAndEnabled && currentBehaviour == behaviour.GetBehaviourCode())
				{
					isAnyBehaviourActive = true;
					behaviour.LocalFixedUpdate();
				}
			}
		}
		else
		{
			foreach (GenericBehaviour behaviour in overridingBehaviours)
			{
				behaviour.LocalFixedUpdate();
			}
		}

		if (!isAnyBehaviourActive && overridingBehaviours.Count == 0)
		{
			rBody.useGravity = true;
			Repositioning ();
		}
	}

	private void LateUpdate()
	{
		if (behaviourLocked > 0 || overridingBehaviours.Count == 0)
		{
			foreach (GenericBehaviour behaviour in registeredBehaviours)
			{
				if (behaviour.isActiveAndEnabled && currentBehaviour == behaviour.GetBehaviourCode())
				{
					behaviour.LocalLateUpdate();
				}
			}
		}
		else
		{
			foreach (GenericBehaviour behaviour in overridingBehaviours)
			{
				behaviour.LocalLateUpdate();
			}
		}

	}

	public void SubscribeBehaviour(GenericBehaviour behaviour)
	{
		registeredBehaviours.Add (behaviour);
	}

	public void RegisterDefaultBehaviour(int behaviourCode)
	{
		defaultBehaviour = behaviourCode;
		currentBehaviour = behaviourCode;
	}

	public void RegisterBehaviour(int behaviourCode)
	{
		if (currentBehaviour == defaultBehaviour)
		{
			currentBehaviour = behaviourCode;
		}
	}

	public void UnregisterBehaviour(int behaviourCode)
	{
		if (currentBehaviour == behaviourCode)
		{
			currentBehaviour = defaultBehaviour;
		}
	}

	public bool OverrideWithBehaviour(GenericBehaviour behaviour)
	{
		if (!overridingBehaviours.Contains(behaviour))
		{
			if (overridingBehaviours.Count == 0)
			{
				foreach (GenericBehaviour overriddenBehaviour in registeredBehaviours)
				{
					if (overriddenBehaviour.isActiveAndEnabled && currentBehaviour == overriddenBehaviour.GetBehaviourCode())
					{
						overriddenBehaviour.OnOverride();
						break;
					}
				}
			}
			overridingBehaviours.Add(behaviour);
			return true;
		}
		return false;
	}

	public bool RevokeOverridingBehaviour(GenericBehaviour behaviour)
	{
		if (overridingBehaviours.Contains(behaviour))
		{
			overridingBehaviours.Remove(behaviour);
			return true;
		}
		return false;
	}

	public bool IsOverriding(GenericBehaviour behaviour = null)
	{
		if (behaviour == null)
			return overridingBehaviours.Count > 0;
		return overridingBehaviours.Contains(behaviour);
	}

	public bool IsCurrentBehaviour(int behaviourCode)
	{
		return this.currentBehaviour == behaviourCode;
	}

	public bool GetTempLockStatus(int behaviourCodeIgnoreSelf = 0)
	{
		return (behaviourLocked != 0 && behaviourLocked != behaviourCodeIgnoreSelf);
	}

	public void LockTempBehaviour(int behaviourCode)
	{
		if (behaviourLocked == 0)
		{
			behaviourLocked = behaviourCode;
		}
	}

	public void UnlockTempBehaviour(int behaviourCode)
	{
		if(behaviourLocked == behaviourCode)
		{
			behaviourLocked = 0;
		}
	}

	public virtual bool IsSprinting()
	{
		return isSprinting && IsMoving() && CanSprint();
	}

	public bool CanSprint()
	{
		foreach (GenericBehaviour behaviour in registeredBehaviours)
		{
			if (!behaviour.AllowSprint ())
			{
				return false;
			}
		}
		foreach(GenericBehaviour behaviour in overridingBehaviours)
		{
			if (!behaviour.AllowSprint())
			{
				return false;
			}
		}
		return true;
	}

	public bool IsHorizontalMoving()
	{
		return horizontalAxis != 0;
	}

	public bool IsMoving()
	{
		return (horizontalAxis != 0)|| (verticalAxis != 0);
	}

	public Vector3 GetLastFacingDirection()
	{
		return lastFacingDirection;
	}

	public void SetLastFacingDirection(Vector3 direction)
	{
		lastFacingDirection = direction;
	}

	public void Repositioning()
	{
		if(lastFacingDirection != Vector3.zero)
		{
			lastFacingDirection.y = 0;
			Quaternion targetRotation = Quaternion.LookRotation (lastFacingDirection);
			Quaternion newRotation = Quaternion.Slerp(rBody.rotation, targetRotation, turnSmoothingFactor);
			rBody.MoveRotation (newRotation);
		}
	}

	public bool IsGrounded()
	{
		Ray ray = new Ray(this.transform.position + Vector3.up * 2 * colExtents.x, Vector3.down);
		return Physics.SphereCast(ray, colExtents.x, colExtents.x + 0.2f);
	}
}

public abstract class GenericBehaviour : MonoBehaviour
{
	protected int speedFloat;                      
	protected BasicBehaviour behaviourManager;     
	protected int behaviourCode;                   
	protected bool canSprint;                      

	void Awake()
	{
		behaviourManager = GetComponent<BasicBehaviour> ();
		speedFloat = Animator.StringToHash("Speed");
		canSprint = true;

		behaviourCode = this.GetType().GetHashCode();
	}

	public virtual void LocalFixedUpdate() { }
	public virtual void LocalLateUpdate() { }
	public virtual void OnOverride() { }

	public int GetBehaviourCode()
	{
		return behaviourCode;
	}

	public bool AllowSprint()
	{
		return canSprint;
	}
}
