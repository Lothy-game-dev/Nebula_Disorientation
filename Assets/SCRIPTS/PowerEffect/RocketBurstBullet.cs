using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBurstBullet : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
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
        CheckPosition();
    }
    #endregion
    #region Change rotation
    // Group all function that serve the same algorithm
    public void CheckPosition()
    {
        Vector2 pos = CalculatePos(70);
        transform.position = pos;
        
    }
    public Vector2 CalculatePos(float range)
    {
        float angle = FindAnyObjectByType<FighterController>().PlayerFighter.GetComponent<PlayerMovement>().CurrentRotateAngle;
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
        return new Vector2(x, y);

    }
    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    #endregion
}
