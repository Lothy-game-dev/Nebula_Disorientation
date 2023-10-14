using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WSHealthBar : MonoBehaviour
{
    #region ComponentVariables
    public Slider slider;
    #endregion
    #region InitializeVariables
    public Vector3 Position;
    #endregion
    #region NormalVariables
    private float CurrentValue;
    private float MaxValue;
    private Vector2 PositionSlider;
    private float Timer;
    private bool isHit;
    private Coroutine coroutine;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        Timer = 2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isHit)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            Timer -= Time.deltaTime;
            Color c = slider.gameObject.transform.GetChild(1).GetChild(0).GetComponent<Image>().color;
            c.a = 1f;
            slider.gameObject.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = c;
            slider.gameObject.SetActive(true);
            if (Timer <= 0f)
            {
                coroutine = StartCoroutine(SliderFaded());                            
                Timer = 2f;
                isHit = false;
            }
        }
        slider.value = CurrentValue;
        slider.gameObject.transform.position = transform.parent.position + Position;
    }
    #endregion
    #region Public Set Value
    // Set Value from public
    public void SetValue(float value, float maxValue, bool hit)
    {
        CurrentValue = value;
        MaxValue = maxValue;
        slider.maxValue = MaxValue;
        isHit = hit;
        Timer = 2f;
    }

    public void SetPostion(Vector2 pos)
    {
        PositionSlider = pos;
    }

    IEnumerator SliderFaded()
    {
        for (int i = 0; i < 10; i++)
        {
            Color c = slider.gameObject.transform.GetChild(1).GetChild(0).GetComponent<Image>().color;
            c.a -= 0.1f;
            slider.gameObject.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = c;
            yield return new WaitForSeconds(0.2f);
        }

        slider.gameObject.SetActive(false);
    }
    #endregion
}
