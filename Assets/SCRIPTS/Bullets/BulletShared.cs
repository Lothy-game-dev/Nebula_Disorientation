using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShared : MonoBehaviour
{
    #region Shared Variables
    public Rigidbody2D rb;
    public Vector2 Destination;
    public float Speed;
    public float Distance;
    public Vector2 Velocity;
    public Vector2 RealVelocity;
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
    // Acceleration for the first 1 second
    public IEnumerator Accelerate()
    {
        for (int i=0; i<10; i++)
        {
            if (Mathf.Abs(RealVelocity.x) < Mathf.Abs(Velocity.x) && Mathf.Abs(RealVelocity.y) < Mathf.Abs(Velocity.y))
            {
                RealVelocity = new Vector2(RealVelocity.x + Velocity.x / 10, RealVelocity.y + Velocity.y / 10);
            }
            rb.velocity = RealVelocity;
            yield return new WaitForSeconds(0.1f);
        }
    }
    #endregion
}
