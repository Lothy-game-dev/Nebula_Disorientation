using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDFighterInfoEffects : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject FirePlace;
    public GameObject Fire;
    public GameObject FreezePlace;
    public GameObject Freeze;
    public GameObject DistanceCheck;
    #endregion
    #region NormalVariables
    public float CurrentTemp;
    private float Distance;
    private float DelaySpawn;
    private List<GameObject> ListEffect;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        Distance = Mathf.Abs((FirePlace.transform.position - DistanceCheck.transform.position).magnitude);
        ListEffect = new List<GameObject>();
        CurrentTemp = 50f;
    }

    // Update is called once per frame
    void Update()
    {
        DelaySpawn -= Time.deltaTime;
        CheckTemp();
    }
    #endregion
    #region Check temp to show UI
    private void CheckTemp()
    {
        if (DelaySpawn<=0f)
        {
            if (CurrentTemp>=90)
            {
                SpawnItem(true, 3);
            } 
            else if (CurrentTemp>=75)
            {
                SpawnItem(true, 2);
            } 
            else if (CurrentTemp>=60)
            {
                SpawnItem(true, 1);
            } 
            else if (CurrentTemp>=40)
            {
                foreach (var eff in ListEffect)
                {
                    if (eff!=null)
                    {
                        Destroy(eff);
                    }
                }
                ListEffect = new List<GameObject>();
            } 
            else if (CurrentTemp>=20)
            {
                SpawnItem(false, 1);
            }  
            else if (CurrentTemp>0)
            {
                SpawnItem(false, 2);
            }
            else if(CurrentTemp == 0)
            {
                SpawnItem(false, 3);
            }
        }
    }

    private void SpawnItem(bool isFire, int count)
    {
        if (isFire)
            DelaySpawn = 1f;
        else
            DelaySpawn = 2f;
        for (int i=0;i<count;i++)
        {
            float x = Random.Range(-Distance, Distance);
            float LimitY = Mathf.Sqrt(Distance * Distance - x * x);
            float y = Random.Range(-LimitY, LimitY);
            if (isFire)
            {
                GameObject fireSp = Instantiate(Fire, new Vector3(FirePlace.transform.position.x + x, FirePlace.transform.position.y + y, Fire.transform.position.z), Quaternion.identity);
                fireSp.transform.SetParent(FirePlace.transform);
                fireSp.transform.localScale = Fire.transform.localScale;
                ListEffect.Add(fireSp);
                fireSp.SetActive(true);
                Destroy(fireSp, 1f);
            } else
            {
                GameObject freezeSp = Instantiate(Freeze, new Vector3(FreezePlace.transform.position.x + x, FreezePlace.transform.position.y + y, Freeze.transform.position.z), Quaternion.identity);
                freezeSp.transform.SetParent(FreezePlace.transform);
                freezeSp.transform.localScale = Freeze.transform.localScale;
                ListEffect.Add(freezeSp);
                freezeSp.SetActive(true);
                Destroy(freezeSp, 2f);
            }
        }
    }
    #endregion
}
