using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBurst : Powers
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
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    private List<Vector2> VList;
    private List<float> AngleList;
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
    #endregion
    #region Generate rocket
    // Group all function that serve the same algorithm
    public void GenerateRocket()
    {
        VList = CalculateAngle(10);
        for (int i = 0;  i < 1; i++)
        {
            GameObject game = Instantiate(Effect, VList[i], Quaternion.identity);
            game.SetActive(true);
            game.transform.Rotate(0, 0, -AngleList[i]);
            game.GetComponent<Rigidbody2D>().velocity = VList[i]*10;
            game.GetComponent<RocketBurstBullet>().Distance = Range;
            game.GetComponent<RocketBurstBullet>().Damage = DPH;
        }
    }
    #endregion
    #region Calculate angle for each rocket
    // Group all function that serve the same algorithm
    public List<Vector2> CalculateAngle(float range)
    {
        float angle = Fighter.GetComponent<PlayerMovement>().CurrentRotateAngle;
        List<Vector2> VList = new List<Vector2>();
        AngleList = new List<float>();
        for (int i = 0; i < AoH; i++)
        {
            angle += 360 / AoH;
            AngleList.Add(angle);
            float x = 0, y = 0;
            if (angle < 0) angle = angle % 360 + 360;
            if (angle >= 360) angle = angle % 360;
            if (angle >= 0 && angle <= 90)
            {
                x = Mathf.Abs(Mathf.Sin(angle * Mathf.Deg2Rad) * range);
                y = Mathf.Abs(Mathf.Cos(angle * Mathf.Deg2Rad) * range);
            }
            else if (angle > 90 && angle <= 180)
            {
                x = Mathf.Abs(Mathf.Sin((180 - angle) * Mathf.Deg2Rad) * range);
                y = -Mathf.Abs(Mathf.Cos((180 - angle) * Mathf.Deg2Rad) * range);
            }
            else if (angle > 180 && angle <= 270)
            {
                x = -Mathf.Abs(Mathf.Sin((angle - 180) * Mathf.Deg2Rad) * range);
                y = -Mathf.Abs(Mathf.Cos((angle - 180) * Mathf.Deg2Rad) * range);
            }
            else if (angle > 270 && angle < 360)
            {
                x = -Mathf.Abs(Mathf.Sin((360 - angle) * Mathf.Deg2Rad) * range);
                y = Mathf.Abs(Mathf.Cos((360 - angle) * Mathf.Deg2Rad) * range);
            }
            VList.Add(new Vector2(x, y));
        }
        return VList;
    }
    #endregion
}
