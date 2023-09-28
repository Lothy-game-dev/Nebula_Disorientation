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
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        
    }
    private void FixedUpdate()
    {
        if (isFire)
        {
            GenerateLaserBeam();
            DurationTimer += Time.deltaTime;

            if (DurationTimer >= Duration)
            {
                isFire = false;
                DurationTimer = 0f;
                Debug.Log(Duration);
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
            game.GetComponent<Beam>().InitScale = game.transform.localScale;
            game2.GetComponent<Rigidbody2D>().velocity = pos*2;
            game2.GetComponent<Beam>().Distance = Range;
            game2.GetComponent<Beam>().InitScale = game2.transform.localScale;

        }

    }
    #endregion
    #region Generate charging animation
    // Group all function that serve the same algorithm
    public void Charging()
    {
        Vector2 pos = CalculatePos(40);
        GameObject charging = Instantiate(ChargingEffect, pos, Quaternion.identity);
        charging.SetActive(true);
        Destroy(charging, 3f);
    }
    #endregion
}
