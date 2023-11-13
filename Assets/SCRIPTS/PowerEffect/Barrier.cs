using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : Powers
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject EndEffect;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    private GameObject Shield;
    private float Timer;
    private bool isStart;
    private bool isEnding;
    private float IncreaseCurrentBarrierAmount;
    private float IncreaseMaxBarrierAmount;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        Timer = 0f;
        isEnding = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (isStart)
        {
            Timer += Time.deltaTime;
            if (isEnding && Timer >= Duration * 3f/4)
            {
                Destroy(Shield);
                isEnding = false;
                GenEndBarrier();
            }
            if (Timer >= Duration)
            {
                if (BRx != 0)
                {
                    if (Fighter.GetComponent<FighterShared>().CurrentBarrier > Fighter.GetComponent<FighterShared>().MaxBarrier - IncreaseMaxBarrierAmount)
                    {
                        Fighter.GetComponent<FighterShared>().CurrentBarrier = Fighter.GetComponent<FighterShared>().MaxBarrier - IncreaseMaxBarrierAmount;
                    }
                    else
                    {
                        if (Fighter.GetComponent<FighterShared>().CurrentBarrier>=IncreaseMaxBarrierAmount)
                        {
                            Fighter.GetComponent<FighterShared>().CurrentBarrier -= IncreaseMaxBarrierAmount;
                            if (Fighter.GetComponent<FighterShared>().CurrentBarrier > Fighter.GetComponent<FighterShared>().MaxBarrier)
                            {
                                Fighter.GetComponent<FighterShared>().CurrentBarrier = Fighter.GetComponent<FighterShared>().MaxBarrier;
                            }
                        } else
                        {
                            Fighter.GetComponent<FighterShared>().CurrentBarrier = 0;
                        }                    
                    }
                    Fighter.GetComponent<FighterShared>().MaxBarrier -= IncreaseMaxBarrierAmount;
                }
                Destroy(Shield);
                Timer = 0f;
                isStart = false;

            }
            if (Fighter.GetComponent<FighterShared>().CurrentBarrier == 0)
            {
                if (Shield != null)
                {
                    Destroy(Shield);
                }
            }
        }
        
    }
    #endregion
    #region Gen barrier
    // Group all function that serve the same algorithm
    public void GenBarrier()
    {
        PlaySound(SoundEffect);
        isStart = true;
        Shield = Instantiate(Effect, Fighter.transform.position, Quaternion.identity);
        Shield.transform.SetParent(Fighter.transform);
        Shield.SetActive(true);
        if (BRx != 0)
        {

            //current amount / max amount will be added
            IncreaseCurrentBarrierAmount = (Fighter.GetComponent<FighterShared>().CurrentBarrier * BRx) - Fighter.GetComponent<FighterShared>().CurrentBarrier;
            IncreaseMaxBarrierAmount = (Fighter.GetComponent<FighterShared>().MaxBarrier * BRx) - Fighter.GetComponent<FighterShared>().MaxBarrier;

            Fighter.GetComponent<FighterShared>().MaxBarrier *= BRx;
            Fighter.GetComponent<FighterShared>().CurrentBarrier += IncreaseMaxBarrierAmount;
        }
        
    }
    #endregion
    #region Function group ...
    public void GenEndBarrier()
    {
        Shield = Instantiate(EndEffect, Fighter.transform.position, Quaternion.identity);
        Shield.transform.SetParent(Fighter.transform);
        Shield.SetActive(true);
    }
    #endregion
}
