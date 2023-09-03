using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    #region ComponentVariables
    private AudioSource aus;
    #endregion
    #region InitializeVariables
    public GameObject Fighter;
    public GameObject Aim;
    public GameObject RotatePoint;
    public GameObject WeaponPosition;
    public GameObject ShootingPosition;
    public GameObject OverHeatImage;
    public bool isLeftWeapon;
    public GameObject Bullet;
    public float RotateLimitNegative;
    public float RotateLimitPositive;
    public bool IsThermalType;
    public int RateOfHit;
    public float RateOfFire;
    public bool NoOverheat;
    public float OverheatIncreasePerShot;
    public float OverheatResetTimer;
    public float OverheatTimer;
    public AudioClip WeaponShootSound;
    public bool IsOrbWeapon;
    #endregion
    #region NormalVariables
    public bool tracking;
    public bool Fireable;
    public int CurrentHitCount;
    public float HitCountResetTimer;
    public float OverheatSpeedIncreaseRate;
    public float currentOverheat;
    private bool isOverheatted;
    private float OverheatCDTimer;
    private float OverheatDecreaseTimer;
    private bool isWarning;

    private float FireTimer;
    private PlayerMovement pm;
    private float PrevAngle;
    private float CalAngle;
    private float CurrentAngle;
    private float ExpectedAngle;
    private float LimitNegative;
    private float LimitPositive;
    private int MouseInput;
    private GameObject bulletFire;
    private BulletShared bul;
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
        OverheatSpeedIncreaseRate = 1f;
        if (NoOverheat)
        {
            OverheatIncreasePerShot = 0;
        }
        gameObject.AddComponent<AudioSource>();
        aus = GetComponent<AudioSource>();
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
            // Calculate Angel and if they are <0 or >360 then set them to between 0 and 360
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
            // In case the mouse doesnt change position: Will not add the rotation
            if (PrevAngle != CalAngle)
            {
                // If the mouse aim vector in shootable range -> set fireable = true
                // And rotate the weapon to the mouse aim position
                if (CheckIfAngle1BetweenAngle2And3(ExpectedAngle, LimitNegative, LimitPositive))
                {
                    Fireable = true;
                    transform.RotateAround(RotatePoint.transform.position, Vector3.back, CalAngle - PrevAngle);
                    CurrentAngle = ExpectedAngle;
                    PrevAngle = CalAngle;
                } else
                {
                    // Else set fireable = false
                    // And rotate the weapon to the nearest posible position to the mouse
                    // also end weapon sound if there is sound
                    // (only thermal weapon get this case since their sounds loop)
                    Fireable = false;
                    if (!isWarning && !isOverheatted && aus.clip!=null && IsThermalType)
                    {
                        EndSound();
                    }
                    float NearestAngle = NearestPossibleAngle(CurrentAngle, LimitNegative, LimitPositive);
                    transform.RotateAround(RotatePoint.transform.position, Vector3.back, NearestAngle);
                    CurrentAngle += NearestAngle;
                    PrevAngle += NearestAngle;
                }
            } 
        }
        // Reset thermal hit count per 1/rate of hit second
        if (HitCountResetTimer > 0f)
        {
            HitCountResetTimer -= Time.deltaTime;
        } else
        {
            CurrentHitCount = 0;
            HitCountResetTimer = 1/RateOfHit;
        }
        // Check weapon overheat
        CheckOverheatStatus();
        // Remove sound when stop holding for thermal weapon
        // Kinetic and other type unaffected
        if (Input.GetMouseButtonUp(MouseInput))
        {
            if (!isOverheatted && IsThermalType)
            {
                EndSound();
            }
        }
    }
    private void FixedUpdate()
    {
        // Fire Weapon's Bullet
        if (FireTimer <= 0f)
        {
            if (Input.GetMouseButton(MouseInput) && Fireable && !isOverheatted)
            {
                if (!IsThermalType)
                {
                    FireBullet();
                    FireTimer = 1 / RateOfFire;
                }
                else
                {
                    FireFlamethrowerOrb();
                    FireTimer = 1 / RateOfFire;
                }
            }
        }
        FireTimer -= Time.deltaTime;
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
    // Fire Kinetic/Laser/Orb Bullet
    void FireBullet()
    {
        // Incase Lava orb weapon, the gameobject will be different for tracer && rotate animation
        // so there will be an if clause
        if (IsOrbWeapon)
        {
            bulletFire = Instantiate(Bullet.transform.parent.gameObject, ShootingPosition.transform.position, Quaternion.identity);
        } 
        else
        {
            bulletFire = Instantiate(Bullet, ShootingPosition.transform.position, Quaternion.identity);
        }
        // Rotate the bullet to the firing position
        bulletFire.transform.RotateAround(ShootingPosition.transform.position, Vector3.back, CalculateRotateAngle());
        // Same as above
        if (IsOrbWeapon)
        {
            bul = bulletFire.transform.GetChild(0).GetComponent<BulletShared>();
        } else
        {
            bul = bulletFire.GetComponent<BulletShared>();
        }
        // Set bullet's properties required
        bul.Destination = Aim.transform.position;
        bul.WeaponShoot = this;
        bulletFire.SetActive(true);
        // Increase overheat bar for each shot, increasing with themral status overloadded
        currentOverheat += OverheatIncreasePerShot * (1 + Fighter.GetComponent<FighterShared>().OverheatIncreaseScale);
        // Set reset timer
        OverheatDecreaseTimer = OverheatResetTimer;
        // Create sound
        if (!isOverheatted) KineticSound();
    }
    // Fire Flamethrower type orbs
    void FireFlamethrowerOrb()
    {
        // Because of performance issues, flamethrower type will fire 30 times each second,
        // with each time firing 5 orbs
        for (int i=0;i<5;i++)
        {
            // Mostly same as kinetic bullet
            GameObject orbFire = Instantiate(Bullet, ShootingPosition.transform.position, Quaternion.identity);
            // Set Angle random between -5 and 5 degree so that it could make the fire looks real
            float Angle = Random.Range(-5f, 5f);
            orbFire.transform.RotateAround(ShootingPosition.transform.position, Vector3.back, CalculateRotateAngle() + Angle);
            BulletShared bul = orbFire.GetComponent<BulletShared>();
            bul.Destination = CalculateFTOrbDestination(Angle, bul);
            // For the fire shape
            bul.Range = bul.MaxEffectiveDistance + 40 * Mathf.Cos(Angle * 90/10 * Mathf.Deg2Rad);
            bul.WeaponShoot = this;
            orbFire.SetActive(true);
            // Sound
            if (!isOverheatted) ThermalSound();
            // Overheat
            currentOverheat += OverheatIncreasePerShot * (1 + Fighter.GetComponent<FighterShared>().OverheatIncreaseScale);
            OverheatDecreaseTimer = OverheatResetTimer;
        }
    }

    // Calculate Flamethrower type Orb
    Vector2 CalculateFTOrbDestination(float Angle, BulletShared bul)
    {
        // Get the vector from shooting pos to aim pos
        Vector2 baseVector = Aim.transform.position - ShootingPosition.transform.position;
        // This is mostly for animation to be real
        baseVector = baseVector * bul.MaxEffectiveDistance * 1.5f / baseVector.magnitude;
        // Get the length of the vector
        float length = baseVector.magnitude;
        // This is pure Mathematics. Calculate the position the orb will be
        float length1 = length * Mathf.Cos(Angle * Mathf.Deg2Rad);
        Vector2 JoinPos = new Vector2(ShootingPosition.transform.position.x, ShootingPosition.transform.position.y)
            + baseVector * length1 / length;
        Vector2 peVector = Vector2.Perpendicular(baseVector);
        float length2 = length * Mathf.Sin(Angle * Mathf.Deg2Rad);
        Vector2 aimPos = JoinPos + peVector * length2 / length;
        return aimPos;
    }
    #endregion
    #region Overheat System
    // Check overheat
    void CheckOverheatStatus()
    {
        // If overheat rate < 80% when is not overheatted then remove warning sound
        if (currentOverheat <80 && !isOverheatted && isWarning)
        {
            EndOverheatWarningSound();
            isWarning = false;
        }
        // If overheat rate >= 80% then play overheat warning sounds
        if (currentOverheat >= 80 && !isOverheatted)
        {
            OverheatSound80Percent(((currentOverheat-75)*2)/100);
            isWarning = true;
        }
        // If overheat rate >= 100% and is not overheatted then become overheatted and set timer, etc.
        if (currentOverheat >= 100 && !isOverheatted)
        {
            isOverheatted = true;
            EndOverheatWarningSound();
            OverheatedSound();
            // Set overheat rate to 100 for exact animation
            currentOverheat = 100;
            OverheatCDTimer = OverheatTimer;
            OverheatDecreaseTimer = 0f;
        }
        // Overheat run out timer;
        if (OverheatCDTimer > 0f)
        {
            OverheatCDTimer -= Time.deltaTime;
        }
        else
        {
            if (isOverheatted)
            {
                isOverheatted = false;
                isWarning = false;
                EndSound();
                currentOverheat = 0f;
            }
        }
        // If is overheatted, set fireable = false and decrease overheat rate 100/overheatTimer per second
        if (isOverheatted)
        {
            Fireable = false;
            if (OverheatDecreaseTimer > 0f)
            {
                OverheatDecreaseTimer -= Time.deltaTime;
            }
            else
            {
                if (currentOverheat >= 100 / OverheatTimer)
                {
                    currentOverheat -= 100 / OverheatTimer;
                }
                else
                {
                    currentOverheat = 0;
                }
                OverheatDecreaseTimer = 1;
            }
        }
        // If not overheatted: Automatically decrease overheat when not firing based on the stats of weapons
        else
        {
            if (OverheatDecreaseTimer > 0f)
            {
                OverheatDecreaseTimer -= Time.deltaTime;
            }
            else
            {
                if (currentOverheat >= 1)
                {
                    currentOverheat -= 1;
                }
                else
                {
                    currentOverheat = 0;
                }
                OverheatDecreaseTimer = 1 / 20f;
            }
        }
    }
    #endregion
    #region Weapon Sound
    public void KineticSound()
    {
        aus.clip = WeaponShootSound;
        aus.loop = false;
        aus.Play();
        aus.volume = 1f;
    }
    public void ThermalSound()
    {
        if (aus.clip!=WeaponShootSound)
        {
            aus.clip = WeaponShootSound;
            aus.loop = true;
            aus.Play();
            if (Bullet.GetComponent<UsualThermalOrb>()!=null && Bullet.GetComponent<UsualThermalOrb>().isHeat) aus.volume = 1f;
            else aus.volume = 0.2f;
        }
    }

    public void EndSound()
    {
        aus.clip = null;
    }

    public void OverheatSound80Percent(float volume)
    {
        if (OverHeatImage.GetComponent<AudioSource>().clip != Fighter.GetComponent<PlayerFighter>().OverheatWarning)
        {
            OverHeatImage.GetComponent<AudioSource>().clip = Fighter.GetComponent<PlayerFighter>().OverheatWarning;
            OverHeatImage.GetComponent<AudioSource>().loop = true;
            OverHeatImage.GetComponent<AudioSource>().Play();
        }
        OverHeatImage.GetComponent<AudioSource>().volume = volume;
        OverHeatImage.GetComponent<AudioSource>().priority = 10;
    }

    public void EndOverheatWarningSound()
    {
        OverHeatImage.GetComponent<AudioSource>().clip = null;
    }

    public void OverheatedSound()
    {
        aus.clip = Fighter.GetComponent<PlayerFighter>().Overheated;
        aus.loop = true;
        aus.Play();
        aus.volume = 0.08f;
    }
    #endregion
}
