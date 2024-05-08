using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedInteractionVisual : MonoBehaviour
{
    [SerializeField] private NPC npc;
    [SerializeField] private GameObject visualGameObject;

    private void Start()
    {
        PlayerController.Instance.OnSelectedInteractionChanged += Player_OnSelectedInteractionChanged;
        Hide(); // Hide the visual initially
    }

    private void Player_OnSelectedInteractionChanged(object sender, PlayerController.OnSelectedInteractionChangedEventArgs e)
    {
        if (e.selectedInteraction == npc)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        visualGameObject.SetActive(true);
    }

    private void Hide()
    {
        visualGameObject.SetActive(false);
    }
}
