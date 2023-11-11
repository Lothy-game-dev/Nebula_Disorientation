using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDPopUp : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Popup;
    #endregion
    #region NormalVariables
    private float showTimer;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        showTimer -= Time.deltaTime;
        if (showTimer<=0f)
        {
            Popup.SetActive(false);
        }
    }
    #endregion
    #region mouse check
    private void OnMouseEnter()
    {
        Popup.SetActive(true);
        showTimer = 1f;
    }
    private void OnMouseOver()
    {
        showTimer = 1f;
    }

    private void OnDisable()
    {
        Popup.SetActive(false);
    }
    #endregion
}
