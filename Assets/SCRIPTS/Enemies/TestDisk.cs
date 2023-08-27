using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDisk : EnemyShared
{
    public Rigidbody2D rb;
    private float changeDirTimer;
    public GameObject EnemyStatus;
    private StatusBoard Status;
    private SpriteRenderer EnemySprite;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        MaxHP = 100f;
        CurrentHP = MaxHP;
        changeDirTimer = 2.5f;
        rb.velocity = new Vector2(0, 30);
        Status = EnemyStatus.GetComponent<StatusBoard>();
        EnemySprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
        SetHealth();
        if (changeDirTimer > 0f)
        {
            changeDirTimer -= Time.deltaTime;
        } else
        {
            changeDirTimer = 5f;
            rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
        }
        if (CurrentHP<=0f)
        {
            Destroy(gameObject);
        }

        if (Status.Timer > 0f)
        {
            Status.Timer -= Time.deltaTime;
        } else
        {
            if (!Status.isShow)
            {
                EnemyStatus.SetActive(false);
                Status.DeleteClone(true);
            }
        }
      
    }

    private void OnMouseEnter()
    {
        Status.isShow = true;
        EnemyStatus.SetActive(true);
        Status.ShowStatus(gameObject);       
    }
    private void OnMouseExit()
    {
        Status.isShow = false;
        Status.Timer = 10f;
    }
    
}
