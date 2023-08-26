using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShared : MonoBehaviour
{
    #region Shared Variables
    public Rigidbody2D rb;
    public Vector2 Destination;
    public float Speed;
    private float Distance;
    private Vector2 Velocity;
    private Vector2 RealVelocity;
    public float BaseDamagePerHit;
    private float RealDamage;
    public LayerMask EnemyLayer;
    public string Type;
    public float DistanceTravel;
    #endregion
    #region Shared Functions
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
            rb.velocity = RealVelocity;
            yield return new WaitForSeconds(time/10f);
        }
    }

    public void CalculateDamage()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 0.01f, EnemyLayer);
        foreach (var col in cols)
        {
            EnemyShared enemy = col.gameObject.GetComponent<EnemyShared>();
            if (enemy!=null)
            {
                enemy.CurrentHP -= RealDamage;
                Debug.Log(RealDamage);
                Destroy(gameObject);
            }
        }
    }

    public void CheckDistanceTravel(float EffectiveDistance, float MaxDistance)
    {
        if (DistanceTravel <= EffectiveDistance)
        {
            RealDamage = BaseDamagePerHit;
        }
        if (DistanceTravel > EffectiveDistance && DistanceTravel < MaxDistance)
        {
            RealDamage = (0.5f + (MaxDistance - DistanceTravel)/(2*(MaxDistance - EffectiveDistance))) * BaseDamagePerHit;
            Color c = GetComponent<SpriteRenderer>().color;
            c.a = 0.5f + (MaxDistance - DistanceTravel) / 2 * (MaxDistance - EffectiveDistance);
            GetComponent<SpriteRenderer>().color = c;
        }
        if (DistanceTravel >= MaxDistance)
        {
            RealDamage = 0;
            Destroy(gameObject);
        }
    }
    #endregion
}
