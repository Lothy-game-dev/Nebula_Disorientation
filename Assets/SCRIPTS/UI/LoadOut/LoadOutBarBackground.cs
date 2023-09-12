using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadOutBarBackground : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Bar;
    #endregion
    #region NormalVariables
    public int DisableCollider;
    private bool alreadyClick;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        DisableCollider = 1;
    }

    private void OnEnable()
    {
        alreadyClick = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    private void LateUpdate()
    {
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
    #region Mouse Check
    private void OnMouseDown()
    {
        if (!alreadyClick)
        {
            alreadyClick = true;
            if (Bar.GetComponent<LoadOutBar>() != null)
            {
                Bar.GetComponent<LoadOutBar>().BackgroundMouseDown();
            }
        }
    }
    #endregion
}
