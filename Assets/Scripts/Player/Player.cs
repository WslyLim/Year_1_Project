using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public static Player Instance { get; private set; }

	public event EventHandler<OnSelectedInteractionChangedEventArgs> OnSelectedInteractionChanged;
	public class OnSelectedInteractionChangedEventArgs : EventArgs
	{
		public NPC selectedInteraction;
	}

	[SerializeField] private float moveSpeed = 7f;
	[SerializeField] private GameInput gameInput;
	[SerializeField] private LayerMask npcLayerMask;

	private bool isWalking;
	private Vector3 lastInteractDir;

	private NPC selectedInteraction; // NPC to interact

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("There is more than one Player instance");
		}
		Instance = this;
	}

	private void Start()
	{
		gameInput.OnInteractAction += GameInput_OnInteractAction;
	}

	private void GameInput_OnInteractAction(object sender, System.EventArgs e)
	{
		if (selectedInteraction != null) // Check if NPC is not null
		{
			selectedInteraction.Interact(); // Interact with NPC, Interact() is from NPC class,
		}
	}

	private void Update()
	{
		HandleMovement();
		HandleInteractions();
	}

	public bool IsWalking()
	{
		return isWalking;
	}

	private void HandleInteractions()
	{
		Vector2 inputVector = gameInput.GetMovementVectorNormalized();

		Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

		if (moveDir != Vector3.zero)
		{
			lastInteractDir = moveDir;
		}

		float interactDistance = 2f;
		// Check if the player is moving or if the last interact direction is non-zero
		if (isWalking || lastInteractDir != Vector3.zero)
		{
			if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, npcLayerMask))
			{
				if (raycastHit.transform.TryGetComponent(out NPC npc))
				{
					// is NPC
					if (npc != selectedInteraction)
					{
						SetSelectedInteraction(npc);
					}
				}
				else
				{
					SetSelectedInteraction(null);
				}
			}
			else
			{
				SetSelectedInteraction(null);
			}
		}
		else
		{
			SetSelectedInteraction(null);
		}

		//Debug.Log(selectedInteraction);
	}

	private void HandleMovement()
	{
		Vector2 inputVector = gameInput.GetMovementVectorNormalized();
		Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
		float moveDistance = moveSpeed * Time.deltaTime;
		float playerRadius = .7f;
		float playerHeight = 2f;
		bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

		if (!canMove)
		{
			// Cannot move towards moveDir

			// Attempt only X movement
			Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
			canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

			if (canMove)
			{
				// Can move only on the x
				moveDir = moveDirX;
			}
			else
			{
				// Cannot move only on the x

				// Attempt only z movement
				Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
				canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

				if (canMove)
				{
					// Can move only on the z
					moveDir = moveDirZ;
				}
				else
				{
					// Cannot move in any direction
				}
			}
		}

		if (canMove)
		{
			transform.position += moveDir * moveDistance;
		}

		isWalking = moveDir != Vector3.zero;

		float rotateSpeed = 10f;
		transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
	}

	private void SetSelectedInteraction(NPC selectedInteraction)
	{
		this.selectedInteraction = selectedInteraction;

		OnSelectedInteractionChanged?.Invoke(this, new OnSelectedInteractionChangedEventArgs
		{
			selectedInteraction = selectedInteraction
		}); ;

	}
}