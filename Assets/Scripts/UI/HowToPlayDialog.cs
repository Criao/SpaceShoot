using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HowToPlayDialog : DialogBase
{
    [SerializeField] private GameObject infoPage;
    [SerializeField] private GameObject controlPage;
    [SerializeField] private Button infoButton;
    [SerializeField] private Button controlButton;

    void Start()
    {
        infoPage.SetActive(true);
        controlPage.SetActive(false);

        infoButton.interactable = false;
        controlButton.interactable = true;
    }

    public void ControlButtonClicked()
    {
        infoPage.SetActive(false);
        controlPage.SetActive(true);

        infoButton.interactable = true;
        controlButton.interactable = false;
    }

    public void InfoButtonClicked()
    {
        infoPage.SetActive(true);
        controlPage.SetActive(false);

        infoButton.interactable = false;
        controlButton.interactable = true;
    }




}
