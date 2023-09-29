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
    public GameObject Effect;
    public GameObject ChargingEffect;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public GameObject LeftWeapon;
    public GameObject RightWeapon;
    public bool isFire;
    private float DurationTimer;
    public LayerMask EnemyLayer;
    private bool isStart;
    private GameObject CharingClone;
    private GameObject CharingClone2;
    private Vector2 Spd;
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
        
    }
    private void FixedUpdate()
    {
        if (isFire)
        {
            GenerateLaserBeam();
            if (DurationTimer == 0)
            {
                LaserBeamSound();
            }
            DurationTimer += Time.deltaTime;

            if (DurationTimer >= Duration)
            {
                isFire = false;
                DurationTimer = 0f;
                EndSound();
                FindAnyObjectByType<FighterController>().PlayerFighter.GetComponent<Rigidbody2D>().velocity = Spd;
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
        for (int i = 0; i < 14; i++)
        {
            Vector2 posBullet = new Vector2(LeftWeapon.transform.position.x, LeftWeapon.transform.position.y) + (8 - i)*pos/(Range*0.55f);
            Vector2 posBullet2 = new Vector2(RightWeapon.transform.position.x, RightWeapon.transform.position.y) + (8 - i) * pos / (Range * 0.55f);
            GameObject game = Instantiate(Effect, new Vector3(posBullet.x, posBullet.y, LeftWeapon.transform.position.z), Quaternion.identity);
            GameObject game2 = Instantiate(Effect, new Vector3(posBullet2.x, posBullet2.y, RightWeapon.transform.position.z), Quaternion.identity);
            game.GetComponent<Rigidbody2D>().velocity = pos*2;
            game.GetComponent<Beam>().Distance = Range;
            game.GetComponent<Beam>().Damage = DPH;
            game.GetComponent<Beam>().Layer = EnemyLayer;
            game.GetComponent<Beam>().InitScale = game.transform.localScale;
            game2.GetComponent<Rigidbody2D>().velocity = pos*2;
            game2.GetComponent<Beam>().Distance = Range;
            game2.GetComponent<Beam>().Damage = DPH;
            game2.GetComponent<Beam>().Layer = EnemyLayer;
            game2.GetComponent<Beam>().InitScale = game2.transform.localScale;

        }
        //slow down when firing
        FindAnyObjectByType<FighterController>().PlayerFighter.GetComponent<Rigidbody2D>().velocity *= 0.5f;

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
