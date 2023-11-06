using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UECSessionWeaponBox : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Scene;
    public GameObject WeaponList;
    public GameObject Content;
    public GameObject ScrollView;
    public GameObject Template;
    public GameObject Background;
    public GameObject Left;
    public GameObject Right;
    public GameObject Top;
    public GameObject Bottom;
    public GameObject[] DisableColliders;
    public string Type;
    #endregion
    #region NormalVariables
    public string ChosenWeapon;
    public string OtherChosen;
    private List<string> ListWeapon;
    private List<GameObject> ListItem;
    private GameObject CurrentChosen;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        StartCoroutine(StartAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (!CheckIfMouseOutsideScrollRange())
        {
            Background.GetComponent<UECSessionBlackBackground>().DisableCollider *= 0;
        }
    }
    #endregion
    #region Generate Data
    // Group all function that serve the same algorithm
    public void GenerateData(string chosen, string other)
    {
        ScrollView.GetComponent<ScrollRect>().horizontal = false;
        ListItem = new List<GameObject>();
        ListWeapon = FindObjectOfType<AccessDatabase>().GetSessionOwnedWeaponExceptForName(PlayerPrefs.GetInt("PlayerID"), other);
        for (int i = 0; i < ListWeapon.Count; i++)
        {
            for (int j = 0; j < WeaponList.transform.childCount; j++)
            {
                if (WeaponList.transform.GetChild(j).name.Replace(" ", "").Replace("-", "").ToLower().Equals(ListWeapon[i].Replace(" ", "").Replace("-", "").ToLower()))
                {
                    GameObject Weap = Instantiate(Template, Template.transform.position, Quaternion.identity);
                    Weap.transform.SetParent(Content.transform);
                    Weap.transform.localScale = Template.transform.localScale;
                    Weap.transform.GetChild(0).GetComponent<Image>().sprite = WeaponList.transform.GetChild(j).GetChild(0).GetChild(0).GetComponent<Image>().sprite;
                    Weap.SetActive(true);
                    if (ListWeapon[i].Replace(" ","").Replace("-","").ToLower().Equals(chosen.Replace(" ", "").Replace("-", "").ToLower()))
                    {
                        Weap.GetComponent<Image>().color = Color.green;
                        CurrentChosen = Weap;
                    }
                    Weap.name = ListWeapon[i];
                    ListItem.Add(Weap);
                    break;
                }
            }
        }
        ScrollView.GetComponent<ScrollRect>().horizontal = true;
    }

    private IEnumerator StartAnimation()
    {
        foreach (var col in DisableColliders)
        {
            col.GetComponent<Collider2D>().enabled = false;
        }
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(false);
        Color c = GetComponent<SpriteRenderer>().color;
        c.a = 0;
        GetComponent<SpriteRenderer>().color = c;
        for (int i=0; i<10; i++)
        {
            Color c1 = GetComponent<SpriteRenderer>().color;
            c1.a += 1/10f;
            GetComponent<SpriteRenderer>().color = c1;
            yield return new WaitForSeconds(0.5f / 10f);
        }
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(true);
        transform.GetChild(3).gameObject.SetActive(true);
        GenerateData(ChosenWeapon, OtherChosen);
    }

    private void OnDisable()
    {
        int n = 0;
        while (n<ListItem.Count)
        {
            if (ListItem[n]!=null)
            {
                GameObject temp = ListItem[n];
                ListItem.Remove(temp);
                Destroy(temp);
            }
            else
            {
                n++;
            }
        }
    }

    public void ChooseItem(GameObject go)
    {
        if (ListItem.Contains(go))
        {
            for (int i=0; i< ListItem.Count;i++)
            {
                ListItem[i].GetComponent<Image>().color = Color.white;
            }
            go.GetComponent<Image>().color = Color.green;
            CurrentChosen = go;
        }
    }

    public void BackgroundMouseDown()
    {
        foreach (var col in DisableColliders)
        {
            if (col.GetComponent<Collider2D>() != null)
                col.GetComponent<Collider2D>().enabled = true;
        }
        gameObject.SetActive(false);
        if (!CurrentChosen.name.Replace(" ","").Replace("-","").ToLower().Equals(ChosenWeapon.Replace(" ","").Replace("-","").ToLower()))
        {
            // Update Db
            FindObjectOfType<AccessDatabase>().UpdateSessionInfo(PlayerPrefs.GetInt("PlayerID"), Type, CurrentChosen.name.Replace("(Clone)","").Replace(" ", ""));
            // reset data
            Scene.GetComponent<UECSessionScene>().RegenerateAllData();
        }
        transform.parent.GetComponent<BoxCollider2D>().enabled = true;
    }

    private bool CheckIfMouseOutsideScrollRange()
    {
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > Right.transform.position.x ||
            Camera.main.ScreenToWorldPoint(Input.mousePosition).x < Left.transform.position.x ||
            Camera.main.ScreenToWorldPoint(Input.mousePosition).y > Top.transform.position.y ||
            Camera.main.ScreenToWorldPoint(Input.mousePosition).y < Bottom.transform.position.y)
            return true;
        return false;
    }
    #endregion
}
