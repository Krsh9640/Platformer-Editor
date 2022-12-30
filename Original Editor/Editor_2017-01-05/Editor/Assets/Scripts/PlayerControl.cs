using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

    bool active;
    bool conveyorDirectionFwdExt;
    bool turnMessage;
    float jumpMovementX, jumpMovementY;
    float motionProgress;
    int abyssDetection;
    int action, actionBefore, actionExt;                            // 0 = Idle; 1 = Falling; 2 = Walking; 3 = Climbing; 4 = Jumping; 5 = Driving; 15 = Dying
    int dieProgress;
    public int directionX, directionY;
    int directionXExt, directionYExt;
    int lastDirectionX, lastDirectionY;
    int gravity;
	int jumpConditionRight, jumpConditionLeft;
    int jumpProgress;
    int noCollision;
    int noLadderBehind, noLadderBelow;
    int remainingTime;
    int startMode;
    int walkingSpeed;
	int tileSize;
	Animator animator;
    General general;
    PlayerControl playerControl;
    ConveyorControl conveyorControl;
	SpriteRenderer spriteRenderer;
	Vector3 playerPosition;
        
    void Fall()
    {
        transform.Translate(Vector3.down * Time.deltaTime * gravity);
        if (transform.localPosition.y < -2000)
        {
            Destroy(gameObject);
        }
    }

    void Walk()
    {
        transform.Translate(directionX * Time.deltaTime * walkingSpeed, 0, 0);
        motionProgress += Mathf.Abs(directionX) * Time.deltaTime * walkingSpeed;
        ResetMotionProgress();
    }

    void Climb()
    {
        transform.Translate(0, directionY * Time.deltaTime * walkingSpeed, 0);
        motionProgress += Mathf.Abs(directionY) * Time.deltaTime * walkingSpeed;
        ResetMotionProgress();

        if (noLadderBehind > 5 & noLadderBelow > 5 & motionProgress >= tileSize / 1.5)
        {
            Debug.Log("Fall!");
            action = 1;
            animator.Play("Falling");
        }
    }

    void Jump()
    {
        if (jumpProgress == 0)
        {
            jumpMovementX = 4;
            jumpMovementY = 6;
        }
        if (jumpProgress > 80)
        {
            jumpProgress = 0;
            action = 1;
        }
        else
        {
            transform.Translate(directionX * Time.deltaTime * walkingSpeed * jumpMovementX, 1 * Time.deltaTime * walkingSpeed * jumpMovementY, 0);
            jumpMovementX /= 1.04f;
            jumpMovementY /= 1.04f;
            jumpProgress++;
        }
    }

    void Drive()
    {
        transform.Translate(directionX * Time.deltaTime * (walkingSpeed / 2), 0, 0);
        motionProgress += Mathf.Abs(directionX) * Time.deltaTime * walkingSpeed;
        ResetMotionProgress();
    }

    void Die()
    {
        if (dieProgress > 50)
        {
            Destroy(gameObject);
        }
        else
        {
            dieProgress++;
        }
    }

    void PlayerTurnX()
    {
        directionX = -directionX;
        spriteRenderer.flipX = !spriteRenderer.flipX;
        motionProgress = tileSize - motionProgress;
    }

    void PlayerTurnY()
    {
        directionY = -directionY;
        motionProgress = tileSize - motionProgress;
    }

	// Use this for initialization

	void Start ()
	{
        general = GameObject.Find("MainCamera").GetComponent<General>();
        startMode = Game.startMode;
		tileSize = Editor.tileSize;
		walkingSpeed = Game.walkingSpeed;
		gravity = Game.gravity;
        remainingTime = Game.remainingTime;
        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        if (!spriteRenderer.flipX)
        {
            directionX = 1;
        }
        else
        {
            directionX = -1;
        }

		directionY = 1;

	}
	
	// Update is called once per frame

	void FixedUpdate ()
    {
        if (General.programState > 0)
        {
            if (active)
            {
                switch (action)
                {
                    case 1:
                        Fall();
                        break;
                    case 2:
                        Walk();
                        break;
                    case 3:
                        Climb();
                        break;
                    case 4:
                        Jump();
                        break;
                    case 5:
                        Drive();
                        break;
                    case 15:
                        Die();
                        break;
                }
                noCollision++;

                if (noCollision > 1 & action != 1 & action != 4)
                {
                    action = 1;
                    animator.Play("Falling");
                }

                // Spielzeit auf abgelaufen überprüfen

                if (action != 9 & Game.levelTime > 0 & Game.remainingTime == 0)
                {

                    // Sterben

                    action = 15;
                    animator.Play("Standing");
                }
            }
            else
            {
                // Automatischer Player Start

                if (transform.position.z != -10 & Game.startMode == 0)
                {
                    active = true;
                    action = 2;
                    animator.Play("Walking");
                }
            }
        }
    }

    void OnMouseDown()
    {
        if (General.programState > 0)
        {
            switch (action)
            {
                case 0:
                    if (startMode == -1)
                    {
                        active = true;
                        action = 2;
                        animator.Play("Walking");
                    }
                    break;
                case 2:
                    PlayerTurnX();
                    break;
                case 3:
                    PlayerTurnY();
                    break;
            }
        }
    }

	void OnTriggerStay2D(Collider2D collider)
	{
        if (General.programState > 0)
        {

            bool groundDetection = false;
            Vector3 position = collider.transform.position;
            playerPosition = transform.position;
    
            noCollision = 0;

            if (active & action > 0 & action < 15)
            {

                switch (collider.tag)
                {
                // Wenn das kollidierende Objekt vom Typ "Wall" ist

                    case "Wall":

				    // Wenn sich dieses Objekt unter dem Player befindet

                        if (playerPosition.y - position.y > 0 & playerPosition.y - position.y < (tileSize / 1.9))
                        {
                            if (playerPosition.x - position.x > -(tileSize / 1.1) & playerPosition.x - position.x < tileSize / 1.1)
                            {
                                groundDetection = true;
                                abyssDetection = 0;

                                if (action != 2)
                                {
                                    if (action == 1 | action == 3 & motionProgress > tileSize / 2 | action == 3 & noLadderBelow > 2 & directionY == -1)
                                    {

                                        // Laufen

                                        playerPosition.y = position.y + (tileSize / 2);
                                        transform.position = playerPosition;
                                        action = 2;
                                        motionProgress = 0;
                                        animator.Play("Walking");
                                        Debug.Log("Walk!");
                                    }
                                }
                            }
                        }

				    // Wenn Objekt in Hoehe des Players

                        if (playerPosition.y - position.y > -tileSize & playerPosition.y - position.y < 0)
                        {
                            // Wenn unmittelbar auf dessen rechten Seite

                            if (playerPosition.x - position.x > -tileSize & playerPosition.x - position.x < 0)
                            {
                                if (action == 2 & directionX == 1)
                                {
                                    directionX = -1;
                                    spriteRenderer.flipX = true;
                                }
                            }

                            // Wenn unmittelbar auf dessen linken Seite

                            if (playerPosition.x - position.x > 0 & playerPosition.x - position.x < tileSize)
                            {
                                if (action == 2 & directionX == -1)
                                {
                                    directionX = 1;
                                    spriteRenderer.flipX = false;
                                }
                            }

                            // Wenn einen halben Block rechts vom ihm

                            if (playerPosition.x - position.x > -tileSize * 1.5 & playerPosition.x - position.x < -(tileSize / 2))
                            {
                                if (action == 2 & directionX == 1)
                                {
                                    jumpConditionRight++;

                                    if (jumpConditionRight > 1)
                                    {

                                        // Springen

                                        jumpConditionRight = 0;
                                        action = 4;
                                        animator.Play("Jumping");
                                        Debug.Log("Jump!");
                                    }
                                }
                            }

                            // Wenn einen halben Block links vom ihm

                            if (playerPosition.x - position.x > tileSize / 2 & playerPosition.x - position.x < tileSize * 1.5)
                            {
                                if (action == 2 & directionX == -1)
                                {
                                    jumpConditionLeft++;

                                    if (jumpConditionLeft > 1)
                                    {

                                        // Springen

                                        jumpConditionLeft = 0;
                                        action = 4;
                                        animator.Play("Jumping");
                                        Debug.Log("Jump!");
                                    }
                                }
                            }
                        }

				    // Wenn Objekt einen Block hoeher

                        if (playerPosition.y - position.y > -(tileSize * 2) & playerPosition.y - position.y < -tileSize)
                        {

                            // Wenn einen halben rechts

                            if (playerPosition.x - position.x > -tileSize * 1.5 & playerPosition.x - position.x < -(tileSize / 2))
                            {
                                if (action == 2 & directionX == 1)
                                {
                                    jumpConditionRight = 0;
                                }
                            }

                            // Wenn einen halben links

                            if (playerPosition.x - position.x > tileSize / 2 & playerPosition.x - position.x < tileSize * 1.5)
                            {
                                if (action == 2 & directionX == -1)
                                {
                                    jumpConditionLeft = 0;
                                }
                            }
                        }

				    // Wenn kein Objekt vom Typ "Wall" unter Player

                        if (!groundDetection)
                        {
                            abyssDetection++;

                            if (action == 2 & abyssDetection > 5) // & action == 2)
                            {

                                // Fallen

                                action = 1;
                                animator.Play("Falling");
                                Debug.Log("Fall!");
                            }
                        }

                        break;

                //Wenn vom Typ "Ladder"

                    case "Ladder":
                    
                        if (playerPosition.x - position.x > -(tileSize / 4) & playerPosition.x - position.x < tileSize / 4)
                        {

                            // Wenn unter dem Player

                            if (playerPosition.y - position.y > 0 & playerPosition.y - position.y < (tileSize / 1.9))
                            {
                                noLadderBelow = 0;

                                if (action != 3 & motionProgress > tileSize / 2 & noLadderBehind > 2)
                                {

                                    // Nach unten klettern

                                    playerPosition.x = position.x;
                                    transform.position = playerPosition;
                                    directionY = -1;
                                    action = 3;
                                    motionProgress = 0;
                                    animator.Play("Climbing");
                                    Debug.Log("Climb!");
                                }
                            }
                            else
                            {
                                noLadderBelow++;
                            }

                            // Wenn dahinter

                            if (playerPosition.y - position.y > -tileSize & playerPosition.y - position.y < 0)
                            {
                                noLadderBehind = 0;

                                if (action != 3 & motionProgress >= tileSize / 1.5)
                                {

                                    // Klettern

                                    playerPosition.x = position.x;
                                    transform.position = playerPosition;

                                    if (abyssDetection <= 5)
                                    {
                                        directionY = 1;
                                    }
                                    action = 3;
                                    motionProgress = 0;
                                    animator.Play("Climbing");
                                    Debug.Log("Climb!");
                                }
                            }
                            else
                            {
                                noLadderBehind++;
                            }
                        }

                        break;

                    case "Conveyor":

                        conveyorControl = collider.GetComponent<ConveyorControl>();
                        conveyorDirectionFwdExt = conveyorControl.conveyorDirectionFwd;
                    
                    // Wenn sich dieses Objekt unter dem Player befindet

                        if (playerPosition.y - position.y > 0 & playerPosition.y - position.y < (tileSize / 1.9))
                        {
                            if (playerPosition.x - position.x > -(tileSize / 1.1) & playerPosition.x - position.x < tileSize / 1.1)
                            {
                                groundDetection = true;
                                abyssDetection = 0;

                                if (action != 5 | directionX == 1 & !conveyorDirectionFwdExt | directionX == -1 & conveyorDirectionFwdExt)
                                {

                                    // Fahren

                                    playerPosition.y = position.y + (tileSize / 2);
                                    transform.position = playerPosition;
                                    if (conveyorDirectionFwdExt)
                                    {
                                        directionX = 1;
                                        spriteRenderer.flipX = false;
                                    }
                                    else
                                    {
                                        directionX = -1;
                                        spriteRenderer.flipX = true;
                                    }
                                    action = 5;
                                    motionProgress = 0;
                                    animator.Play("DriveR");
                                    Debug.Log("Drive!");
                                }
                            }
                        }

                        break;

                // Wenn Typ "Spikes"

                    case "Spikes":

                    // Wenn dahinter

                        if (playerPosition.y - position.y > -tileSize & playerPosition.y - position.y < 0)
                        {
                            if (playerPosition.x - position.x > -tileSize / 2 & playerPosition.x - position.x < tileSize / 2)
                            {

                                // Sterben

                                playerPosition.y = position.y + (tileSize / 2);
                                action = 15;
                                animator.Play("Standing");
                                Debug.Log("Die!");
                            }
                        }

                        break;

                // Wenn Typ "Player"

                    case "Player":

                        playerControl = collider.GetComponent<PlayerControl>();
                        actionExt = playerControl.action;
                        directionXExt = playerControl.directionX;
                        directionYExt = playerControl.directionY;

                    // Wenn in Hoehe des Players

                        if (playerPosition.y - position.y > -tileSize / 2 & playerPosition.y - position.y < tileSize / 2)
                        {

                            // Wenn unmittelbar auf dessen rechten Seite

                            if (playerPosition.x - position.x > -tileSize & playerPosition.x - position.x < 0)
                            {
                                if (action == 2 & actionExt == 2 & directionX == 1)// & directionXExt == -1)
                                {
                                    PlayerTurnX();
                                }
                            }

                            // Wenn unmittelbar auf dessen linken Seite

                            if (playerPosition.x - position.x > 0 & playerPosition.x - position.x < tileSize)
                            {
                                if (action == 2 & actionExt == 2 & directionX == -1)// & directionXExt == 1)
                                {
                                    PlayerTurnX();
                                }
                            }
                        }

                        if (playerPosition.x - position.x > -tileSize / 2 & playerPosition.x - position.x < tileSize / 2)
                        {

                            // Wenn unmittelbar über ihm

                            if (playerPosition.y - position.y > -tileSize * 1.5 & playerPosition.y - position.y < -tileSize)
                            {
                                if (action == 3 & actionExt == 3 & directionY == 1)
                                {
                                    PlayerTurnY();
                                }
                            }

                            // Wenn unmittelbar unter ihm

                            if (playerPosition.y - position.y > tileSize & playerPosition.y - position.y < tileSize * 1.5)
                            {
                                if (action == 3 & actionExt == 3 & directionY == -1)
                                {
                                    PlayerTurnY();
                                }
                            }
                        }

                        break;

                }
            }
        }
	}

    void ResetMotionProgress()
    {
        if (motionProgress >= tileSize)
        {
            motionProgress -= tileSize;   
        }
    }
}

