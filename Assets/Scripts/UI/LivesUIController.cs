using System.Collections;
using System.Collections.Generic;
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
        if (livesIcons == null || livesIcons.Length == 0) return;

        int visibleLives = Mathf.Clamp(livesNum, 0, livesIcons.Length);

        // 先隐藏所有生命值图标
        for(int i = 0;i < livesIcons.Length; i++)
        {
            if (livesIcons[i] != null)
            {
                livesIcons[i].SetActive(i < visibleLives);
            }
        }
    }

}
