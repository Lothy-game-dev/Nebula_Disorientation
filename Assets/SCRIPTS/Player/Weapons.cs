using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapons : MonoBehaviour
{
    #region ComponentVariables
    private AudioSource aus;
    private SpaceZoneHazardEnvironment HazEnv;
    #endregion
    #region InitializeVariables
    public GameObject Fighter;
    public GameObject Aim;
    public GameObject RotatePoint;
    public GameObject WeaponPosition;
    public GameObject ShootingPosition;
    public GameObject OverHeatImage;
    public LayerMask EnemyLayer;
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
    public AudioClip WeaponChargeSound;
    public bool IsOrbWeapon;
    public GameObject ReloadBar;
    public GameObject Effect;
    public float Cooldown;
    public float Duration;
    public float ChargingTime;
    public GameObject WeaponPoint;
    public float WeaponROTSpeed;
    public GameObject Charging;
    public GameObject ChargingPosition;
    public GameplayInteriorController ControllerMain;
    public LOTWEffect LOTWEffect;
    #endregion
    #region NormalVariables
    public bool tracking;
    public bool Fireable;
    public int CurrentHitCount;
    public float HitCountResetTimer;
    public float OverheatSpeedIncreaseRate;
    public float currentOverheat;
    public float FighterWeaponDamageMod;
    public float FighterWeaponAoEMod;

    private bool isOverheatted;
    private float OverheatCDTimer;
    private float OverheatDecreaseTimer;
    private bool isWarning;

    public float FireTimer;
    private PlayerMovement pm;
    private FighterMovement fm;
    private WSMovement wm;
    public float PrevAngle;
    public float CalAngle;
    public float CurrentAngle;
    public float ExpectedAngle;
    private float LimitNegative;
    private float LimitPositive;
    private int MouseInput;
    private GameObject bulletFire;
    private BulletShared bul;
    private float audioScale;

    public bool BeamActivating;
    public bool isUsingWormhole;

    public bool isMainWeapon;
    public bool isFire;
    private float BeamTimer;
    private float ChargeTimer;
    private int DirMov;
    public bool isCharging;
    public bool isSpaceStation;

    public float ResetHitTimer;
    private bool isHiding;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        gameObject.AddComponent<LimitRendering>();
        pm = Fighter.GetComponent<PlayerMovement>();
        fm = Fighter.GetComponent<FighterMovement>();
        wm = Fighter.GetComponent<WSMovement>();
        HazEnv = FindObjectOfType<SpaceZoneHazardEnvironment>();
        if (ControllerMain==null)
        {
            ControllerMain = FindObjectOfType<GameplayInteriorController>();
        }
        PrevAngle = 0;
        CalAngle = 0;
        CurrentAngle = 0;
        if (isLeftWeapon) MouseInput = 0;
        else MouseInput = 1;
        Fireable = true;
        isUsingWormhole = false;
        BeamActivating = false;
        OverheatSpeedIncreaseRate = 1f;
        if (NoOverheat)
        {
            OverheatIncreasePerShot = 0;
        }
        gameObject.AddComponent<AudioSource>();
        aus = GetComponent<AudioSource>();
        aus.spatialBlend = 1;
        aus.rolloffMode = AudioRolloffMode.Linear;
        aus.maxDistance = 2000;
        aus.minDistance = 1000;
        aus.priority = 256;
        aus.dopplerLevel = 0;
        aus.spread = 360;
        audioScale = 0.5f;

        if (!isMainWeapon)
        {
            WeaponROTSpeed *= 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Moving To Location On Player
        if (WeaponPosition!=null)
        {
            transform.position = WeaponPosition.transform.position;
        }
        // Rotate the weapon around rotate point clockwise with angle calculated
            if (Aim != null)
                CalAngle = CalculateRotateAngle();
            CurrentAngle %= 360;
            PrevAngle %= 360;
            // Check rotate weapon for player
            if (tracking && pm != null)
            {
                // Calculate Angel and if they are <0 or >360 then set them to between 0 and 360
                ExpectedAngle = CurrentAngle + CalAngle - PrevAngle;
                if (ExpectedAngle >= 360)
                {
                    ExpectedAngle -= 360;
                }
                else if (ExpectedAngle < 0)
                {
                    ExpectedAngle += 360;
                }
                LimitNegative = 360 + RotateLimitNegative + pm.CurrentRotateAngle % 360;
                if (LimitNegative >= 360)
                {

                    LimitNegative -= 360;
                }
                else if (LimitNegative < 0)
                {
                    LimitNegative += 360;
                }
                LimitPositive = RotateLimitPositive + pm.CurrentRotateAngle % 360;
                if (LimitPositive >= 360)
                {
                    LimitPositive -= 360;
                }
                else if (LimitPositive < 0)
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
                    }
                    else
                    {
                        // Else set fireable = false
                        // And rotate the weapon to the nearest posible position to the mouse
                        // also end weapon sound if there is sound
                        // (only thermal weapon get this case since their sounds loop)
                        Fireable = false;
                        if (!isWarning && !isOverheatted && aus.clip != null && IsThermalType)
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
        // Check rotate Weapon for non-player
        if (tracking && fm != null)
        {
            if (Aim!=null)
            {
                Vector2 RotPointToTarget = Aim.transform.position - RotatePoint.transform.position;
                Vector2 RotPointToShoot = Fighter.transform.position - (fm.GetComponent<EnemyShared>()!=null? fm.GetComponent<EnemyShared>().BackFire.transform.position : fm.GetComponent<AlliesShared>().BackFire.transform.position);
                float angle = Vector2.Angle(RotPointToTarget, RotPointToShoot);
                if (angle <= RotateLimitPositive)
                {
                    Fireable = true;
                    transform.RotateAround(RotatePoint.transform.position, new Vector3(0, 0, DirMov), CurrentAngle);
                    CheckIsUpOrDownMovement();
                    transform.RotateAround(RotatePoint.transform.position, new Vector3(0, 0, -DirMov), angle);
                    CurrentAngle = angle;
                }
                else
                {
                    Fireable = false;
                }
            }
        }
        // Check rotate Weapon for Warship
        if (tracking && wm != null)
        {
            if (Aim != null)
            {
                Vector3 pos = ShootingPosition.transform.position - RotatePoint.transform.position;
                Vector3 ToEnemy = Aim.transform.position - ShootingPosition.transform.position;
                float angle = Vector3.Angle(ToEnemy, pos);
                CheckIsUpOrDownMovement();
                if (angle < 5)
                {
                    angle = 0;
                    isFire = true;
                } else
                {
                    if (!isMainWeapon) isFire = false;
                }  
                transform.RotateAround(WeaponPoint.transform.position, new Vector3(0,0, -DirMov), 2 * WeaponROTSpeed);
                CurrentAngle = angle;
            }
        }

        // Check rotate Weapon for SpaceStation
        if (tracking && isSpaceStation)
        {
            if (Aim != null)
            {
                Vector3 pos = ShootingPosition.transform.position - RotatePoint.transform.position;
                Vector3 ToEnemy = Aim.transform.position - ShootingPosition.transform.position;
                float angle = Vector3.Angle(ToEnemy, pos);
                CheckIsUpOrDownMovement();
                if (angle < 5)
                {
                    angle = 0;
                    isFire = true;
                }
                else
                {
                    if (!isMainWeapon) isFire = false;
                }
                transform.RotateAround(WeaponPoint.transform.position, new Vector3(0, 0, -DirMov), 2 * WeaponROTSpeed);
            }
        }
        // Reset thermal hit count per 1/rate of hit second
        if (HitCountResetTimer > 0f)
            {
                HitCountResetTimer -= Time.deltaTime;
            }
            else
            {
                CurrentHitCount = 0;
                HitCountResetTimer = 1 / RateOfHit;
            }
            // Check weapon overheat
            if (Fighter.GetComponent<PlayerFighter>() != null)
                CheckOverheatStatus();
            // Remove sound when stop holding for thermal weapon
            // Kinetic and other type unaffected
            if (Input.GetMouseButtonUp(MouseInput) && Fighter.GetComponent<PlayerFighter>() != null)
            {
                if (!isOverheatted && IsThermalType)
                {
                    EndSound();
                }
            }

        ResetHitTimer -= Time.deltaTime;
        if (ResetHitTimer <= 0f)
        {
            CurrentHitCount = 0;
            ResetHitTimer = 1 / RateOfFire;
        }

        CheckCollide();
       

    }
    private void FixedUpdate()
    {
        // Fire Weapon's Bullet
        if (FireTimer <= 0f)
        {
            if (Input.GetMouseButton(MouseInput) && Fireable && !isOverheatted && Fighter.GetComponent<PlayerFighter>() != null && !Fighter.GetComponent<PlayerFighter>().isFrozen && !Fighter.GetComponent<PlayerFighter>().isSFBFreeze && !BeamActivating && !isUsingWormhole && !ControllerMain.IsInLoading)
            {
                ReloadBar.GetComponent<Image>().fillAmount = 1;
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
        if (ReloadBar!=null)
        {
            ReloadBar.GetComponent<Image>().fillAmount -= RateOfFire * Time.fixedDeltaTime;
        }
        FireTimer -= Time.deltaTime;


        if (Fireable && isMainWeapon && isFire)
        {
            if (Aim == null)
            {
                EndSound();
            }
            else
            {
                if (isCharging)
                {
                    if (ChargeTimer == 0)
                    {
                        LaserBeamChargingSound();
                        ChargingWSLaserBeam();
                    }
                    ChargeTimer += Time.deltaTime;
                }
                if (ChargeTimer >= ChargingTime)
                {
                    isCharging = false;
                    if (BeamTimer >= Duration)
                    {
                        isFire = false;
                        Fireable = false;
                        BeamTimer = 0f;
                        ChargeTimer = 0f;
                        WeaponROTSpeed *= 2;
                    }
                    else
                    {
                        if (BeamTimer == 0)
                        {
                            LaserBeamSound();
                            WeaponROTSpeed /= 2;
                        }
                        FireWSLaserBeam();
                        FireTimer = 1 / 14;                   
                        BeamTimer += Time.deltaTime;
                    }
                }
            }
        }
    }
    #endregion
    #region Weapon Rotation
    // Calculate Rotation To Aim
    float CalculateRotateAngle()
    {
        float x = RotatePoint.transform.position.x - Aim.transform.position.x;
        float y = RotatePoint.transform.position.y - Aim.transform.position.y;
        if (Mathf.Abs(x) < 0.5f && Mathf.Abs(y) < 0.5f) {
            Fireable = false;
            return 0;
        } else
        {
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

    float AICalculateRotateAngle(float xIn, float yIn)
    {
        float x = RotatePoint.transform.position.x - Aim.transform.position.x - xIn;
        float y = RotatePoint.transform.position.y - Aim.transform.position.y - yIn;
        if (Mathf.Abs(x) < 0.5f && Mathf.Abs(y) < 0.5f)
        {
            Fireable = false;
            return 0;
        }
        else
        {
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
        if (RotateLimitPositive<=90)
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
            }
            else if (180 <= angle1 && angle1 < 360)
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
            }
            else return false;
        } else
        {
            if (0 <= angle1 && angle1 < 180)
            {
                if (180 <= angle2 && angle2 < 360 && 0 <= angle3 && angle3 < 180)
                {
                    if (angle1 <= angle3) return true;
                    else return false;
                }
                else if (180 <= angle2 && angle2 < 360 && 180 <= angle3 && angle3 < 360) return true;
                else if (0 <= angle2 && angle2 < 180 && 180 <= angle3 && angle3 < 360)
                {
                    if (angle1 >= angle2) return true;
                    else return false;
                }
                else if (0 <= angle2 && angle2 < 180 && 0 <= angle3 && angle3 < 180)
                {
                    if (angle1 >= angle2 || angle1 <= angle3) return true;
                    else return false;
                }
                else return false;
            }
            else if (180 <= angle1 && angle1 < 360)
            {
                if (180 <= angle2 && angle2 < 360 && 0 <= angle3 && angle3 < 180)
                {
                    if (angle1 >= angle2) return true;
                    else return false;
                }
                else if (180 <= angle2 && angle2 < 360 && 180 <= angle3 && angle3 < 360)
                {
                    if (angle2 <= angle1 || angle1 >= angle3) return true;
                    else return false;
                }
                else if (0 <= angle2 && angle2 < 180 && 180 <= angle3 && angle3 < 360)
                {
                    if (angle1 <= angle3) return true;
                    else return false;
                }
                else if (0 <= angle2 && angle2 < 180 && 0 <= angle3 && angle3 < 180) return true;
                else return false;
            }
            else return false;
        }
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
    public void AIShootBullet(float angle)
    {
        if (Aim==null)
        {
            EndSound();
        } else
        {
            if (Fighter.GetComponent<FighterShared>()!=null)
            {
                if (FireTimer <= 0f && Fireable && !Fighter.GetComponent<FighterShared>().isFrozen &&
                !Fighter.GetComponent<FighterShared>().isSFBFreeze && !BeamActivating && !isUsingWormhole)
                {
                    if (!IsThermalType)
                    {
                        AIFireBullet(angle);
                        FireTimer = 1 / RateOfFire;
                    }
                    else
                    {
                        AIFireFlamethrowerOrb(angle);
                        FireTimer = 1 / RateOfFire;
                    }
                }
            } else if (Fighter.GetComponent<WSShared>() != null)
            {
                if (FireTimer <= 0f && Fireable)
                {
                    if (!IsThermalType)
                    {
                        AIFireBullet(angle);
                        FireTimer = 1 / RateOfFire;
                    }
                    else
                    {
                        AIFireFlamethrowerOrb(angle);
                        FireTimer = 1 / RateOfFire;
                    }
                }
            }
            
        }
    }

    // Fire Kinetic/Laser/Orb Bullet
    void AIFireBullet(float angle)
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
        bulletFire.transform.RotateAround(ShootingPosition.transform.position, Vector3.back, CalculateRotateAngle() + angle);
        // Same as above
        if (IsOrbWeapon)
        {
            bul = bulletFire.transform.GetChild(0).GetComponent<BulletShared>();
        }
        else
        {
            bul = bulletFire.GetComponent<BulletShared>();
        }
        // Set bullet's properties required
        Vector2 OldAim = Aim.transform.position - ShootingPosition.transform.position;
        Vector2 perpen = Vector2.Perpendicular(OldAim);
        Vector2 Dest = new Vector2(Aim.transform.position.x,Aim.transform.position.y) + perpen / perpen.magnitude * (angle > 0 ? -1 : 1) * Mathf.Tan(Mathf.Abs(angle) * Mathf.Deg2Rad) * OldAim.magnitude;
        bul.Destination = Dest;
        bul.WeaponShoot = this;
        bul.EnemyLayer = EnemyLayer;
        bulletFire.SetActive(true);
        GenerateEffect();
        if (Fighter.GetComponent<FighterShared>() != null)
            // Increase overheat bar for each shot, increasing with themral status overloadded
            currentOverheat += OverheatIncreasePerShot * (1 + Fighter.GetComponent<FighterShared>().OverheatIncreaseScale) * HazEnv.HazardOverheat;
        // Set reset timer
        OverheatDecreaseTimer = OverheatResetTimer;
        // Create sound
        if (!isOverheatted) KineticSound();
    }
    // Fire Flamethrower type orbs
    void AIFireFlamethrowerOrb(float angle)
    {
        // Because of performance issues, flamethrower type will fire 30 times each second,
        // with each time firing 5 orbs
        for (int i = 0; i < 5; i++)
        {
            // Mostly same as kinetic bullet
            GameObject orbFire = Instantiate(Bullet, ShootingPosition.transform.position, Quaternion.identity);
            // Set Angle random between -5 and 5 degree so that it could make the fire looks real
            float Angle = Random.Range(-5f, 5f);
            orbFire.transform.RotateAround(ShootingPosition.transform.position, Vector3.back, CalculateRotateAngle() + angle + Angle);
            BulletShared bul = orbFire.GetComponent<BulletShared>();
            bul.Destination = CalculateFTOrbDestination(Angle + angle, bul);
            // For the fire shape
            bul.Range = bul.MaxEffectiveDistance + 40 * Mathf.Cos(Angle * 90 / 10 * Mathf.Deg2Rad);
            bul.WeaponShoot = this;
            bul.EnemyLayer = EnemyLayer;
            orbFire.SetActive(true);
            GenerateEffect();
            if (Fighter.GetComponent<FighterShared>() != null)
            {
                // Sound
                if (!isOverheatted) ThermalSound();
                // Overheat
                currentOverheat += OverheatIncreasePerShot * (1 + Fighter.GetComponent<FighterShared>().OverheatIncreaseScale) * HazEnv.HazardOverheat;
                OverheatDecreaseTimer = OverheatResetTimer;
            }
        }
    }
    public void WSShootBullet()
    {       
        if (Aim == null)
        {
            EndSound();
        } else
        {
            if (FireTimer <= 0f && isFire && !isMainWeapon)
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
    }

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
        bul.EnemyLayer = EnemyLayer;
        bul.LOTWEffect = LOTWEffect;
        bulletFire.SetActive(true);
        GenerateEffect();
        if (Fighter.GetComponent<FighterShared>() != null) 
        // Increase overheat bar for each shot, increasing with themral status overloadded
        currentOverheat += OverheatIncreasePerShot * 
                (1 + Fighter.GetComponent<FighterShared>().OverheatIncreaseScale) * 
                (LOTWEffect != null && !LOTWEffect.LOTWAffectEnvironment ? 1 : HazEnv.HazardOverheat);
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
            bul.EnemyLayer = EnemyLayer;
            bul.LOTWEffect = LOTWEffect;
            orbFire.SetActive(true);
            GenerateEffect();
            if (Fighter.GetComponent<FighterShared>() != null)
            {
                // Sound
                if (!isOverheatted) ThermalSound();
                // Overheat
                currentOverheat += OverheatIncreasePerShot * (1 + Fighter.GetComponent<FighterShared>().OverheatIncreaseScale) *
                    (LOTWEffect != null && !LOTWEffect.LOTWAffectEnvironment ? 1 : HazEnv.HazardOverheat);
                OverheatDecreaseTimer = OverheatResetTimer;
            }
        }
    }

    void FireWSLaserBeam()
    {
        GameObject laserbeam = Instantiate(Bullet, ShootingPosition.transform.position, Quaternion.identity);
        laserbeam.transform.RotateAround(ShootingPosition.transform.position, new Vector3(0,0, (ShootingPosition.transform.position.x < transform.position.x ? 1 : -1)), Vector2.Angle(new Vector2(0,1),ShootingPosition.transform.position - RotatePoint.transform.position));
        BulletShared bul = laserbeam.GetComponent<BulletShared>();

        bul.Destination = RotatePoint.transform.position + (ShootingPosition.transform.position - RotatePoint.transform.position) * 2;
        bul.WeaponShoot = this;
        bul.EnemyLayer = EnemyLayer;
        laserbeam.SetActive(true);
    }

    public void ChargingWSLaserBeam()
    {
        
        GameObject charging = Instantiate(Charging, ChargingPosition.transform.position, Quaternion.identity);
        charging.transform.SetParent(gameObject.transform);
        charging.SetActive(true);
        Destroy(charging, ChargingTime);
    }

    private void GenerateEffect()
    {
        if (Effect!=null)
        {
            GameObject Eff = Instantiate(Effect, ShootingPosition.transform.position, Quaternion.identity);
            Eff.transform.localScale = Eff.transform.localScale * 30f;
            Color c = Eff.transform.GetComponent<SpriteRenderer>().color;
            c.a = 0f;
            Eff.transform.GetComponent<SpriteRenderer>().color = c;
            Eff.transform.SetParent(transform);
            Eff.SetActive(true);
            StartCoroutine(ShowEffect(Eff));
        }
    }

    private IEnumerator ShowEffect(GameObject go)
    {
        for (int i=0;i<10;i++)
        {
            Color c = go.transform.GetComponent<SpriteRenderer>().color;
            c.a += 0.1f;
            go.transform.GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 0; i < 10; i++)
        {
            Color c = go.transform.GetComponent<SpriteRenderer>().color;
            c.a -= 0.1f;
            go.transform.GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(0.1f);
        }
        Destroy(go);
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
            // Hide reload bar
            ReloadBar.gameObject.SetActive(false);
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
                ReloadBar.SetActive(true);
                ReloadBar.GetComponent<Image>().fillAmount = 0;
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
        if (name.Replace("(Clone)","")== "StarBlaster")
        {
            aus.volume = 0.25f* audioScale * ControllerMain.MasterVolumeScale * ControllerMain.SFXVolumeScale;
        } else
        aus.volume = 1f* audioScale * ControllerMain.MasterVolumeScale * ControllerMain.SFXVolumeScale;
    }
    public void ThermalSound()
    {
        if (aus.clip!=WeaponShootSound)
        {
            aus.clip = WeaponShootSound;
            aus.loop = true;
            aus.Play();
            if (Bullet.GetComponent<UsualThermalOrb>()!=null && Bullet.GetComponent<UsualThermalOrb>().isHeat) aus.volume = 1f * audioScale;
            else aus.volume = 0.2f * audioScale * ControllerMain.MasterVolumeScale * ControllerMain.SFXVolumeScale;
        }
    }

    public void LaserBeamSound()
    {
        aus.clip = WeaponShootSound;
        aus.loop = false;
        aus.Play();
        aus.volume = 1f * audioScale * ControllerMain.MasterVolumeScale * ControllerMain.SFXVolumeScale;
    }

    public void LaserBeamChargingSound()
    {
        aus.clip = WeaponChargeSound;
        aus.loop = false;
        aus.Play();
        aus.volume = 1f * audioScale * ControllerMain.MasterVolumeScale * ControllerMain.SFXVolumeScale;
    }

    public void EndSound()
    {
        if (aus!=null)
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
        aus.volume = 0.08f * audioScale * ControllerMain.MasterVolumeScale * ControllerMain.SFXVolumeScale;
    }
    #endregion
    #region Check direction
    private void CheckIsUpOrDownMovement()
    {
        Vector2 HeadToTarget = Aim.transform.position - ShootingPosition.transform.position;
        Vector2 MovingVector = ShootingPosition.transform.position - RotatePoint.transform.position;
        float angle = Vector2.Angle(HeadToTarget, MovingVector);
        float DistanceNew = Mathf.Cos(angle * Mathf.Deg2Rad) * HeadToTarget.magnitude;
        Vector2 TempPos = new Vector2(RotatePoint.transform.position.x, RotatePoint.transform.position.y) + MovingVector / MovingVector.magnitude * (MovingVector.magnitude + DistanceNew);
        Vector2 CheckPos = new Vector2(Aim.transform.position.x, Aim.transform.position.y) + (TempPos - new Vector2(Aim.transform.position.x, Aim.transform.position.y)) * 2;
        if (ShootingPosition.transform.position.x == RotatePoint.transform.position.x)
        {
            if (ShootingPosition.transform.position.y > RotatePoint.transform.position.y)
            {
                if (Aim.transform.position.x < ShootingPosition.transform.position.x)
                {
                    DirMov = -1;
                }
                else if (Aim.transform.position.x == ShootingPosition.transform.position.x)
                {
                    DirMov = 0;
                }
                else
                {
                    DirMov = 1;
                }
            }
            else
            {
                if (Aim.transform.position.x < ShootingPosition.transform.position.x)
                {
                    DirMov = 1;
                }
                else if (Aim.transform.position.x == ShootingPosition.transform.position.x)
                {
                    DirMov = 0;
                }
                else
                {
                    DirMov = -1;
                }
            }
        }
        else if (ShootingPosition.transform.position.y == RotatePoint.transform.position.y)
        {
            if (ShootingPosition.transform.position.x > RotatePoint.transform.position.x)
            {
                if (Aim.transform.position.y > ShootingPosition.transform.position.y)
                {
                    DirMov -= 1;
                }
                else if (Aim.transform.position.y == ShootingPosition.transform.position.y)
                {
                    DirMov = 0;
                }
                else
                {
                    DirMov = 1;
                }
            }
            else
            {
                if (Aim.transform.position.y > ShootingPosition.transform.position.y)
                {
                    DirMov = 1;
                }
                else if (Aim.transform.position.y == ShootingPosition.transform.position.y)
                {
                    DirMov = 0;
                }
                else
                {
                    DirMov -= 1;
                }
            }
        }
        else if (ShootingPosition.transform.position.x > RotatePoint.transform.position.x)
        {
            if (CheckPos.y < Aim.transform.position.y)
            {
                DirMov = -1;
            }
            else
            {
                DirMov = 1;
            }
        }
        else
        {
            if (CheckPos.y < Aim.transform.position.y)
            {
                DirMov = 1;
            }
            else
            {
                DirMov = -1;
            }
        }

    }
    #endregion
    #region Check Collide
    public void CheckCollide()
    {
        // Reduce the transparency when weapon collide warship or spacestation
        Color c = GetComponent<SpriteRenderer>().color;
        c.a = 1f;
        if (GetComponent<BoxCollider2D>() != null)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.zero);

            foreach (var x in hits)
            {
                if (x.collider.gameObject != gameObject && x.collider.gameObject != Fighter && (x.collider.GetComponent<WSShared>() != null || x.collider.GetComponent<SpaceStationShared>() != null))
                {
                    if (Fighter.transform.position.y > x.collider.transform.position.y)
                    {
                        c.a = 0f;
                    }
                }               
            }
        }
        GetComponent<SpriteRenderer>().color = c;
    }
    #endregion
}
