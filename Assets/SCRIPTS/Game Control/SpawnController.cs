using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject[] EnemyTest;
    #endregion
    #region NormalVariables
    private float SpawnTimer;
    private int SpawnCount;
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
        if (SpawnTimer<=0f)
        {
            int spawn = Random.Range(0, 3);
            float spawnX = Random.Range(-4000f, 4000f);
            float spawnY = Random.Range(-4000f, 4000f);
            GameObject test = Instantiate(EnemyTest[spawn], new Vector2(spawnX, spawnY), Quaternion.identity);
            SpawnCount++;
            test.name += SpawnCount;
            SpawnTimer = 5f;
        } else
        {
            SpawnTimer -= Time.deltaTime;
        }
    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    #endregion
}
