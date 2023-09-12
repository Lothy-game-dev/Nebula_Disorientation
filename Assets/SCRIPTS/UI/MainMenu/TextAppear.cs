using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextAppear : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        Enter();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Enter & Exit
    public void Enter()
    {
        Color c = GetComponent<SpriteRenderer>().color;
        c.a = 0f;
        GetComponent<SpriteRenderer>().color = c;
        StartCoroutine(EnterDelay());
    }

    private IEnumerator EnterDelay()
    {
        for (int i=0;i<10;i++)
        {
            Color c = GetComponent<SpriteRenderer>().color;
            c.a += 0.1f;
            GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(0.2f);
        }
    }
    #endregion
}
