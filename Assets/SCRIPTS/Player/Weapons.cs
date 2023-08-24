using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    #region ComponentVariables

    #endregion
    #region InitializeVariables
    public GameObject Aim;
    public GameObject RotatePoint;
    public GameObject WeaponPosition;
    public GameObject ShootingPosition;
    public bool isLeftWeapon;
    public GameObject Bullet;
    #endregion
    #region NormalVariables
    private float PrevAngle;
    private float CalAngle;
    private bool tracking;
    private int MouseInput;
    private bool Fireable;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        PrevAngle = 0;
        CalAngle = 0;
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
        if (tracking)
        {
            if (PrevAngle != CalAngle)
            {
                transform.RotateAround(RotatePoint.transform.position, Vector3.back, CalAngle - PrevAngle);
            }
            PrevAngle = CalAngle;
        }
        // Fire Weapon's Bullet
        if (Input.GetMouseButtonDown(MouseInput) && Fireable)
        {
            FireBullet();
            Fireable = false;
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
    #endregion
    #region Weapon Fire
    // Fire Bullet
    void FireBullet()
    {
        GameObject bulletFire = Instantiate(Bullet, ShootingPosition.transform.position, Quaternion.identity);
        bulletFire.transform.RotateAround(ShootingPosition.transform.position, Vector3.back, CalculateRotateAngle());
        BulletShared bul = bulletFire.GetComponent<BulletShared>();
        bul.Destination = Aim.transform.position;
        bul.Speed = 100;
        bulletFire.SetActive(true);
    }
    #endregion
}
