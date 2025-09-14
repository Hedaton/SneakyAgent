using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public TextMeshProUGUI ammoDisplay;
    public Camera mainCamera;
    public Camera camera1;
    public Transform panel;
    public GameObject crosshair;

    public TextMeshProUGUI text;


    public void Panel()
    {
        if (mainCamera.gameObject.activeSelf == true)
        {
            text.text = "Main Camera";
            mainCamera.gameObject.SetActive(false);
            camera1.gameObject.SetActive(true);
            panel.gameObject.SetActive(true);
            crosshair.gameObject.SetActive(false);
        }
        else
        {
            text.text = "Second Camera";
            mainCamera.gameObject.SetActive(true);
            camera1.gameObject.SetActive(false);
            panel.gameObject.SetActive(false);
            crosshair.gameObject.SetActive(true);
        }
    }

    public void UpdateAmmoDislay(int currentAmmo, int maxAmmo)
    {
        ammoDisplay.text = currentAmmo + "/" + maxAmmo;
    }

    public void AmmoDisplay(bool show)
    {
        ammoDisplay.enabled = show;
    }
}
