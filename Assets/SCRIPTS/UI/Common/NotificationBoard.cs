using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationBoard : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    #endregion
    #region NormalVariables
    public int DisableCollider;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        DisableCollider = 1;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    private void LateUpdate()
    {
        // Call in late update after all has updated: if there is any other objects
        // call it to disable -> disable
        if (DisableCollider == 0)
        {
            GetComponent<Collider2D>().enabled = false;
        }
        else
        {
            GetComponent<Collider2D>().enabled = true;
        }
        DisableCollider = 1;
    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    #endregion
}
