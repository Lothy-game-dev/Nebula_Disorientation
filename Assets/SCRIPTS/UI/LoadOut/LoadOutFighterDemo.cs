using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadOutFighterDemo : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Scene;
    public GameObject Border;
    public GameObject ModelList;
    public GameObject WeaponList;
    public GameObject SecondPower;
    public GameObject Consumable;
    public LoadOutDetailStatus DetailStatus;
    #endregion
    #region NormalVariables
    private GameObject Model;
    private GameObject ModelAfterGenerate;
    private Sprite LeftWeapon;
    private Sprite RightWeapon;
    private float currentSpinAngle;
    private bool isFocusingWeapon;
    private float initScaleModel;
    private Vector2 ModelInitPos;
    private int currentNumberOfPower;
    public int currentNumberOfCons;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        currentNumberOfPower = 2;
        currentNumberOfCons = 4;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (!isFocusingWeapon && ModelAfterGenerate!=null && ModelAfterGenerate.activeSelf)
        {
            SpinModel();
        }
    }
    #endregion
    #region Set Model
    public void SetModel(string currentModel)
    {
        for (int i=0;i<ModelList.transform.childCount;i++)
        {
            if (ModelList.transform.GetChild(i).name.Replace(" ", "").ToLower()
                    .Equals(currentModel.Replace(" ", "").ToLower()))
            {
                Model = ModelList.transform.GetChild(i).gameObject;
                break;
            }
        }
        //Replace current Model
        ReplaceCurrentModel();
        ModelAfterGenerate = Instantiate(Model, new Vector3(Border.transform.position.x,
            Border.transform.position.y, Model.transform.position.z), Quaternion.identity);
        ModelAfterGenerate.transform.SetParent(Border.transform);
        ModelAfterGenerate.transform.Rotate(new Vector3(0, 0, currentSpinAngle));
        ModelAfterGenerate.transform.localScale = new Vector3(1, 1, 1);
        initScaleModel = ModelAfterGenerate.transform.localScale.x;
        if (LeftWeapon!=null)
        {
            ModelAfterGenerate.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = LeftWeapon;
        }
        if (RightWeapon != null)
        {
            ModelAfterGenerate.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = RightWeapon;
        }
        if (isFocusingWeapon)
        {
            ModelAfterGenerate.transform.GetChild(0).GetComponent<Canvas>().sortingOrder = 51;
        }
        else
        {
            ModelAfterGenerate.transform.GetChild(0).GetComponent<Canvas>().sortingOrder = 4;
        }
        ModelInitPos = ModelAfterGenerate.transform.position;
        Dictionary<string, object> statsDict = new Dictionary<string, object>();
        string stats = FindObjectOfType<AccessDatabase>().GetFighterStatsByName(currentModel);
        if ("Fail".Equals(stats))
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                "Unable to fetch fighter's data!\nPlease try again.", 5f);
        } else
        {
            statsDict = FindObjectOfType<GlobalFunctionController>().ConvertModelStatsToDictionary(stats);
        }
        if (int.Parse((string)statsDict["SP"])>=1)
        {
            if (int.Parse((string)statsDict["SP"]) == 1 && currentNumberOfPower == 2)
            {
                Scene.GetComponent<LoadoutScene>().SecondPower = "";
                currentNumberOfPower = 1;
                SecondPower.GetComponent<LoadOutPowerBar>().enabled = false;
                SecondPower.GetComponent<Collider2D>().enabled = false;
                SecondPower.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
                SecondPower.transform.GetChild(3).GetComponent<LoadOutStatusBoard>().SetData("");
                SecondPower.transform.GetChild(3).gameObject.SetActive(false);
                SecondPower.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                // Chain
                SecondPower.GetComponent<SpriteRenderer>().color = new Color(125/255f, 125/255f, 125/255f);
            }
            else if (int.Parse((string)statsDict["SP"]) == 2 && currentNumberOfPower == 1)
            {
                currentNumberOfPower = 2;
                SecondPower.GetComponent<LoadOutPowerBar>().enabled = true;
                SecondPower.GetComponent<Collider2D>().enabled = true;
                SecondPower.GetComponent<LoadOutPowerBar>().SetItem("");
                SecondPower.transform.GetChild(3).gameObject.SetActive(true);
                // Chain
                SecondPower.GetComponent<SpriteRenderer>().color = Color.white;
            }
        } else
        {
            // error
        }
        if (int.Parse((string)statsDict["SC"]) >= 3)
        {
            if (int.Parse((string)statsDict["SC"]) == 3 && currentNumberOfCons == 4)
            {
                currentNumberOfCons = 3;
                Consumable.GetComponent<LoadOutConsumables>().ResetNumberOfConsumable(3);
            }
            else if (int.Parse((string)statsDict["SC"]) == 4 && currentNumberOfCons == 3)
            {
                currentNumberOfCons = 4;
                Consumable.GetComponent<LoadOutConsumables>().ResetNumberOfConsumable(4);
            }
        }
        else
        {
            // error
        }
        DetailStatus.SetData(currentModel, statsDict);
        ModelAfterGenerate.SetActive(true);
    }
    public void FocusOnWeapon(bool isLeftWeapon)
    {
        isFocusingWeapon = true;
        ModelAfterGenerate.transform.Rotate(new Vector3(0, 0, -currentSpinAngle));
        currentSpinAngle = 0;
        GetComponent<SpriteRenderer>().sortingOrder = 50;
        transform.GetChild(1).GetComponent<Canvas>().sortingOrder = 52;
        if (ModelAfterGenerate!=null)
        {
            ModelAfterGenerate.transform.GetChild(0).GetComponent<Canvas>().sortingOrder = 51;
        }
        // Animation
        StartCoroutine(ZoomToWeapon(isLeftWeapon));
    }

    public void StopFocusOnWeapon(bool isLeftWeapon)
    {
        GetComponent<SpriteRenderer>().sortingOrder = 3;
        transform.GetChild(1).GetComponent<Canvas>().sortingOrder = 5;
        if (ModelAfterGenerate != null)
        {
            ModelAfterGenerate.transform.GetChild(0).GetComponent<Canvas>().sortingOrder = 4;
        }
        // Animation
        StartCoroutine(ZoomOutFromWeapon(isLeftWeapon));
    }

    private IEnumerator ZoomToWeapon(bool isLeft)
    {
        Vector2 speed = new Vector2();
        Rigidbody2D rb;
        if (ModelAfterGenerate.GetComponent<Rigidbody2D>()==null)
        {
            rb = ModelAfterGenerate.AddComponent<Rigidbody2D>();
        } else
        {
            rb = ModelAfterGenerate.GetComponent<Rigidbody2D>();
        }
        rb.isKinematic = true;
        for (int i=0;i<50;i++)
        {
            if (i==12)
            {
                if (isLeft)
                {
                    ModelAfterGenerate.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                } else
                {
                    ModelAfterGenerate.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                }
            }
            ModelAfterGenerate.transform.localScale = new Vector3(
                ModelAfterGenerate.transform.localScale.x + initScaleModel * 0.04f,
                ModelAfterGenerate.transform.localScale.y + initScaleModel * 0.04f,
                ModelAfterGenerate.transform.localScale.z);
            if (isLeft)
            {
                speed = (ModelAfterGenerate.transform.GetChild(0).GetChild(0).position
                    - transform.position) * 3f;
            }
            else
            {
                speed = (ModelAfterGenerate.transform.GetChild(0).GetChild(1).position
                    - transform.position) * 3f;
            }
            rb.velocity = -speed;
            yield return new WaitForSeconds(0.01f);
        }
        rb.velocity = new Vector2(0, 0);
    }

    private IEnumerator ZoomOutFromWeapon(bool isLeft)
    {
        Vector2 speed = new Vector2(); Rigidbody2D rb;
        if (ModelAfterGenerate.GetComponent<Rigidbody2D>() == null)
        {
            rb = ModelAfterGenerate.AddComponent<Rigidbody2D>();
        }
        else
        {
            rb = ModelAfterGenerate.GetComponent<Rigidbody2D>();
        }
        rb.isKinematic = true;
        for (int i = 0; i < 50; i++)
        {
            if (i==32)
            {
                if (isLeft)
                {
                    ModelAfterGenerate.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                }
                else
                {
                    ModelAfterGenerate.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                }
            }
            ModelAfterGenerate.transform.localScale = new Vector3(
                ModelAfterGenerate.transform.localScale.x - initScaleModel * 0.04f,
                ModelAfterGenerate.transform.localScale.y - initScaleModel * 0.04f,
                ModelAfterGenerate.transform.localScale.z);
            speed = (ModelAfterGenerate.transform.position - transform.position) *3f;
            rb.velocity = -speed;
            yield return new WaitForSeconds(0.01f);
        }
        rb.velocity = new Vector2(0, 0);
        if (isLeft)
        {
            ModelAfterGenerate.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            ModelAfterGenerate.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        }
        ModelAfterGenerate.transform.position = new Vector3(
            ModelInitPos.x, ModelInitPos.y, ModelAfterGenerate.transform.position.z);
        isFocusingWeapon = false;
    }

    public void SetWeapon(bool isLeftWeapon, string WeaponName)
    {
        for (int i=0; i < WeaponList.transform.childCount;i++)
        {
            if (WeaponList.transform.GetChild(i).name.Replace(" ","").ToLower()
                .Equals(WeaponName.Replace(" ","").ToLower())) {
                if (isLeftWeapon)
                {
                    LeftWeapon = WeaponList.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>().sprite;
                } else
                {
                    RightWeapon = WeaponList.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>().sprite;
                }
            }
        }
        if (ModelAfterGenerate!=null)
        {
            if (isLeftWeapon)
            {
                ModelAfterGenerate.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = LeftWeapon;
            } else
            {
                ModelAfterGenerate.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = RightWeapon;
            }
        }
    }
    private void ReplaceCurrentModel()
    {
        Destroy(ModelAfterGenerate);
    }

    private void SpinModel()
    {
        ModelAfterGenerate.transform.Rotate(new Vector3(0, 0, 1f));
        if (currentSpinAngle==360)
        {
            currentSpinAngle = 1;
        } else
        {
            currentSpinAngle += 1;
        }
    }
    #endregion
}
