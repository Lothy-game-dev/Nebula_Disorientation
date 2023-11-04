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
    private float BeamAngle;
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
                    Fighter.GetComponent<PlayerMovement>().ExteriorROTSpeed = 1;
                }
                else if (Fighter.GetComponent<FighterMovement>() != null)
                {
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
        //slow down Fighter when firing
        if (Fighter.GetComponent<PlayerMovement>()!=null)
        {
            Fighter.GetComponent<PlayerMovement>().ExteriorROTSpeed = 0.5f;
            BeamAngle = Fighter.GetComponent<PlayerMovement>().CurrentRotateAngle;
        } else if (Fighter.GetComponent<FighterMovement>() != null)
        {
            Fighter.GetComponent<FighterMovement>().ExteriorROTSpeed = 0.5f;
            BeamAngle = Fighter.GetComponent<FighterMovement>().CurrentRotateAngle;
        }

        Vector2 pos = CalculatePos(Range);
        LeftWeapon = Fighter.GetComponent<FighterShared>().LeftWeapon;
        RightWeapon = Fighter.GetComponent<FighterShared>().RightWeapon;

        GameObject LeftWPRotationPoint = LeftWeapon.GetComponent<Weapons>().RotatePoint;
        GameObject RightWPRotationPoint = RightWeapon.GetComponent<Weapons>().RotatePoint;
        //Generate          
        GameObject game = Instantiate(Effect, Fighter.GetComponent<FighterShared>().LeftLaserBeamPos.transform.position, Quaternion.identity);
        game.GetComponent<Beam>().Distance = Range;
        game.GetComponent<Beam>().Damage = DPH;
        game.GetComponent<Beam>().Layer = EnemyLayer;
        game.GetComponent<Beam>().Fighter = Fighter;
        game.GetComponent<Beam>().Laser = this;
        game.transform.Rotate(0, 0, -BeamAngle);

        GameObject game2 = Instantiate(Effect, Fighter.GetComponent<FighterShared>().RightLaserBeamPos.transform.position, Quaternion.identity);
        game2.GetComponent<Beam>().Distance = Range;
        game2.GetComponent<Beam>().Damage = DPH;
        game2.GetComponent<Beam>().Layer = EnemyLayer;
        game2.GetComponent<Beam>().Fighter = Fighter;
        game2.GetComponent<Beam>().Laser = this;
        game2.transform.Rotate(0, 0, -BeamAngle);


        game.SetActive(true);
        game2.SetActive(true);
        game.GetComponent<Rigidbody2D>().velocity = pos*2;
        game2.GetComponent<Rigidbody2D>().velocity = pos*2;
      
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
    #region Weapon Rotation
    // Calculate Rotation To Aim
    float CalculateRotateAngle(Vector2 RotPoint, Vector2 AimPos)
    {
        float x = RotPoint.x - AimPos.x;
        float y = RotPoint.y - AimPos.y;
        if (Mathf.Abs(x) < 0.5f && Mathf.Abs(y) < 0.5f)
        {        
            return 0;
        }       
        if (x > 0 && y > 0)
        {
            return Mathf.Atan((float)Mathf.Abs(x) / Mathf.Abs(y)) * Mathf.Rad2Deg + 180;
        }
        else if (x > 0 && y < 0)
        {
            return 360 - Mathf.Atan((float)Mathf.Abs(x) / Mathf.Abs(y)) * Mathf.Rad2Deg;
        }
        else if (x < 0 && y > 0)
        {
            return 180 - Mathf.Atan((float)Mathf.Abs(x) / Mathf.Abs(y)) * Mathf.Rad2Deg;
        }
        else if (x < 0 && y < 0)
        {
            return Mathf.Atan((float)Mathf.Abs(x) / Mathf.Abs(y)) * Mathf.Rad2Deg;
        }
        else return 0;
    }
    #endregion


}
