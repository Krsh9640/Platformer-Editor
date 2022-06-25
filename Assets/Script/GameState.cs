using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour
{
    public GameObject character;
    public GameObject tileTabs;
    public GameObject coinCounter, timeCounter;
    public GameObject stopButton, pauseButton;
    private GameObject playButton;

    private CharacterController2D controller;
    private PlayerMovement movement;
    private Animator playerAnimator;
    
    public GameObject[] enemies;

    private bool isPaused;
    public bool isPlay = false;

    private void Awake() {
        controller = character.GetComponent<CharacterController2D>();
        movement = character.GetComponent<PlayerMovement>();
        playerAnimator = character.GetComponent<Animator>();

        playButton = this.gameObject;
    }
    
    void Start() {    
        Button stpBtn = stopButton.GetComponent<Button>();
        Button pauBtn = pauseButton.GetComponent<Button>();

        stpBtn.onClick.AddListener(StopPlaying);
        pauBtn.onClick.AddListener(PauseGame);
    }

    void Update() {
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    public void BtnPlay()
    {
        isPlay = true;

        GetComponentEnemy(true);

        controller.enabled = true;
        movement.enabled = true;
        playerAnimator.enabled = true;
        
        tileTabs.SetActive(false);
        coinCounter.SetActive(true);
        timeCounter.SetActive(true);
        pauseButton.SetActive(true);
        stopButton.SetActive(true);
        playButton.SetActive(false);
    }

    public void PauseGame(){
        Time.timeScale = 0f;
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void StopPlaying()
    {   
        isPlay = false;

        GetComponentEnemy(false);
        
        controller.enabled = false;
        movement.enabled = false;
        playerAnimator.enabled = false;

        tileTabs.SetActive(true);
        coinCounter.SetActive(false);
        timeCounter.SetActive(false);
        pauseButton.SetActive(false);
        playButton.SetActive(true);
        stopButton.SetActive(false);
    }

    public void GetComponentEnemy(bool isEnabled){
        foreach(GameObject i in enemies){
            if(i != null){
                if(i.TryGetComponent(out MushroomBehaviour mushroom)){
                    mushroom.enabled = isEnabled;
                    i.GetComponent<Animator>().enabled = isEnabled;
                    if(isEnabled == true){
                        i.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                    } else {
                        i.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                    }
                } else if(i.TryGetComponent(out FrogBehaviour frog)){
                    frog.enabled = isEnabled;
                    i.GetComponent<Animator>().enabled = isEnabled;
                    if(isEnabled == true){
                        i.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                    } else {
                        i.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                    }
                } else if(i.TryGetComponent(out Animator animator)){ 
                     animator.enabled = isEnabled;                
                }
            }
        }
    }   
}
