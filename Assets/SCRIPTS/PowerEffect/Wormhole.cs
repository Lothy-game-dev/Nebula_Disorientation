using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wormhole : Powers
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject Effect;  
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    private Vector2 pos;
    private Vector3 newPos;
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
    #region Teleport
    // Group all function that serve the same algorithm
    public void GenerateWormhole()
    {
        pos = CalculatePos(Range);
        if (pos.x >= 5000f || pos.x <= -5000f || pos.y >= 5000f || pos.y <= -5000f)
        {           
            GameObject TpEffect = Instantiate(Effect, Fighter.transform.position, Quaternion.identity);
            TpEffect.SetActive(true);
            Fighter.transform.position = Fighter.transform.position;
            Destroy(TpEffect, 1f);
        } else
        {
            newPos = new Vector3(Fighter.transform.position.x + pos.x, Fighter.transform.position.y + pos.y, Fighter.transform.position.z);
            WormholeSound();
            StartCoroutine(StartTeleport());
        }
    }
    #endregion
    
    #region Animation
    IEnumerator StartTeleport()
    {
        GameObject TpEffectTo = Instantiate(Effect, newPos, Quaternion.identity);
        TpEffectTo.GetComponent<Animator>().speed = 2;
        TpEffectTo.SetActive(true);
        yield return new WaitForSeconds(0.25f);           
 
        
        GameObject TpEffectFrom = Instantiate(Effect, Fighter.transform.position, Quaternion.identity);
        TpEffectFrom.GetComponent<Animator>().speed = 2;
        TpEffectFrom.SetActive(true);
        yield return new WaitForSeconds(0.25f);


        Destroy(TpEffectFrom, 0.2f);
        
        Fighter.transform.position = newPos;
        Destroy(TpEffectTo, 0.2f);
        EndSound();
        
    }
    #endregion
    #region Sound Eff
    
    #endregion
}
