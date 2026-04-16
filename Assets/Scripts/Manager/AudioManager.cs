using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    // 缓存当前 AudioSource 组件，避免每次重复查找
    private AudioSource audioSource;

    // Awake 在对象实例化时调用，优先于 Start
    private void Awake()
    {
        // 获取挂载的 AudioSource 组件
        audioSource = GetComponent<AudioSource>();

        // 立即应用当前存档中的音量设置
        ApplyVolume();
    }

    // OnEnable 在 GameObject 激活时调用
    private void OnEnable()
    {
        // 订阅 DataManager 的设置变化回调
        DataManager.SettingsChanged += ApplyVolume;

        // 再次应用当前值，保证状态同步
        ApplyVolume();
    }

    // OnDisable 在 GameObject 禁用或销毁时调用
    private void OnDisable()
    {
        // 取消订阅，避免内存泄漏或空引用
        DataManager.SettingsChanged -= ApplyVolume;
    }

    // 统一音量应用逻辑
    private void ApplyVolume()
    {
        // 如果 audioSource 丢失，尝试重新获取
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        // 如果仍然为空，则直接跳过
        if (audioSource == null) return;

        // 仅在 DataManager 实例存在时才读取设置值
        if (DataManager.Instance != null)
        {
            audioSource.volume = DataManager.Instance.musicSettingValue;
            Debug.Log("AudioManager 设置音量：" + audioSource.volume);
        }
    }
}

