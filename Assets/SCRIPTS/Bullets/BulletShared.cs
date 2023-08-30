using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShared : MonoBehaviour
{
    #region Shared Variables
    public Weapons WeaponShoot;
    public float MaxEffectiveDistance;
    public float MaximumDistance;
    public float Speed;
    public float AoE;
    public float BaseDamagePerHit;
    public LayerMask EnemyLayer;
    public LayerMask BlackholeLayer;
    public float Range;
    public Vector2 Destination;

    protected Rigidbody2D rb;
    protected float Distance;
    protected Vector2 Velocity;
    protected Vector2 RealVelocity;
    protected float DistanceTravel;

    private float RealDamage;
    private bool StartCounting;
    private bool isBHPulled;
    private List<Vector2> PulledVector;
    private bool AlreadyHit;
    #endregion
    #region Shared Functions
    public void InitializeBullet()
    {
        if (MaximumDistance==0f)
        {
            MaximumDistance = MaxEffectiveDistance;
        }
    }
    public void UpdateBullet()
    {
        CheckInsideBlackhole();
        CalculateVelocity(RealVelocity);
    }
    // Calculate Velocity Required To Reach Destination
    public void CalculateVelocity()
    {
        if (Destination!=null)
        {
            Distance = Mathf.Sqrt((Destination.x - transform.position.x) * (Destination.x - transform.position.x)
            + (Destination.y - transform.position.y) * (Destination.y - transform.position.y));
            Velocity = new Vector2((Destination.x - transform.position.x) / (Distance / Speed + 1), (Destination.y - transform.position.y) / (Distance / Speed + 1));
            RealVelocity = new Vector2(0, 0);
        }
    }
    // Acceleration for the first time second
    public IEnumerator Accelerate(float time)
    {
        for (int i=0; i<10; i++)
        {
            if (Mathf.Abs(RealVelocity.x) < Mathf.Abs(Velocity.x) && Mathf.Abs(RealVelocity.y) < Mathf.Abs(Velocity.y))
            {
                RealVelocity = new Vector2(RealVelocity.x + Velocity.x / 10, RealVelocity.y + Velocity.y / 10);
            }
            CalculateVelocity(RealVelocity);
            yield return new WaitForSeconds(time/10f);
        }
    }

    public void CalculateDamage()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 0.01f, EnemyLayer);
        foreach (var col in cols)
        {
            if (!AlreadyHit)
            {
                AlreadyHit = true;
                if (WeaponShoot.CurrentHitCount < WeaponShoot.RateOfHit)
                {
                    if (WeaponShoot.CurrentHitCount == 0)
                    {
                        WeaponShoot.HitCountResetTimer = 1f;
                    }
                    Collider2D[] cols2 = Physics2D.OverlapCircleAll(transform.position, AoE, EnemyLayer);
                    foreach (var col2 in cols2)
                    {
                        EnemyShared enemy = col2.gameObject.GetComponent<EnemyShared>();
                        if (enemy!=null)
                        {
                            enemy.CurrentHP -= RealDamage;
                            Debug.Log(RealDamage);
                        }
                    }
                    WeaponShoot.CurrentHitCount++;
                    Destroy(gameObject);
                } else
                {
                    Destroy(gameObject);
                }
            }
            break;
        }
    }

    public void CalculateThermalDamage(bool isHeat)
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 0.01f, EnemyLayer);
        foreach (var col in cols)
        {
            if (!AlreadyHit)
            {
                AlreadyHit = true;
                if (WeaponShoot.CurrentHitCount < WeaponShoot.RateOfHit)
                {
                    if (WeaponShoot.CurrentHitCount == 0)
                    {
                        WeaponShoot.HitCountResetTimer = 1f;
                    }
                    Collider2D[] cols2 = Physics2D.OverlapCircleAll(transform.position, AoE, EnemyLayer);
                    foreach (var col2 in cols2)
                    {
                        EnemyShared enemy = col2.gameObject.GetComponent<EnemyShared>();
                        if (enemy != null)
                        {
                            enemy.CurrentHP -= RealDamage;
                            enemy.ReceiveThermalDamage(isHeat);
                            Debug.Log(RealDamage);
                        }
                    }
                    WeaponShoot.CurrentHitCount++;
                    Destroy(gameObject);
                }
            }
            break;
        }
    }

    public void CheckDistanceTravel()
    {
        if (Range!=0f)
        {
            MaxEffectiveDistance = Range;
            MaximumDistance = Range;
        }
        if (DistanceTravel <= MaxEffectiveDistance)
        {
            RealDamage = BaseDamagePerHit;
        }
        if (DistanceTravel > MaxEffectiveDistance && DistanceTravel < MaximumDistance)
        {
            RealDamage = (0.5f + (MaximumDistance - DistanceTravel) / (2*(MaximumDistance - MaxEffectiveDistance))) * BaseDamagePerHit;
            Color c = GetComponent<SpriteRenderer>().color;
            c.a = (MaximumDistance - DistanceTravel) / (MaximumDistance - MaxEffectiveDistance);
            GetComponent<SpriteRenderer>().color = c;
        }
        if (DistanceTravel >= MaximumDistance)
        {
            RealDamage = 0;
            Destroy(gameObject);
        }
    }
    public void CheckInsideBlackhole()
    {
        isBHPulled = false;
        PulledVector = new List<Vector2>();
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 1f, BlackholeLayer);
        if (cols.Length > 0)
        {
            foreach (var col in cols)
            {
                BlackHole bh = col.GetComponent<BlackHole>();
                if (bh != null)
                {
                    isBHPulled = true;
                    PulledVector.Add(bh.CalculatePullingVector(gameObject));
                }
            }
        }
    }
    public void CalculateVelocity(Vector2 veloc)
    {
        if (isBHPulled)
        {
            foreach (Vector2 v in PulledVector)
            {
                veloc += v;
            }
        }
        GetComponent<Rigidbody2D>().velocity = veloc;
    }
    #endregion
}
