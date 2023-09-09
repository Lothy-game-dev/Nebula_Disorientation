using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UECEnterSession : MonoBehaviour
{
    #region ComponentVariables
    private Animator anim;
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Mouse Check
    private void OnMouseEnter()
    {
        anim.ResetTrigger("Stop");
        anim.SetTrigger("Move");
    }

    private void OnMouseExit()
    {
        anim.ResetTrigger("Move");
        anim.SetTrigger("Stop");
    }
    #endregion
}
