using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDisk : EnemyShared
{
    public Rigidbody2D rb;
    public float changeDirTimer;
    private bool test;
    private Vector2 check;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        changeDirTimer = 2.5f;
        test = false;
        check = new Vector2(0, 30);
        StartEnemy(100f);
        CalculateVelocity(!test ? check : -check);
        InitializeFighter();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEnemy();
        CalculateVelocity(!test ? check : -check);
        if (changeDirTimer > 0f)
        {
            changeDirTimer -= Time.deltaTime;
        } else
        {
            changeDirTimer = 5f;
            test = !test;
        }
    }
}
