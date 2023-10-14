using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WSSSDetected : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public LayerMask EnemyLayer;
    public LayerMask AlliesLayer;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public Dictionary<float, GameObject> WSSSDict;
    public List<string> WSSSName;
    public List<float> WSSSDistance;
    public List<GameObject> WSSS;
    public Dictionary<GameObject, int> PrioritizeDict;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        WSSSDict = new Dictionary<float, GameObject>();
        PrioritizeDict = new Dictionary<GameObject, int>();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Detect WSSS
    // Group all function that serve the same algorithm
    public void DectectWSSS()
    {
        // Get all Warship and SpaceStation
        Collider2D[] cols1 = Physics2D.OverlapCircleAll(transform.position, 10000f, EnemyLayer);
        Collider2D[] cols2 = Physics2D.OverlapCircleAll(transform.position, 10000f, AlliesLayer);

        foreach (var obj1 in cols1)
        {
            if (obj1.tag == "BossEnemy")
            {
                float distanceToCamera = Vector3.Distance(obj1.transform.position, Camera.main.transform.position);
                if (!WSSSDict.ContainsKey(distanceToCamera))
                {
                    WSSSDict.Add(distanceToCamera, obj1.gameObject);
                    WSSSDistance.Add(distanceToCamera);                                  
                }
            }
        }

        foreach (var obj2 in cols2)
        {
            if (obj2.tag == "AlliesBossFighter" || obj2.tag == "AlliesEliteFighter")
            {
                float distanceToCamera = Vector3.Distance(obj2.transform.position, Camera.main.transform.position);
                if (!WSSSDict.ContainsKey(distanceToCamera))
                {
                    WSSSDict.Add(distanceToCamera, obj2.gameObject);
                    WSSSDistance.Add(distanceToCamera);
                }

            }
        }
        WSSSDistance.Sort();
        for (int i = 0; i < WSSSDistance.Count; i++)
        {
            if (!PrioritizeDict.ContainsKey(WSSSDict[WSSSDistance[i]]))
            {
                PrioritizeDict.Add(WSSSDict[WSSSDistance[i]], i);
            }
        }
    }
    #endregion
   
}
