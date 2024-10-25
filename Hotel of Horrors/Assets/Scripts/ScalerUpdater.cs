using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScalerUpdater : MonoBehaviour
{
    [SerializeField] CanvasScaler scaler;
    private void OnEnable()
    {
        scaler = gameObject.GetComponent<CanvasScaler>();

        if (GlobalSettings.isFullScreen)
        {
            scaler.scaleFactor = 1f;
        }
        else
        {
            scaler.scaleFactor = 0.3f;
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (GlobalSettings.isFullScreen)
            {
                scaler.scaleFactor = 1f;
            }
            else
            {
                scaler.scaleFactor = 0.3f;
            }
            GlobalSettings.isFullScreen = !GlobalSettings.isFullScreen;
        }
    }
}
