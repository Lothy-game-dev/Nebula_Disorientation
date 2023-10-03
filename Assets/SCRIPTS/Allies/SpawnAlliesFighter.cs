using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAlliesFighter : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject AllyModel;
    public GameObject AllyTemplate;
    #endregion
    #region NormalVariables
    // will do delay later
    public List<GameObject> Allies;
    private Dictionary<int, float> AllySpawnDelay;
    private Vector2[] AllySpawnPosition;
    private int[] AllySpawnID;
    private GameObject ChosenModel;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        AllySpawnID = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 14/*1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15*/};
        AllySpawnPosition = new Vector2[] {
        new Vector2(-2500,0),
        new Vector2(-1500,0), new Vector2(-3500,0), new Vector2(-2500,1000),new Vector2(-2500,-1000),
        new Vector2(-1500,1000), new Vector2(-1500,-1000), new Vector2(-3500,1000),new Vector2(-3500,-1000),
        new Vector2(-500,0), new Vector2(-4500,0), new Vector2(-2500,2000),new Vector2(-2500,-2000),
        new Vector2(-4500,2000), new Vector2(-4500,-2000)};
        AllySpawnDelay = new Dictionary<int, float>();
        AllySpawnDelay.Add(1, 5f);
        AllySpawnDelay.Add(2, 10f);
        AllySpawnDelay.Add(3, 5f);
        AllySpawnDelay.Add(4, 10f);
        Allies = new List<GameObject>();
        for (int i = 0; i < AllySpawnID.Length; i++)
        {
            CreateEnemy(AllySpawnID[i], AllySpawnPosition[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Spawn Enemy
    private void CreateEnemy(int id, Vector2 spawnPos)
    {
        Dictionary<string, object> DataDict = FindObjectOfType<AccessDatabase>().GetDataAlliesById(id);
        // Get Model
        for (int i = 0; i < AllyModel.transform.childCount; i++)
        {
            if (AllyModel.transform.GetChild(i).name.Replace(" ", "").ToLower().Equals(((string)DataDict["Name"]).Replace(" ", "").ToLower()))
            {
                ChosenModel = AllyModel.transform.GetChild(i).gameObject;
            }
        }
        GameObject Ally = Instantiate(AllyTemplate, new Vector3(spawnPos.x, spawnPos.y, AllyTemplate.transform.position.z), Quaternion.identity);
        Allies.Add(Ally);
        Ally.name = ChosenModel.name + " |" + spawnPos.x + " - " + spawnPos.y;
        Ally.GetComponent<SpriteRenderer>().sprite = ChosenModel.GetComponent<SpriteRenderer>().sprite;
        Ally.GetComponent<AlliesShared>().InitData(DataDict, ChosenModel);
    }

    public IEnumerator SpawnAlly()
    {
        yield return null;
    }
    #endregion
}
