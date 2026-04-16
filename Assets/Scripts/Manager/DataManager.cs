using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;


public class DataManager : MonoBehaviour
{
    public static DataManager Instance{ get; private set; }//静态Instance属性用来保持数据持久化
    public static event Action SettingsChanged;

    public float musicSettingValue;
    public float SFXSettingValue;
    public int highScore;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);//销毁新的DataManager对象，保持数据持久化
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);//将第一次加载的dataManager对象保存在游戏场景中
        LoadMusicSetting();
        SettingsChanged?.Invoke();
    }
    [Serializable]
    public class SaveData
    {
        public float musicSetting;
        public float SFXSetting;
        public int highScore;
    }

    public void SaveMusicSetting()
    {
        SaveData data = new SaveData();
        data.musicSetting = musicSettingValue;
        data.SFXSetting = SFXSettingValue;
        data.highScore = highScore;

        var json = JsonConvert.SerializeObject(data);
        File.WriteAllText(Application.persistentDataPath + "/Settings.json", json);
        SettingsChanged?.Invoke();
    }

    public void LoadMusicSetting()
    {
        string path = Application.persistentDataPath + "/Settings.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonConvert.DeserializeObject<SaveData>(json);
            musicSettingValue = Mathf.Clamp01(data.musicSetting);
            SFXSettingValue = Mathf.Clamp01(data.SFXSetting);
            highScore = data.highScore;
            Debug.Log($"DataManager 读取成功，music={musicSettingValue} sfx={SFXSettingValue} highScore={highScore}");
        }
        else
        {
            Debug.Log("DataManager 没有找到存档，使用默认值");
            musicSettingValue = 0.5f;
            SFXSettingValue = 0.5f;
            highScore = 0;
        }

    }

    /// <summary>
    /// 更新最高分，如果当前分数更高则保存
    /// </summary>
    public void UpdateHighScore(int currentScore)
    {
        if (currentScore > highScore)
        {
            highScore = currentScore;
            SaveMusicSetting();
            Debug.Log($"新的最高分: {highScore}");
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("DataManager");
            obj.AddComponent<DataManager>();
        }
    }
}
