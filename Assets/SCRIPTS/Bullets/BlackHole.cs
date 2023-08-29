using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public float BasePullingForce;
    public GameObject RangeCheck;
    public GameObject CenterRange;
    #endregion
    #region NormalVariables
    public Vector2 PullingVector;
    private float radius;
    private float centerRadius;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        radius = Mathf.Abs((transform.position - RangeCheck.transform.position).magnitude);
        centerRadius = Mathf.Abs((transform.position - CenterRange.transform.position).magnitude);
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        transform.Rotate(new Vector3(0, 0, 1));
    }
    #endregion
    #region Pulling Vector Cal
    public Vector2 CalculatePullingVector(GameObject go)
    {
        float distance = Mathf.Abs((go.transform.position - transform.position).magnitude);
        if (distance > radius)
        {
            return new Vector2(0, 0);
        } 
        else if (distance <= centerRadius)
        {
            Vector2 vectorDis = transform.position - go.transform.position;
            float ForceX = BasePullingForce * vectorDis.x / Mathf.Sqrt(vectorDis.x * vectorDis.x + vectorDis.y * vectorDis.y);
            float ForceY = BasePullingForce * vectorDis.y / Mathf.Sqrt(vectorDis.x * vectorDis.x + vectorDis.y * vectorDis.y);
            return new Vector2(ForceX, ForceY);
        }
        else
        {
            Vector2 vectorDis = transform.position - go.transform.position;
            float pullForceCal = (radius - distance / 2) / radius * BasePullingForce;
            float ForceX = pullForceCal * vectorDis.x / Mathf.Sqrt(vectorDis.x * vectorDis.x + vectorDis.y * vectorDis.y);
            float ForceY = pullForceCal * vectorDis.y / Mathf.Sqrt(vectorDis.x * vectorDis.x + vectorDis.y * vectorDis.y);
            return new Vector2(ForceX, ForceY);
        }
    }
    #endregion
}
