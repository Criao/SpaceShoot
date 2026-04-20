using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 生命值UI控制器 - 管理生命值图标的显示
/// </summary>
public class LivesUIController : MonoBehaviour
{
    [SerializeField] private GameObject[] livesIcons; // 生命值图标数组

    /// <summary>
    /// 更新生命值显示
    /// </summary>
    /// <param name="livesNum">当前生命值数量</param>
    public void UpdateLives(int livesNum)
    {
        // 先隐藏所有生命值图标
        for(int i = 0;i < 5; i++)
        {
            livesIcons[i].gameObject.SetActive(false);
        }
        // 根据当前生命值显示对应数量的图标
        for(int i = 0;i < livesNum; i++)
        {
            livesIcons[i].gameObject.SetActive(true);
        }
    }

}
