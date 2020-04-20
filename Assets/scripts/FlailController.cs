﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUnityEvents;

[RequireComponent(typeof (Rigidbody2D), typeof (Fish))]
public class FlailController : MonoBehaviour
{
#region VARIABLES
    [Header ("Force and Torque values")]
    
    [Tooltip ("The force that will be added to the fish in the direction of the puddle")] 
    [SerializeField] private float flailForce = 600;
    
    [Tooltip ("The torque that will be added to the other body parts to make it look like the fish is flailing (shouldn't be a big value, or else it will add more movement to the fish)")]
    [SerializeField] private float flailTorque = 150;
    
    [Space, Header ("Flail increment parameters")]
    [Tooltip ("The time it takes for the flail to reset")]
    [SerializeField] private float resetTimer = 5f; 
    [Tooltip ("The amount that each key press adds to the flail amount which goes from 0 (no movement) to 1 (full movement)")]
    [Range (0,1), SerializeField] private float flailIncrement = 0.1f; 
    [Space, Header ("Other parameters")]
    [Tooltip("The exact name of the puddle tag")]
    [SerializeField] private string puddleTag;

    [Tooltip ("The noise in the flail x direction, no noise if x and y are the same")]
    [SerializeField] private Vector2 minMaxX;

    [Tooltip ("The noise in the flail x direction, no noise if x and y are the same")]
    [SerializeField] private Vector2 minMaxY;

    [SerializeField] private FloatEvent onChangeFlailAmount;

    private float flailAmount;
    public float FlailAmount {
        get => flailAmount;
        set {
            if (FlailAmount != value) {
                flailAmount = value;
                onChangeFlailAmount.Invoke (value);
            }
        }
    }

    private GameManager gm;
    private Fish fish;
    private Vector2 closestPuddle;
    private Rigidbody2D rb;
    private Rigidbody2D[] bodyParts;
    private Vector2 flailDir;
    private GameObject[] puddles;
    private bool flail, grounded;
    private float timer;
#endregion

    void Start()
    {
        gm = GameManager.Instance;
        fish = GetComponent<Fish> ();
        rb = GetComponent<Rigidbody2D> ();
        bodyParts = fish.rbParts;
        puddles = GameObject.FindGameObjectsWithTag(fish.puddleTag);
    }
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= resetTimer) FlailAmount = 0;

        if (Input.GetKeyDown (KeyCode.Space) && fish.grounded) {
            timer = 0;
            FlailAmount += flailIncrement;
            FlailAmount = Mathf.Clamp01(FlailAmount);

            closestPuddle = GetClosestPuddle ();
            flailDir = new Vector2 (
                Mathf.Sign (closestPuddle.x - transform.position.x) * Random.Range (minMaxX.x,minMaxX.y),
                Random.Range (minMaxY.x, minMaxY.y)
            );
            
            flail = true;
        } else {
            flail = false;
        }
    }

    private void FixedUpdate() {
        if (flail) {
            rb.AddForce (flailDir * flailForce * FlailAmount);
            for (int i = 0; i < bodyParts.Length; i++)
            {
                bodyParts[i].AddTorque (Random.Range (-flailTorque, flailTorque));
            }
        }
    }

    private Vector2 GetClosestPuddle () {
        Vector2 closestPuddle = Vector2.zero;
        for (int i = 0; i < puddles.Length; i++)
        {   
            var currentPuddle = puddles[i];
            if (i == 0) {
                closestPuddle = currentPuddle.transform.position;
            } else {
                float currentDistance = Vector2.Distance (transform.position, closestPuddle);
                float newDistance = Vector2.Distance (transform.position, currentPuddle.transform.position);
                if (newDistance < currentDistance) closestPuddle = currentPuddle.transform.position;
            }
        }
        return closestPuddle;
    }
}
