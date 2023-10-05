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
    public float[] AllySpawnDelay;
    public Vector2[] AllySpawnPosition;
    public int[] AllySpawnID;
    private GameObject ChosenModel;
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
    #region Spawn Enemy
    public void SpawnAlly()
    {
        Allies = new List<GameObject>();
        for (int i = 0; i < AllySpawnID.Length; i++)
        {
            CreateAlly(AllySpawnID[i], AllySpawnPosition[i], i);
        }
    }
    private void CreateAlly(int id, Vector2 spawnPos, int count)
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
        if (id >= 13)
        {
            Ally.tag = "AlliesEliteFighter";
        }
        Ally.name = ChosenModel.name + " |" + spawnPos.x + " - " + spawnPos.y + " - " + count;
        Ally.GetComponent<SpriteRenderer>().sprite = ChosenModel.GetComponent<SpriteRenderer>().sprite;
        Ally.GetComponent<AlliesShared>().InitData(DataDict, ChosenModel);
    }
    #endregion
}
