using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomBehaviour : MonoBehaviour
{
   public float walkSpeed;

   public bool mustPatrol = true;
   public bool mustFlip;
   public Rigidbody2D rb;
   public Transform groundDetection;
   public LayerMask platformLayer;

   bool isGrounded = false;

   public Collider2D bodyCollider;

   public Animator animator;
   private Collider2D mushroomCld, beeCld, frogCld;

   void Start() {
       mustPatrol = true; 
   }

   void Update(){
       if(mustPatrol == true && isGrounded == true){
           Patrol();
       }
   }

   private void FixedUpdate() {
       if(mustPatrol == true){
           mustFlip = !Physics2D.OverlapCircle(groundDetection.position, 0.1f, platformLayer);
       }
   }

    private void LateUpdate() {
        if(isGrounded == true){
            animator.SetFloat("Speed", rb.velocity.magnitude);
        }
   }

   void Patrol(){
       rb.velocity = new Vector2(walkSpeed * Time.fixedDeltaTime, rb. velocity.y);
       if(mustFlip == true || bodyCollider.IsTouchingLayers(platformLayer)){
           Flip();
       }
   }

   void Flip(){
       mustPatrol = false;
       transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
       walkSpeed *= -1;
       mustPatrol = true;
       mustFlip = false;
   }

   private void OnCollisionEnter2D(Collision2D collision) {
       if(collision.gameObject.tag == "Enemy" && collision != null){
           Flip();
       }

       if(collision.collider.gameObject.layer == LayerMask.NameToLayer("Platform")){
            isGrounded = true;
        }
   }
}
