using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NanoCoat : Consumable
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public LayerMask EffectLayer;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    private LayerMask InitLayer;
    private bool isStart;
    private float Timer;
    private Color FighterColor;
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
        if (isStart)
        {
            if (Timer >= Duration)
            {
                isStart = false;
                Timer = 0;
                Fighter.layer = InitLayer;
                Fighter.GetComponent<SpriteRenderer>().color = FighterColor;
            }
            Timer += Time.deltaTime;
        }
    }
    #endregion
    #region Activate cons
    // Group all function that serve the same algorithm
    public void NanoCoatActivated()
    {
        FighterColor = Fighter.GetComponent<SpriteRenderer>().color;
        InitLayer = Fighter.layer;
        isStart = true;
        Fighter.layer = LayerMask.NameToLayer("Untargetable");
        Color c = Fighter.GetComponent<SpriteRenderer>().color;
        c.a = 0.5f;
        Fighter.GetComponent<SpriteRenderer>().color = c;
    }
    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    #endregion
}
