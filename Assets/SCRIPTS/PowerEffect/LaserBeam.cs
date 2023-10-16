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
            resetHitTimer = 1/15f;
        }
    }
    private void FixedUpdate()
    {
        if (isFire)
        {
            GenerateLaserBeam();
            LeftWeapon.GetComponent<Weapons>().BeamActivating = true;
            RightWeapon.GetComponent<Weapons>().BeamActivating = true;
            if (DurationTimer == 0)
            {
                PlaySound(SoundEffect);
            }
            DurationTimer += Time.fixedDeltaTime;

            if (DurationTimer >= Duration)
            {
                LeftWeapon.GetComponent<Weapons>().BeamActivating = false;
                RightWeapon.GetComponent<Weapons>().BeamActivating = false;
                DurationTimer = 0f;               
                isFire = false;
                //slow down Fighter when firing
                if (Fighter.GetComponent<PlayerMovement>() != null)
                {
                    Fighter.GetComponent<PlayerMovement>().LaserBeamSlowScale = 1f;
                    Fighter.GetComponent<PlayerMovement>().ExteriorROTSpeed = 1;
                }
                else if (Fighter.GetComponent<FighterMovement>() != null)
                {
                    Fighter.GetComponent<FighterMovement>().LaserBeamSlowScale = 1;
                    Fighter.GetComponent<FighterMovement>().ExteriorROTSpeed = 1;
                }
            }
        }
        
    }
    #endregion
    #region Generate bullet
    // Group all function that serve the same algorithm
    public void GenerateLaserBeam()
    {
        Vector2 pos = CalculatePos(Range);
        LeftWeapon = Fighter.GetComponent<FighterShared>().LeftWeapon;
        RightWeapon = Fighter.GetComponent<FighterShared>().RightWeapon;
        int n = 6;
        int m = 2;
        for (int i = 0; i < n; i++)
        {
            //Generate
            Vector2 posBullet = new Vector2(LeftWeapon.transform.position.x, LeftWeapon.transform.position.y) + (n/2 + 1 - i)*pos/(Range*0.12f);
            Vector2 posBullet2 = new Vector2(RightWeapon.transform.position.x, RightWeapon.transform.position.y) + (n / 2 + 1 - i) * pos / (Range * 0.12f);
            GameObject game = Instantiate(Effect, new Vector3(posBullet.x, posBullet.y, LeftWeapon.transform.position.z), Quaternion.identity);
            GameObject game2 = Instantiate(Effect, new Vector3(posBullet2.x, posBullet2.y, RightWeapon.transform.position.z), Quaternion.identity);
            game.GetComponent<Beam>().Distance = Range;
            game.GetComponent<Beam>().Damage = DPH;
            game.GetComponent<Beam>().Layer = EnemyLayer;
            game.GetComponent<Beam>().Fighter = Fighter;
            game.GetComponent<Beam>().LOTWEffect = LOTWEffect;
            game.transform.localScale = game.transform.localScale * (i > m ? (float)(n-i)/(n-m) : 1);

            //Change the spirte of the bullet like a laser
            if (!name.Contains("Superior")) {
                if (i > m)
                {
                    Color c = game.GetComponent<SpriteRenderer>().color;
                    c.r = (i > m ? (float)(n - i) / (n - m) : 1);
                    c.a = (i > m ? (float)(n - i) / (n - m) : 1);
                    game.GetComponent<SpriteRenderer>().color = c;
                    Color c2 = game.transform.GetChild(2).GetComponent<SpriteRenderer>().color;
                    c2.a = (i > m ? (float)(n - i) / (n - m) : 1);
                    game.transform.GetChild(2).GetComponent<SpriteRenderer>().color = c2;
                }
                if (i > m)
                {
                    Color c = game2.GetComponent<SpriteRenderer>().color;
                    c.r = (i > m ? (float)(n - i) / (n - m) : 1);
                    c.a = (i > m ? (float)(n - i) / (n - m) : 1);
                    game2.GetComponent<SpriteRenderer>().color = c;
                    Color c2 = game2.transform.GetChild(2).GetComponent<SpriteRenderer>().color;
                    c2.a = (i > m ? (float)(n - i) / (n - m) : 1);
                    game2.transform.GetChild(2).GetComponent<SpriteRenderer>().color = c2;
                }
            }
           
            game.GetComponent<Beam>().Laser = this;
            game2.GetComponent<Beam>().Distance = Range;
            game2.GetComponent<Beam>().Damage = DPH;
            game2.GetComponent<Beam>().Layer = EnemyLayer;
            game2.GetComponent<Beam>().LOTWEffect = LOTWEffect;
            game2.transform.localScale = game2.transform.localScale * (i > m ? (float)(n - i) / (n - m) : 1);
            game2.GetComponent<Beam>().Laser = this;
            
            game.SetActive(true);
            game2.SetActive(true);
            game.GetComponent<Rigidbody2D>().velocity = pos*2;
            game2.GetComponent<Rigidbody2D>().velocity = pos*2;


        }
        //slow down Fighter when firing
        if (Fighter.GetComponent<PlayerMovement>()!=null)
        {
            Fighter.GetComponent<PlayerMovement>().LaserBeamSlowScale = 0.5f;
            Fighter.GetComponent<PlayerMovement>().ExteriorROTSpeed = 0.5f;
        } else if (Fighter.GetComponent<FighterMovement>() != null)
        {
            Fighter.GetComponent<FighterMovement>().LaserBeamSlowScale = 0.5f;
            Fighter.GetComponent<FighterMovement>().ExteriorROTSpeed = 0.5f;
        }
    }
    #endregion
    #region Generate charging animation
    // Group all function that serve the same algorithm
    public void Charging()

    {
        isStart = true;
        PlaySound(ChargingSoundEffect);
        LeftWeapon = Fighter.GetComponent<FighterShared>().LeftWeapon;
        RightWeapon = Fighter.GetComponent<FighterShared>().RightWeapon;
        CharingClone = Instantiate(ChargingEffect, LeftWeapon.transform.position, Quaternion.identity);
        CharingClone2 = Instantiate(ChargingEffect, RightWeapon.transform.position, Quaternion.identity);
        CharingClone.SetActive(true);
        CharingClone2.SetActive(true);
        Destroy(CharingClone, 1.14f);
        Destroy(CharingClone2, 1.14f);
    }
    public void ChangeAnimationPos()
    {
        if (CharingClone != null && CharingClone2 != null)
        {
            CharingClone.transform.position = new Vector3(LeftWeapon.transform.position.x + CalculatePos(10).x, LeftWeapon.transform.position.y + CalculatePos(10).y, 0);
            CharingClone2.transform.position = new Vector3(RightWeapon.transform.position.x + CalculatePos(10).x, RightWeapon.transform.position.y + CalculatePos(10).y, 0);
        } else
        {
            isStart = false;
        }
    }

    public void DestroyChargingWhenNotHolding()
    {
        if (CharingClone != null && CharingClone2 != null)
        {
            EndSound();
            Destroy(CharingClone);
            Destroy(CharingClone2);
        }
    }
    #endregion
   
   
}
