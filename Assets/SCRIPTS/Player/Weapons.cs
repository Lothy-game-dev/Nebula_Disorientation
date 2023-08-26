using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    #region ComponentVariables

    #endregion
    #region InitializeVariables
    public GameObject Fighter;
    public GameObject Aim;
    public GameObject RotatePoint;
    public GameObject WeaponPosition;
    public GameObject ShootingPosition;
    public bool isLeftWeapon;
    public GameObject Bullet;
    public float RotateLimitNegative;
    public float RotateLimitPositive;
    #endregion
    #region NormalVariables
    public float RateOfFire;
    private float FireTimer;
    private PlayerMovement pm;
    private float PrevAngle;
    private float CalAngle;
    private float CurrentAngle;
    private float ExpectedAngle;
    private float LimitNegative;
    private float LimitPositive;
    private float AutoChangeDirAngle;
    public bool tracking;
    private int MouseInput;
    public bool Fireable;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        pm = Fighter.GetComponent<PlayerMovement>();
        PrevAngle = 0;
        CalAngle = 0;
        CurrentAngle = 0;
        if (isLeftWeapon) MouseInput = 0;
        else MouseInput = 1;
        Fireable = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Moving To Location On Player
        transform.position = WeaponPosition.transform.position;
        // Rotate the weapon around rotate point clockwise with angle calculated
        CalAngle = CalculateRotateAngle();
        CurrentAngle %= 360;
        PrevAngle %= 360;
        if (tracking)
        {
            ExpectedAngle = CurrentAngle + CalAngle - PrevAngle;
            if (ExpectedAngle >= 360)
            {
                ExpectedAngle -= 360;
            } else if (ExpectedAngle < 0)
            {
                ExpectedAngle += 360;
            }
            LimitNegative = 360 + RotateLimitNegative + pm.CurrentRotateAngle % 360;
            if (LimitNegative >= 360)
            {

                LimitNegative -= 360;
            } else if (LimitNegative < 0)
            {
                LimitNegative += 360;
            }
            LimitPositive = RotateLimitPositive + pm.CurrentRotateAngle % 360;
            if (LimitPositive >= 360)
            {
                LimitPositive -= 360;
            } else if (LimitPositive < 0)
            {
                LimitPositive += 360;
            }
            if (PrevAngle != CalAngle)
            {
                if (CheckIfAngle1BetweenAngle2And3(ExpectedAngle, LimitNegative, LimitPositive))
                {
                    Fireable = true;
                    transform.RotateAround(RotatePoint.transform.position, Vector3.back, CalAngle - PrevAngle);
                    CurrentAngle = ExpectedAngle;
                    PrevAngle = CalAngle;
                } else
                {
                    Fireable = false;
                    float NearestAngle = NearestPossibleAngle(CurrentAngle, LimitNegative, LimitPositive);
                    transform.RotateAround(RotatePoint.transform.position, Vector3.back, NearestAngle);
                    CurrentAngle += NearestAngle;
                    PrevAngle += NearestAngle;
                }
            } 
        }
        // Fire Weapon's Bullet
        if (FireTimer <= 0f)
        {
            if (Input.GetMouseButton(MouseInput) && Fireable)
            {
                FireBullet();
                FireTimer = 1 / RateOfFire;
            }
        } else
        {
            FireTimer -= Time.deltaTime;
        }
    }
    #endregion
    #region Weapon Rotation
    // Calculate Rotation To Aim
    float CalculateRotateAngle()
    {
        float x = transform.position.x - Aim.transform.position.x;
        float y = transform.position.y - Aim.transform.position.y;
        if (Mathf.Abs(x) < 0.5f && Mathf.Abs(y) < 0.5f) {
            tracking = false;
            Fireable = false;
            return 0;
        } else
        {
            tracking = true;
            Fireable = true;
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

    public bool CheckIfAngle1BetweenAngle2And3(float angle1, float angle2, float angle3)
    {
        if (0 <= angle1 && angle1 < 180)
        {
            if (180 <= angle2 && angle2 < 360 && 0 <= angle3 && angle3 < 180)
            {
                if (angle1 <= angle3) return true;
                else return false;
            }
            else if (180 <= angle2 && angle2 < 360 && 180 <= angle3 && angle3 < 360) return false;
            else if (0 <= angle2 && angle2 < 180 && 180 <= angle3 && angle3 < 360)
            {
                if (angle1 >= angle2) return true;
                else return false;
            }
            else if (0 <= angle2 && angle2 < 180 && 0 <= angle3 && angle3 < 180)
            {
                if (angle2 <= angle1 && angle1 <= angle3) return true;
                else return false;
            }
            else return false;
        } else if (180 <= angle1 && angle1 < 360)
        {
            if (180 <= angle2 && angle2 < 360 && 0 <= angle3 && angle3 < 180)
            {
                if (angle1 >= angle2) return true;
                else return false;
            }
            else if (180 <= angle2 && angle2 < 360 && 180 <= angle3 && angle3 < 360)
            {
                if (angle2 <= angle1 && angle1 <= angle3) return true;
                else return false;
            }
            else if (0 <= angle2 && angle2 < 180 && 180 <= angle3 && angle3 < 360)
            {
                if (angle1 <= angle3) return true;
                else return false;
            }
            else if (0 <= angle2 && angle2 < 180 && 0 <= angle3 && angle3 < 180) return false;
            else return false;
        } else return false;
    }

    public float NearestPossibleAngle(float angle1, float angle2, float angle3)
    {
        float AngleBetween12 = 0;
        float AngleBetween13 = 0;
        if (0 <= angle1 && angle1 < 180)
        {
            if (180 <= angle2 && angle2 < 360 && 0 <= angle3 && angle3 < 180)
            {
                AngleBetween12 = angle2 - angle1;
                AngleBetween13 = angle3 - angle1;
            }
            else if (180 <= angle2 && angle2 < 360 && 180 <= angle3 && angle3 < 360)
            {
                AngleBetween12 = angle2 - angle1;
                AngleBetween13 = angle3 - angle1;
            }
            else if (0 <= angle2 && angle2 < 180 && 180 <= angle3 && angle3 < 360)
            {
                AngleBetween12 = angle2 - angle1;
                AngleBetween13 = angle3 - angle1;
            }
            else if (0 <= angle2 && angle2 < 180 && 0 <= angle3 && angle3 < 180)
            {
                AngleBetween12 = angle2 - angle1;
                AngleBetween13 = angle3 - angle1;
            }
            else return 0;
        }
        else if (180 <= angle1 && angle1 < 360)
        {
            if (180 <= angle2 && angle2 < 360 && 0 <= angle3 && angle3 < 180)
            {
                AngleBetween12 = angle2 - angle1;
                AngleBetween13 = angle3 + 360 - angle1;
            }
            else if (180 <= angle2 && angle2 < 360 && 180 <= angle3 && angle3 < 360)
            {
                AngleBetween12 = angle2 - angle1;
                AngleBetween13 = angle3 - angle1;
            }
            else if (0 <= angle2 && angle2 < 180 && 180 <= angle3 && angle3 < 360)
            {
                AngleBetween12 = angle2 + 360 - angle1;
                AngleBetween13 = angle3 - angle1;
            }
            else if (0 <= angle2 && angle2 < 180 && 0 <= angle3 && angle3 < 180)
            {
                AngleBetween12 = angle2 + 360 - angle1;
                AngleBetween13 = angle3 + 360 - angle1;
            }
            else return 0;
        }
        return Mathf.Abs(AngleBetween12) > Mathf.Abs(AngleBetween13) ? AngleBetween13 : AngleBetween12;
    }
    #endregion
    #region Weapon Fire
    // Fire Bullet
    void FireBullet()
    {
        GameObject bulletFire = Instantiate(Bullet, ShootingPosition.transform.position, Quaternion.identity);
        bulletFire.transform.RotateAround(ShootingPosition.transform.position, Vector3.back, CalculateRotateAngle());
        BulletShared bul = bulletFire.GetComponent<BulletShared>();
        bul.Destination = Aim.transform.position;
        bulletFire.SetActive(true);
    }
    #endregion
}
