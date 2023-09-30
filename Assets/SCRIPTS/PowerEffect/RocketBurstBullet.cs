using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RocketBurstBullet : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    private Rigidbody2D rb;
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public float RkAngle;
    public LayerMask Layer;
    public float Damage;
    private List<float> DistanceList;
    public float Distance;
    public float DistanceTravel;
    private bool isFound;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        rb = GetComponent<Rigidbody2D>();
        isFound = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        DistanceTravel += Time.deltaTime * rb.velocity.magnitude;

        if (DistanceTravel > 100)
        {
            CheckRange();
        }
        CheckDistanceTravel();
        
    }
    #endregion
    #region Check range
    // Group all function that serve the same algorithm
    public void CheckRange()
    {
        float distance = 0;
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 300f, Layer);
        foreach (var col in cols)
        {
            
            EnemyShared enemy = col.gameObject.GetComponent<EnemyShared>();
            if (enemy != null)
            {
                distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance != 0 && !isFound)
                {
                    Debug.Log("hihi");
                    isFound = true;
                    
                }
            }          
           
        }
    }

    #endregion
    #region Check Distance
    // Group all function that serve the same algorithm
    private void CheckDistanceTravel()
    {
        if (DistanceTravel > Distance)
        {
            Destroy(gameObject);
        }
    }
    #endregion
}
