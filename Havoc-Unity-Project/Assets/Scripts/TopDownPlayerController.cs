﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownPlayerController : MonoBehaviour
{
    private Rigidbody2D rb;    
    private Animator anim;

    [SerializeField] private float inputWalkingSpeed;
    private float walkingSpeed;
    private float heading;
    private bool isCombatMode;

    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        anim = this.GetComponent<Animator>();
        isCombatMode = false;

        heading = 270;
        walkingSpeed = inputWalkingSpeed;
    }

    void FixedUpdate()
    {
        this.ToggleCombatMode();

        if (isCombatMode)
        {
            this.CombatModeAnimationManager();
        }
        this.MovementManager(isCombatMode);
    }

    private bool ToggleCombatMode()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isCombatMode = !(isCombatMode);
        }

        return isCombatMode;
    }

    private void CombatModeAnimationManager()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        heading = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        if (heading > 0 - 22.5 && heading <= 0 + 22.5)
        {
            anim.SetFloat("xPos", 1);
            anim.SetFloat("yPos", 0);
        }
        else if (heading > 45 - 22.5 && heading <= 45 + 22.5)
        {
            anim.SetFloat("xPos", 1);
            anim.SetFloat("yPos", 1);
        }
        else if (heading > 90 - 22.5 && heading <= 90 + 22.5)
        {
            anim.SetFloat("xPos", 0);
            anim.SetFloat("yPos", 1);
        }
        else if (heading > 135 - 22.5 && heading <= 135 + 22.5)
        {
            anim.SetFloat("xPos", -1);
            anim.SetFloat("yPos", 1);
        }
        else if (heading > 180 - 22.5 && heading <= 180 + 22.5)
        {
            anim.SetFloat("xPos", -1);
            anim.SetFloat("yPos", 0);
        }
        else if (heading > -45 - 22.5 && heading <= -45 + 22.5)
        {
            anim.SetFloat("xPos", 1);
            anim.SetFloat("yPos", -1);
        }
        else if (heading > -90 - 22.5 && heading <= -90 + 22.5)
        {
            anim.SetFloat("xPos", 0);
            anim.SetFloat("yPos", -1);
        }
        else if (heading > -135 - 22.5 && heading <= -135 + 22.5)
        {
            anim.SetFloat("xPos", -1);
            anim.SetFloat("yPos", -1);
        }
    }

    private void WalkingModeAnimationManager(float input_x, float input_y)
    {
        anim.SetFloat("xPos", input_x);
        anim.SetFloat("yPos", input_y);
    }
    
    private void MovementManager(bool isCombatMode)
    {
        float input_x = Input.GetAxisRaw("Horizontal");
        float input_y = Input.GetAxisRaw("Vertical");

        bool isWalking = (Mathf.Abs(input_x) + Mathf.Abs(input_y)) > 0;

        if (isWalking)
        {
            // When we come back to sprite layer renderding, make it for the parent!!!
            this.transform.Translate(new Vector3(input_x, input_y, 0).normalized * walkingSpeed * Time.deltaTime);

            if (!isCombatMode)
            {
                this.WalkingModeAnimationManager(input_x, input_y);
            }
        }
    }
}
