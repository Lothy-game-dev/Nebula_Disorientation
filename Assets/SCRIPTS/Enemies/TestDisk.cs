using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDisk : EnemyShared
{
    public Rigidbody2D rb;
    public float changeDirTimer;
    public GameObject EnemyStatus;
    private StatusBoard Status;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        MaxHP = 100f;
        changeDirTimer = 2.5f;
        rb.velocity = new Vector2(0, 30);
        Status = EnemyStatus.GetComponent<StatusBoard>();
        InitializeFighter();
    }

    // Update is called once per frame
    void Update()
    {
        CheckThermal();
        SetHealth();
        if (changeDirTimer > 0f)
        {
            changeDirTimer -= Time.deltaTime;
        } else
        {
            changeDirTimer = 5f;
            if (rb.velocity!=new Vector2(0,0))
            {
                rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
            } else
            {
                rb.velocity = new Vector2(rb.velocity.x, 30);
            }
            
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
