using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadOutPowerPopUpBox : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public LoadOutPowerPopUp popup;
    #endregion
    #region NormalVariables
    public bool isSelected;
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
    #region Mouse Check
    private void OnMouseDown()
    {
        if (!isSelected)
        {
            isSelected = true;
            popup.ChooseItem(gameObject);
        } else
        {
            isSelected = false;
            popup.RemoveItem();
        }
    }
    #endregion
}
