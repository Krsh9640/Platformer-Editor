using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public int enemyHP = 1;
    private int currentHP;

    private AudioSource audioSource;
    public AudioClip deathSound;

    private FrogBehaviour frogBehaviour;

    void Start()
    {
        currentHP = enemyHP;

        audioSource = GameObject.Find("Manager").GetComponent<AudioSource>();
        frogBehaviour = this.gameObject.GetComponent<FrogBehaviour>();
    }

    void Update()
    {
        if (currentHP <= 0)
        {
            if (this.gameObject.name == "Frog(Clone)" && frogBehaviour != null)
            {
                frogBehaviour.ChangeAnimation(FrogBehaviour.Animations.Death);

                Destroy(this.gameObject);
                audioSource.PlayOneShot(deathSound, 0.5f);
            }
            else if (this.gameObject.name == "Mushroom(Clone)")
            {
                Animator anim = this.gameObject.GetComponent<Animator>();
                anim.SetBool("isDead", true);

                Destroy(this.gameObject);
                audioSource.PlayOneShot(deathSound, 0.5f);
            }
            else if (this.gameObject.name == "Bee(Clone)")
            {
                Animator anim = this.gameObject.GetComponent<Animator>();
                anim.SetBool("isDead", true);

                Destroy(this.gameObject);
                audioSource.PlayOneShot(deathSound, 0.5f);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
    }


}
