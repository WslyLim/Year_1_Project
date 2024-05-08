using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbCollider : MonoBehaviour
{
    public GameObject HitPrefab;
    private PlayerController player;

    void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<HealthSystem>().Damage(player.GetPlayerDamage()*5);
        }
    }
   
    void Explode()
    {
        GameObject HitEffect = Instantiate(HitPrefab, this.transform.position,Quaternion.identity);
        Destroy(this.gameObject);
        Destroy(HitEffect,3f);
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(10f);
        Destroy(this.gameObject);

    }

}
