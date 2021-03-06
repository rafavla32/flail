﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUnityEvents;

public class Fish : MonoBehaviour
{
    public bool puddle, grounded; 
    public string puddleTag = "Puddle";
    public Rigidbody2D[] rbParts;
    [SerializeField] private int initialDashCount = 1;
    [SerializeField] private int initialBreath = 100;
    [SerializeField] private float breathRate = 0.1f;
    public FloatEvent onChangeBreath;
    public IntEvent onChangeDashCount;
    [SerializeField] private string splashSoundName = "splash";
    
    private float _breath;
    public float Breath {
        get => _breath; 
        set {
            if (Breath != value) {
                _breath = value;
                _breath = Mathf.Clamp (_breath, 0, initialBreath);
                onChangeBreath.Invoke (value);
            }
        }
    }

    private int _dashs;
    public int Dashs {
        get => _dashs; 
        set {
            if (Dashs != value) {
                _dashs = value;
                onChangeDashCount.Invoke (value);
            }

        }
    }

    private bool dying;
    private GameManager gm;
    private AudioManager am;

    private void Start() {
        gm = GameManager.Instance;
        am = AudioManager.Instance;

        Dashs = initialDashCount;
        Breath = initialBreath;


        rbParts = GetComponentsInChildren<Rigidbody2D> ();
        onChangeDashCount.Invoke (Dashs);
    }

    private void Update() {

        if (!puddle) {
            Breath -= breathRate * Time.deltaTime;
        } else {
            Breath += breathRate * Time.deltaTime;
        }

        if (Breath <= 0)
        {
            gm.LoseGame();
            gameObject.SetActive(false);
        }
    }

    public void UseDash () {
        if (Dashs > 0) Dashs --;
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag (puddleTag)) {
            Dashs++;
            puddle = true;
            am.PlaySound (splashSoundName);
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag (puddleTag)) puddle = false;
    }
}
