using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : Powers
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject ChargingEffect;
    public GameObject Effect;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public GameObject LeftWeapon;
    public GameObject RightWeapon;
    public bool isFire;
    private float DurationTimer;
    
    private bool isStart;
    private GameObject CharingClone;
    private GameObject CharingClone2;
    private Vector2 Spd;
    private float BeamTimer;
    public bool onHit;
    private float resetHitTimer;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        Spd = FindAnyObjectByType<FighterController>().PlayerFighter.GetComponent<Rigidbody2D>().velocity;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (isStart)
        {
            ChangeAnimationPos();
        }
        resetHitTimer -= Time.deltaTime;
        if (resetHitTimer <= 0)
        {
            onHit = false;
            resetHitTimer = 0.1f;
        }
    }
    private void FixedUpdate()
    {
        BeamTimer -= Time.fixedDeltaTime;
        if (isFire)
        {
            if (BeamTimer <= 0)
            {
                BeamTimer = 1 / 60f;
                GenerateLaserBeam();
            }
            if (DurationTimer == 0)
            {
                LaserBeamSound();
            }
            DurationTimer += Time.fixedDeltaTime;

            if (DurationTimer >= Duration)
            {
                isFire = false;
                DurationTimer = 0f;
                EndSound();
                FindAnyObjectByType<FighterController>().PlayerFighter.GetComponent<Rigidbody2D>().velocity = Spd;
                FindAnyObjectByType<FighterController>().PlayerFighter.GetComponent<PlayerMovement>().ExteriorROTSpeed = 1;
            }
        }
        
    }
    #endregion
    #region Generate bullet
    // Group all function that serve the same algorithm
    public void GenerateLaserBeam()
    {
        Vector2 pos = CalculatePos(Range);
        LeftWeapon = FindAnyObjectByType<FighterController>().LeftWeaponPosition;
        RightWeapon = FindAnyObjectByType<FighterController>().RightWeaponPosition;
        int n = 10;
        int m = 4;
        for (int i = 0; i < n; i++)
        {

            //Generate
            Vector2 posBullet = new Vector2(LeftWeapon.transform.position.x, LeftWeapon.transform.position.y) + (n/2 + 1 - i)*pos/(Range*0.15f);
            Vector2 posBullet2 = new Vector2(RightWeapon.transform.position.x, RightWeapon.transform.position.y) + (n / 2 + 1 - i) * pos / (Range * 0.15f);
            GameObject game = Instantiate(Effect, new Vector3(posBullet.x, posBullet.y, LeftWeapon.transform.position.z), Quaternion.identity);
            GameObject game2 = Instantiate(Effect, new Vector3(posBullet2.x, posBullet2.y, RightWeapon.transform.position.z), Quaternion.identity);
            game.GetComponent<Beam>().Distance = Range;
            game.GetComponent<Beam>().Damage = DPH;
            game.GetComponent<Beam>().Layer = EnemyLayer;

            //Change the spirte of the bullet like a laser
            game.transform.localScale = game.transform.localScale * (i > m ? (float)(n-i)/(n-m) : 1);
            if (i>m)
            {
                Color c = game.GetComponent<SpriteRenderer>().color;
                c.r = (i > m ? (float)(n - i) / (n - m) : 1);
                c.a = (i > m ? (float)(n - i) / (n - m) : 1);
                game.GetComponent<SpriteRenderer>().color = c;
                Color c2 = game.transform.GetChild(2).GetComponent<SpriteRenderer>().color;
                c2.a = (i > m ? (float)(n - i) / (n - m) : 1);
                game.transform.GetChild(2).GetComponent<SpriteRenderer>().color = c2;
            }
            game.GetComponent<Beam>().Laser = this;
            game2.GetComponent<Beam>().Distance = Range;
            game2.GetComponent<Beam>().Damage = DPH;
            game2.GetComponent<Beam>().Layer = EnemyLayer;

            //Change the spirte of the bullet like a laser
            game2.transform.localScale = game2.transform.localScale * (i > m ? (float)(n - i) / (n - m) : 1);
            game2.GetComponent<Beam>().Laser = this;
            if (i >m)
            {
                Color c = game2.GetComponent<SpriteRenderer>().color;
                c.r = (i > m ? (float)(n - i) / (n - m) : 1);
                c.a = (i > m ? (float)(n - i) / (n - m) : 1);
                game2.GetComponent<SpriteRenderer>().color = c;
                Color c2 = game2.transform.GetChild(2).GetComponent<SpriteRenderer>().color;
                c2.a = (i > m ? (float)(n - i) / (n - m) : 1);
                game2.transform.GetChild(2).GetComponent<SpriteRenderer>().color = c2;
            }
            game.SetActive(true);
            game2.SetActive(true);
            game.GetComponent<Rigidbody2D>().velocity = pos*2;
            game2.GetComponent<Rigidbody2D>().velocity = pos*2;


        }
        //slow down Fighter when firing
        FindAnyObjectByType<FighterController>().PlayerFighter.GetComponent<Rigidbody2D>().velocity *= 0.5f;
        FindAnyObjectByType<FighterController>().PlayerFighter.GetComponent<PlayerMovement>().ExteriorROTSpeed = 0.5f;

    }
    #endregion
    #region Generate charging animation
    // Group all function that serve the same algorithm
    public void Charging()

    {
        isStart = true;
        LeftWeapon = FindAnyObjectByType<FighterController>().LeftWeaponPosition;
        RightWeapon = FindAnyObjectByType<FighterController>().RightWeaponPosition;
        CharingClone = Instantiate(ChargingEffect, LeftWeapon.transform.position, Quaternion.identity);
        CharingClone2 = Instantiate(ChargingEffect, RightWeapon.transform.position, Quaternion.identity);
        CharingClone.SetActive(true);
        CharingClone2.SetActive(true);
        Destroy(CharingClone, 3f);
        Destroy(CharingClone2, 3f);
    }
    public void ChangeAnimationPos()
    {
        if (CharingClone != null && CharingClone2 != null)
        {
            CharingClone.transform.position = LeftWeapon.transform.position;
            CharingClone2.transform.position = RightWeapon.transform.position;
        } else
        {
            isStart = false;
        }
    }
    #endregion
   
   
}
