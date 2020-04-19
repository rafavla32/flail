﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider breathSlider;
    [SerializeField] private GameObject endGameUI;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject[] dashIcons;
    [SerializeField] private TMP_Text timerText;
    private GameManager gm;
    private bool paused;

    private void Start() {
        gm = GameManager.Instance;
        gm.onEndGame.AddListener(EndGame);

        if (endGameUI != null) endGameUI.SetActive (false);
        if (pauseUI != null) pauseUI.SetActive (false);
        if (breathSlider != null) gm.currentFish.onChangeBreath.AddListener (BreathValue);
        if (dashIcons != null) gm.currentFish.onChangeDashCount.AddListener (DashCount);
        
    }
    private void Update() {
        timerText.text = Time.time.ToString ("0:00");
        if (Input.GetKeyDown (KeyCode.Escape)) Pause ();
    }

    private void EndGame () {
        if (endGameUI != null)
            endGameUI.SetActive (true);
    }
    private void BreathValue (float breath) {
        breathSlider.value = breath/100;
    }
    private void DashCount (int dashCount) {
        for (int i = 0; i < dashIcons.Length; i++)
        {
            GameObject currentIcon = dashIcons[i];
            if (i <= dashCount - 1) currentIcon.SetActive (true);
            else currentIcon.SetActive (false);
        }
    }
    public void Pause () {
        if (paused) {
            Time.timeScale = 1;
            paused = false;
            if (pauseUI != null) pauseUI.SetActive (false);
        } else {
            Time.timeScale = 0;
            paused = true;
            if (pauseUI != null) pauseUI.SetActive (true);
        }

        

    }
}
