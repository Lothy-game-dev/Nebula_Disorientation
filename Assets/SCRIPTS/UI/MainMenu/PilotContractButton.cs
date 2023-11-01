using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotContractButton : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    private Rigidbody2D rb;
    #endregion
    #region InitializeVariables
    public GameObject TOSWord;
    public GameObject PilotContract;
    public GameObject[] AgreeAppearList;
    public Vector2 LeftRightInitPos;
    #endregion
    #region NormalVariables
    private float LeftRightVelocity;
    private float InitScaleX;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        if (name=="TOSButton" || name=="AGButton")
        {
            InitScaleX = transform.localScale.x;
            transform.localScale = new Vector3(0,transform.localScale.y,transform.localScale.z);
            TOSWord.SetActive(false);
            GetComponent<BoxCollider2D>().enabled = false;
            StartCoroutine(StartButtonAnimation());
        } 
        else if (name=="Left")
        {
            rb = GetComponent<Rigidbody2D>();
            InitScaleX = transform.localScale.x;
            LeftRightVelocity = -0.3f;
            transform.position = new Vector3(transform.position.x + 0.3f,transform.position.y,transform.position.z);
        }
        else if (name == "Right")
        {
            rb = GetComponent<Rigidbody2D>();
            InitScaleX = transform.localScale.x;
            LeftRightVelocity = 0.3f;
            transform.position = new Vector3(transform.position.x - 0.3f, transform.position.y, transform.position.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (name == "Left")
        {
            if (transform.position.x <= LeftRightInitPos.x)
            {
                rb.velocity = new Vector2(0, 0);
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = 0;
                GetComponent<SpriteRenderer>().color = c;
                transform.position = new Vector3(LeftRightInitPos.x + 0.3f, LeftRightInitPos.y, transform.position.z);
            } else
            {
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = 1;
                GetComponent<SpriteRenderer>().color = c;
                rb.velocity = new Vector2(LeftRightVelocity, 0);
            }
        } else if (name == "Right")
        {
            if (transform.position.x >= LeftRightInitPos.x)
            {
                rb.velocity = new Vector2(0, 0);
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = 0;
                GetComponent<SpriteRenderer>().color = c;
                transform.position = new Vector3(LeftRightInitPos.x - 0.3f, LeftRightInitPos.y, transform.position.z);
            }
            else
            {
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = 1;
                GetComponent<SpriteRenderer>().color = c;
                rb.velocity = new Vector2(LeftRightVelocity, 0);
            }
        }
    }
    #endregion
    #region Animation
    public IEnumerator StartButtonAnimation()
    {
        for (int i=0;i<10;i++)
        {
            transform.localScale = new Vector3(transform.localScale.x + InitScaleX / 10f, transform.localScale.y, transform.localScale.z);
            yield return new WaitForSeconds(0.1f);
        }
        TOSWord.SetActive(true);
        GetComponent<BoxCollider2D>().enabled = true;
    }
    #endregion
    #region Mouse Check
    private void OnMouseDown()
    {
        FindObjectOfType<SoundSFXGeneratorController>().GenerateSound("ButtonClick");
        if (name=="Right")
        {
            if (PilotContract.GetComponent<PilotContract>()!=null)
            {
                PilotContract.GetComponent<PilotContract>().NextPage();
            }
        }
        else if (name=="Left")
        {
            if (PilotContract.GetComponent<PilotContract>() != null)
            {
                PilotContract.GetComponent<PilotContract>().PreviousPage();
            }
        }
        else if (name== "TOSButton")
        {
            if (PilotContract.GetComponent<PilotContract>() != null)
            {
                PilotContract.GetComponent<PilotContract>().NextPage();
            }
        } else if (name=="AGButton")
        {
            foreach (var a in AgreeAppearList)
            {
                a.SetActive(true);
            }
            PilotContract.SetActive(false);
        }
    }

    public void ResetPosLeftRight()
    {
        if (name == "Left")
        {
            if (LeftRightInitPos == new Vector2(0,0))
            {
                LeftRightInitPos = transform.position;
            }
            transform.position = new Vector3(LeftRightInitPos.x + 0.3f, transform.position.y, transform.position.z);
        }
        else if (name=="Right")
        {
            if (LeftRightInitPos == new Vector2(0, 0))
            {
                LeftRightInitPos = transform.position;
            }
            transform.position = new Vector3(LeftRightInitPos.x - 0.3f, transform.position.y, transform.position.z);
        }
    }
    #endregion
}
