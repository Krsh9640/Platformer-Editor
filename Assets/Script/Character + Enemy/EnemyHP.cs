using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public int enemyHP = 1;
    private int currentHP;

    private GameObject Manager;

    private AudioSource audioSource;
    public AudioClip deathSound;
    
    // Start is called before the first frame update
    void Start()
    {
        currentHP = enemyHP;

        Manager = GameObject.Find("Manager");
        audioSource = Manager.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHP <= 0){
            Destroy(this.gameObject);
            audioSource.PlayOneShot(deathSound, 0.5f);
        }
    }

    public void TakeDamage(int damage){
        currentHP -= damage;
    }
}
