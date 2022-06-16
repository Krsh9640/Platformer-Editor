using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStomp : MonoBehaviour
{
    private int damageToDeal = 1;

    private Rigidbody2D rb;

    private float bounceForce = 10f;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = transform.parent.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Enemy"){
            other.gameObject.GetComponent<EnemyHP>().TakeDamage(damageToDeal);
            rb.AddForce(transform.up * bounceForce, ForceMode2D.Impulse);
        }
    }
}
