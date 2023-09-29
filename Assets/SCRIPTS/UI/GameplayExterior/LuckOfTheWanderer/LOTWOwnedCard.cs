using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOTWOwnedCard : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject NameText;
    public GameObject EffectText;
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
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Mouse check
    private void OnMouseEnter()
    {
        NameText.SetActive(false);
        EffectText.SetActive(true);
    }

    private void OnMouseExit()
    {
        NameText.SetActive(true);
        EffectText.SetActive(false);
    }
    #endregion
}
