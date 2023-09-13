using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadOutModelBoard : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Scene;
    public GameObject BoxTemplate;
    public GameObject StartPos;
    public GameObject Top;
    public GameObject Bottom;
    public GameObject ModelList;
    public GameObject Contents;
    public GameObject FighterDemo;
    #endregion
    #region NormalVariables
    private float Size;
    private float maximumHeight;
    private List<string> ListModelName;
    private List<GameObject> ListOfModel;
    private GameObject CurrentSelectedModel;
    private List<GameObject> ListOfModelAfterGen;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        Size = Mathf.Abs(Top.transform.position.y - Bottom.transform.position.y);
        List<string> listModel = new List<string>();
        listModel.Add("SS29-MK1");
        listModel.Add("SSS-MK1");
        listModel.Add("UEC29-MK1");
        listModel.Add("ND-Zartillery");
        SetItems(listModel, "SSS-MK1");
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Set Items
    public void SetItems(List<string> listModel, string currentModel)
    {
        ListOfModel = new List<GameObject>();
        ListModelName = new List<string>();
        ListOfModelAfterGen = new List<GameObject>();
        if (!listModel.Contains(currentModel))
        {
            // Error MSG
            return;
        }
        ListModelName = listModel;
        for (int i=0;i<listModel.Count;i++)
        {
            for (int j=0;j<ModelList.transform.childCount;j++)
            {
                if (ModelList.transform.GetChild(j).name.Replace(" ", "").ToLower()
                    .Equals(listModel[i].Replace(" ","").ToLower())) {
                    ListOfModel.Add(ModelList.transform.GetChild(j).gameObject);
                    break;
                }
            }
        }
        foreach (var model in ListOfModel)
        {
            if (model.name.Replace(" ", "").ToLower().Equals(currentModel.Replace(" ", "").ToLower())) {
                CurrentSelectedModel = model;
            }
        }
        maximumHeight = Size * (ListOfModel.Count - 1);
        Contents.GetComponent<RectTransform>().sizeDelta
            = new Vector2(Contents.GetComponent<RectTransform>().sizeDelta.x,
            maximumHeight);
        // Add data to scene controller
        Scene.GetComponent<LoadoutScene>().Model = currentModel;
        StartCoroutine(GenerateItems(currentModel));
    }

    private IEnumerator GenerateItems(string currentModel)
    {
        Vector2 currentPosLeft = StartPos.transform.position;
        for (int i=0;i< ListOfModel.Count;i++)
        {
            GameObject go = Instantiate(BoxTemplate, new Vector3(currentPosLeft.x,
                currentPosLeft.y, BoxTemplate.transform.position.z), Quaternion.identity);
            currentPosLeft = new Vector2(currentPosLeft.x, currentPosLeft.y - Size);
            go.transform.SetParent(Contents.transform);
            go.name = ListOfModel[i].name;
            if (go.name.Replace(" ","").ToLower().Equals(currentModel.Replace(" ", "").ToLower()))
            {
                go.GetComponent<Image>().color = Color.green;
                CurrentSelectedModel = go;
            }
            ListOfModelAfterGen.Add(go);
            go.transform.localScale = new Vector2(BoxTemplate.transform.localScale.x, BoxTemplate.transform.localScale.y);
            go.GetComponent<LoadOutModelBox>().board = gameObject;
            go.transform.GetChild(0).GetComponent<Image>().sprite = ListOfModel[i].GetComponent<Image>().sprite;
            go.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = ListOfModel[i].name;
            go.SetActive(true);
            yield return new WaitForSeconds(0.05f);
        }
        FighterDemo.GetComponent<LoadOutFighterDemo>().SetModel(currentModel);
    }

    public void ResetModel(string ModelName)
    {
        if (!ListModelName.Contains(ModelName))
        {
            // Error MSG
            return;
        }
        foreach (var model in ListOfModelAfterGen)
        {
            if (model.name.Replace(" ","").ToLower().Equals(ModelName.Replace(" ","").ToLower()))
            {
                CurrentSelectedModel = model;
                model.GetComponent<Image>().color = Color.green;
            } else
            {
                model.GetComponent<Image>().color = Color.white;
            }
        }
        FighterDemo.GetComponent<LoadOutFighterDemo>().SetModel(ModelName);
        Scene.GetComponent<LoadoutScene>().Model = ModelName;
    }
    #endregion
}
