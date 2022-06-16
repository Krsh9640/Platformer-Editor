using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour
{
    public GameObject character;
    public GameObject tileTabs;
    public GameObject coinCounter;
    public GameObject stopButton, pauseButton;
    private GameObject playButton;

    private CharacterController2D controller;
    private PlayerMovement movement;
    private Animator playerAnimator;
    
    public GameObject[] enemies;
    
    private MushroomBehaviour mushroomBehaviour;
    private Animator mushroomAnimator;
    private Animator beeAnimator;
    private bool isPaused;

    private void Awake() {
        controller = character.GetComponent<CharacterController2D>();
        movement = character.GetComponent<PlayerMovement>();
        playerAnimator = character.GetComponent<Animator>();

        playButton = this.gameObject;
    }
    
    void Start() {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject i in enemies){
            if(enemies != null){
                mushroomAnimator = enemies[0].gameObject.GetComponent<Animator>();
                mushroomBehaviour = enemies[0].gameObject.GetComponent<MushroomBehaviour>();            
                beeAnimator = enemies[1].gameObject.GetComponent<Animator>();
            }
        }
        
        Button stpBtn = stopButton.GetComponent<Button>();
        Button pauBtn = pauseButton.GetComponent<Button>();

        stpBtn.onClick.AddListener(StopPlaying);
        pauBtn.onClick.AddListener(PauseGame);
    }
    
    public void BtnPlay()
    {
        controller.enabled = true;
        movement.enabled = true;
        playerAnimator.enabled = true;
        
        if(mushroomAnimator != null){
            mushroomAnimator.enabled= true;
        }
        
        if(mushroomBehaviour != null){
            mushroomBehaviour.enabled = true;
        }
               
        if(beeAnimator != null){
            beeAnimator.enabled= true;
        }
        
        tileTabs.SetActive(false);
        coinCounter.SetActive(true);
        pauseButton.SetActive(true);
        stopButton.SetActive(true);
        playButton.SetActive(false);
    }

    public void PauseGame(){
        Time.timeScale = 0f;
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void StopPlaying(){
        controller.enabled = false;
        movement.enabled = false;
        playerAnimator.enabled = false;

        if(mushroomAnimator != null){
                mushroomAnimator.enabled= false;
        }

        if(mushroomBehaviour != null){
                mushroomBehaviour.enabled = false;
        }

        if(beeAnimator != null){
                beeAnimator.enabled= false;
        }

        tileTabs.SetActive(true);
        coinCounter.SetActive(false);
        pauseButton.SetActive(false);
        playButton.SetActive(true);
        stopButton.SetActive(false);
    }
}
