using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorEditor : MonoBehaviour
{

    public static ErrorEditor _instance ;
    Text textError;
    float time = 0;
    float timeMaxError = 5f;
    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
        textError = this.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > timeMaxError) textError.text = "";
    }

    public void SetError(string textError)
    {
        this.textError.text = textError;
        time = 0f;

    }
}
