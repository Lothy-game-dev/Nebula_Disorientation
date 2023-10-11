using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceZoneHazardEnvironment : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject[] HazardRocks;
    public GameObject HazardStar;
    public GameplayInteriorController ControllerMain;
    #endregion
    #region NormalVariables
    public int HazardID;
    public float HazardThermalBurnDmgScale;
    public float HazardOverheat;
    public float HazardGammaRayBurstScale;
    public bool StartSpawningStar;
    public float DelaySpawnStar;
    private float SpawnStarDelay;
    public float HazardNDAllDamageScale;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        InitializeHazard();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (StartSpawningStar && !ControllerMain.IsInLoading)
        {
            SpawnStarDelay -= Time.deltaTime;
            if (SpawnStarDelay <= 0f)
            {
                SpawnStarDelay = DelaySpawnStar;
                float x = Random.Range(0,3);
                float y = 0;
                if (x == 0)
                {
                    x = Random.Range(-5200f, -5000);
                    y = Random.Range(-5200f, 5200);
                } else if (x==1)
                {
                    x = Random.Range(-5200f, 5200);
                    y = Random.Range(0, 2);
                    if (y==0)
                    {
                        y = Random.Range(-5200f, -5000);
                    } else
                    {
                        y = Random.Range(5000, 5200f);
                    }
                } else
                {
                    x = Random.Range(5000f, 5200);
                    y = Random.Range(-5200f, 5200);
                }
                GameObject Star = Instantiate(HazardStar, new Vector2(x, y), Quaternion.identity);
                float xDest = 0;
                float yDest = 0;
                if (x < -5000 || x > 5000)
                {
                    xDest = -x;
                    yDest = Random.Range(-5200, 5200);
                } else
                {
                    yDest = -y;
                    xDest = Random.Range(-5200, 5200);
                }
                Star.SetActive(true);
                Star.GetComponent<SpaceZoneStar>().InitializeStar(new Vector2(xDest, yDest), 500f);
            }
        }
    }
    #endregion
    #region Spawn Hazard
    public void InitializeHazard()
    {
        HazardThermalBurnDmgScale = 1;
        HazardOverheat = 1;
        HazardGammaRayBurstScale = 1;
        StartSpawningStar = false;
        HazardNDAllDamageScale = 1;
        DelaySpawnStar = 0;
        if (HazardID==2)
        {
            int n = Random.Range(10, 21);
            for (int i=0;i<n;i++)
            {
                int k = Random.Range(0, 2) == 0 ? 0 : 4;
                GameObject ChosenRock = HazardRocks[k];
                GameObject Rock = Instantiate(ChosenRock, new Vector2(Random.Range(-4500f, 4500f), Random.Range(-4500f, 4500f)), Quaternion.identity);
                Rock.name = "SR" + i;
                Rock.SetActive(true);
            }
        } else if (HazardID==3)
        {
            HazardThermalBurnDmgScale = 2;
            HazardOverheat = 2;
        } else if (HazardID==4)
        {
            HazardGammaRayBurstScale = 2;
        } else if (HazardID==5)
        {
            DelaySpawnStar = 10f;
            StartSpawningStar = true;
        } else if (HazardID==6)
        {
            DelaySpawnStar = 10f;
            StartSpawningStar = true;
            HazardNDAllDamageScale = 2;
            int n = Random.Range(10, 21);
            for (int i = 0; i < n; i++)
            {
                int k = Random.Range(0, 2) == 0 ? 0 : 4;
                GameObject ChosenRock = HazardRocks[k];
                GameObject Rock = Instantiate(ChosenRock, new Vector2(Random.Range(-4500f, 4500f), Random.Range(-4500f, 4500f)), Quaternion.identity);
                Rock.name = "SR" + i;
                Rock.SetActive(true);
            }
        }
    }
    #endregion
}
