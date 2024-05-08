using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : MonoBehaviour
{
    private HealthSystem healthSystem;
    public Animator animator;
    private PlayerController playerManager;
    public TriggerBox triggerBox;

    public bool IsHit = false;
    private void Start()
    {
        playerManager = FindObjectOfType<PlayerController>();
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.SetMaxHealth(100);
        healthSystem.SetHealth(100);
    }

    public void takeDamage(float damage)
    {
        healthSystem.Damage(damage);
        if (healthSystem.GetHealth() <= 0)
        {
            AudioManager.instance.Play("GolemDeath");
            animator.SetTrigger("die");
            GetComponent<Collider>().enabled = false;
            playerManager.levelSystem.AddExperience(500);
            triggerBox.enemiesLeft--;
            Destroy(gameObject, 2f);
        }
        else
        {
            AudioManager.instance.Play("GolemDamage");
            animator.SetTrigger("damage");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerSpells")
        {
            takeDamage(50);
            Debug.Log("Golem taken dmg");
        }
        
        if (other.gameObject.tag == "Weapon" && !IsHit)
        {
            IsHit = true;
            takeDamage(playerManager.GetPlayerDamage());
            //healthSystem.Damage(playerManager.GetPlayerDamage());
            Vector3 random = new Vector3(Random.Range(0f, 0.5f), Random.Range(0f, 0.5f), Random.Range(0f, 0.5f));
            DamagePopUpGenerator.current.CreatePopUp(transform.position + random, playerManager.GetPlayerDamage().ToString());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Weapon" && IsHit)
        {
            IsHit = false;
        }
    }
}
