using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDisk : EnemyShared
{
    public Rigidbody2D rb;
    public float changeDirTimer;
    public GameObject EnemyStatus;
    private StatusBoard Status;
    private bool test;
    private Vector2 check;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        MaxHP = 100f;
        changeDirTimer = 2.5f;
        test = false;
        check = new Vector2(0, 30);
        CalculateVelocity(!test ? check : -check);
        Status = EnemyStatus.GetComponent<StatusBoard>();
        InitializeFighter();
    }

    // Update is called once per frame
    void Update()
    {
        CheckThermal();
        CheckInsideBlackhole();
        CalculateVelocity(!test ? check : -check);
        SetHealth();
        if (changeDirTimer > 0f)
        {
            changeDirTimer -= Time.deltaTime;
        } else
        {
            changeDirTimer = 5f;
            test = !test;
        }
        if (CurrentHP<=0f)
        {
            Status.StopShowing();
            Destroy(gameObject);
        }      
      
    }

    private void OnMouseOver()
    {
        Status.Timer = 5f;
        Status.StartShowing(gameObject);
    }

    private void OnMouseExit()
    {
        Status.CheckOnDestroy();
    }
    
}
