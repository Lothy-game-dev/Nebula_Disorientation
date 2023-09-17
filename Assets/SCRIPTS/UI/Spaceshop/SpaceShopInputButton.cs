using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpaceShopInputButton : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public TMP_InputField InputField;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    #endregion
    #region Start & Update
    private float inputDelay;
    private float fastInputTimer;
    private float ResetScaleTimer;
    private int InputScale;
    void OnEnable()
    {
        // Initialize variables
        InputScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        bool check = false;
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.name == name)
            {
                check = true;
            }
        }
        if (check)
        {
            if (Input.GetMouseButton(0))
            {
                if (inputDelay<=0f)
                {
                    InputField.text = (int.Parse(InputField.text) + (name=="Decrease"?-1:1)
                         * InputScale).ToString();
                    inputDelay = 0.2f;
                } else
                {
                    inputDelay -= Time.deltaTime;
                }
                if (InputScale<10) fastInputTimer+=Time.deltaTime;
                if (fastInputTimer>2f)
                {
                    InputScale = 10;
                    fastInputTimer = 0f;
                }
            } else
            {
                InputScale = 1;
                fastInputTimer = 0f;
            }
        } else
        {
            InputScale = 1;
            fastInputTimer = 0f;
        }
    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    #endregion
}
