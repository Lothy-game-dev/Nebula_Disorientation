using System.Collections;
using System.Collections.Generic;
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
    public GameObject LeftStartPos;
    public GameObject RightStartPos;
    public GameObject Top;
    public GameObject Bottom;
    public GameObject ModelList;
    public GameObject Contents;
    
    #endregion
    #region NormalVariables
    private float Size;
    private bool onLeft;
    private float maximumHeight;
    private List<GameObject> ListOfModel;
    private List<GameObject> ListBoxAfterGenerated;
    private GameObject CurrentSelectedModel;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        onLeft = true;
        Size = Top.transform.position.y - Bottom.transform.position.y;
        List<string> listModel = new List<string>();
        listModel.Add("SS29-MK1");
        listModel.Add("SSS-MK1");
        listModel.Add("UEC29-MK1");
        listModel.Add("ND-Zartillery");
        SetItems(listModel, "UEC29-MK1");
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
        if (!listModel.Contains(currentModel))
        {
            // Error MSG
            return;
        }
        for (int i=0;i<listModel.Count;i++)
        {
            for (int j=0;j<ModelList.transform.childCount;j++)
            {
                if (ModelList.transform.GetChild(0).name.Replace(" ", "").ToLower()
                    .Equals(listModel[i].Replace(" ","").ToLower())) {
                    ListOfModel.Add(ModelList.transform.GetChild(0).gameObject);
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
        maximumHeight = Size * (ListOfModel.Count % 2 == 0 ? listModel.Count / 2 : listModel.Count / 2 + 1);
        Contents.GetComponent<RectTransform>().sizeDelta
            = new Vector2(Contents.GetComponent<RectTransform>().sizeDelta.x,
            maximumHeight);
        // Add data to scene controller
        Scene.GetComponent<LoadoutScene>().Model = currentModel;
        
    }

    private void GenerateItems()
    {
        Vector2 currentPosLeft = LeftStartPos.transform.position;
        Vector2 currentPosRight = RightStartPos.transform.position;
        for (int i=0;i< ListOfModel.Count;i++)
        {
            if (onLeft)
            {
                onLeft = false;
                GameObject go = Instantiate(BoxTemplate, new Vector3(currentPosLeft.x,
                    currentPosLeft.y, BoxTemplate.transform.position.z), Quaternion.identity);
                currentPosLeft = new Vector2(currentPosLeft.x, currentPosLeft.y - Size);
                go.transform.SetParent(Contents.transform);
                go.transform.GetChild(0).GetComponent<Image>().sprite = ListOfModel[i].GetComponent<SpriteRenderer>().sprite;
                go.SetActive(true);
            } else
            {
                onLeft = true;
                GameObject go = Instantiate(BoxTemplate, new Vector3(currentPosRight.x,
                    currentPosRight.y, BoxTemplate.transform.position.z), Quaternion.identity);
                currentPosRight = new Vector2(currentPosRight.x, currentPosRight.y - Size);
                go.transform.SetParent(Contents.transform);
                go.transform.GetChild(0).GetComponent<Image>().sprite = ListOfModel[i].GetComponent<SpriteRenderer>().sprite;
                go.SetActive(true);
            }
        }
    }
    #endregion
}
