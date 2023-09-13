using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadOutConsumableBox : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables

    #endregion
    #region NormalVariables
    public GameObject PopUp;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region mouse check
    private void OnMouseDown()
    {
        PopUp.GetComponent<LoadOutConsumablePopUp>().ShowClickItem(gameObject);
        PopUp.GetComponent<LoadOutConsumablePopUp>().CheckSetClickItem(gameObject);
    }
    #endregion
}
