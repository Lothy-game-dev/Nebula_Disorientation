using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UECSessionBlackBackground : MonoBehaviour
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
        // If click on background close the bar
        if (!alreadyClick)
        {
            alreadyClick = true;
            if (Bar.GetComponent<UECSessionWeaponBox>() != null)
            {
                FindObjectOfType<SoundSFXGeneratorController>().GenerateSound("LoadoutWeapon");
                Bar.GetComponent<UECSessionWeaponBox>().BackgroundMouseDown();
            }
            else if (Bar.GetComponent<UECSessionPowerBox>() != null)
            {
                Bar.GetComponent<UECSessionPowerBox>().BackgroundMouseDown();
            }
            else if (Bar.GetComponent<UECSessionConsumableBox>() != null)
            {
                FindObjectOfType<SoundSFXGeneratorController>().GenerateSound("LoadoutCons");
                Bar.GetComponent<UECSessionConsumableBox>().BackgroundMouseDown();
            }
        }
    }
    #endregion
}
