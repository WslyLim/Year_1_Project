using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour{

	private FirstPersonController firstPersonController;
	private float footstepTimer;
	private float footstepTimerMax = .35f;

	private void Awake()
	{
        firstPersonController = FindObjectOfType<FirstPersonController>();
	}

	private void Update()
	{
		footstepTimer -= Time.deltaTime;
		if (footstepTimer < 0f)
		{
			footstepTimer = footstepTimerMax;

			if (firstPersonController.IsWalking())
			{
				float volume = 1f;
				SoundManager.Instance.PlayFootstepsSound(firstPersonController.transform.position, volume);
			}
		}
	}
}

