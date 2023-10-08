using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitRendering : MonoBehaviour
{
    #region ComponentVariables
    private Camera MainCamera;
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    #endregion
    #region NormalVariables
    public bool isNotRendering;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        MainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        /*Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(MainCamera);

        if (GeometryUtility.TestPlanesAABB(frustumPlanes, GetComponent<Renderer>().bounds) == false)
        {
            // Enemy is outside the camera's frustum, disable rendering.
            GetComponent<Renderer>().enabled = false;
            isNotRendering = true;
        }
        else
        {
            // Enemy is inside the camera's frustum, enable rendering.
            GetComponent<Renderer>().enabled = true;
            isNotRendering = false;
        }*/
    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    #endregion
}
