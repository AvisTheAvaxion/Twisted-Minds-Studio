using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScalerUpdater : MonoBehaviour
{
    bool f = false;
    [SerializeField] CanvasScaler scaler;
    private void OnEnable()
    {
        if (GlobalSettings.isFullScreen)
        {
            scaler.scaleFactor = 1f;
        }
        else
        {
            scaler.scaleFactor = 0.47f;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (f)
            {
                scaler.scaleFactor = 1f;
            }
            else
            {
                scaler.scaleFactor = 0.47f;
            }
            f = !f;
        }
    }
}
