using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public Slider LoadingSlider;
    public float LoadingTime;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        LoadingSlider.maxValue = 100;
        // Initialize variables
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    private void FixedUpdate()
    {
        LoadingSlider.value += 100f / (LoadingTime * 60);
        if (LoadingSlider.value >= 100)
        {
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
    #endregion
    #region Loading animation
/*    // Group all function that serve the same algorithm
    private IEnumerator LoadingAnimation(float second)
    {
        Debug.Log("LoadingScene - " + second);
        for (int i = 0; i < second*60; i++)
        {
            Debug.Log("LoadingScene for count - " + i);
            LoadingSlider.value += 100f/(second * 60);
            yield return new WaitForSeconds(1/60f);
        }
        Destroy(gameObject.transform.parent.gameObject);
    }*/
    #endregion
}
