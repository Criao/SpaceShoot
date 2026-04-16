using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LivesUIController : MonoBehaviour
{
    [SerializeField] private GameObject[] livesIcons;

    public void UpdateLives(int livesNum)
    {
        for(int i = 0;i < 5; i++)
        {
            livesIcons[i].gameObject.SetActive(false);
        }
        for(int i = 0;i < livesNum; i++)
        {
            livesIcons[i].gameObject.SetActive(true);
        }
    }

}
