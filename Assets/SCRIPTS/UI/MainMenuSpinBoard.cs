using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuSpinBoard : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject ModelNameText;
    #endregion
    #region NormalVariables
    private GameObject currentSpinModel;
    private float ChangeTimer;
    private bool doneStart;
    private int currentRotate;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        doneStart = false;
        Color c = GetComponent<SpriteRenderer>().color;
        c.a = 0;
        GetComponent<SpriteRenderer>().color = c;
        StartCoroutine(BrightUp());
        
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (doneStart)
        {
            if (ChangeTimer > 0f)
            {
                ChangeTimer -= Time.deltaTime;
            }
            else
            {
                ChooseRandomModel();
                ChangeTimer = 5f;
            }
        }
    }
    private void FixedUpdate()
    {
        if (doneStart)
        {
            currentSpinModel.transform.Rotate(new Vector3(0, 0, 1f));
            if (currentRotate>360)
            {
                currentRotate -= 360;
            }
            currentRotate++;
        }
    }
    #endregion
    #region Random Model Choose
    private void ChooseRandomModel()
    {
        if (currentSpinModel!=null)
        {
            currentSpinModel.SetActive(false);
        }
        int i = Random.Range(0, transform.childCount);
        currentSpinModel = transform.GetChild(i).gameObject;
        float initScaleX = currentSpinModel.transform.localScale.x;
        float initScaleY = currentSpinModel.transform.localScale.y;
        float initScaleZ = currentSpinModel.transform.localScale.z;
        currentSpinModel.transform.localScale = new Vector3(initScaleX / 40, initScaleY / 40, initScaleZ / 40);
        currentSpinModel.transform.Rotate(new Vector3(0, 0, currentRotate));
        currentSpinModel.SetActive(true);
        ModelNameText.GetComponent<TextMeshPro>().text = currentSpinModel.name;
        StartCoroutine(BiggerModel(initScaleX, initScaleY, initScaleZ));
    }

    private IEnumerator BiggerModel(float initScaleX, float initScaleY, float initScaleZ)
    {
        for (int i=0;i<40;i++)
        {
            currentSpinModel.transform.localScale =
                new Vector3(currentSpinModel.transform.localScale.x + initScaleX / 40,
                currentSpinModel.transform.localScale.y + initScaleY / 40,
                currentSpinModel.transform.localScale.z + initScaleZ / 40);
            yield return new WaitForSeconds(0.025f);
        }
    }

    private IEnumerator BrightUp()
    {
        for (int i=0;i<20;i++)
        {
            Color c = GetComponent<SpriteRenderer>().color;
            c.a += 0.05f;
            GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(0.1f);
        }
        ChangeTimer = 5f;
        ChooseRandomModel();
        doneStart = true;
        ModelNameText.SetActive(true);
    }

    private IEnumerator BrightDown()
    {
        doneStart = false;
        currentSpinModel = null;
        for (int i = 0; i < 50; i++)
        {
            Color c = GetComponent<SpriteRenderer>().color;
            c.a -= 0.02f;
            GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(0.1f);
        }
    }
    #endregion
}
