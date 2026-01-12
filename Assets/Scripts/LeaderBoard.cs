using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Record
{
    public string name = "Player";
    public float value = 0f; 
}

public class Leaderboard : MonoBehaviour
{
    public static Leaderboard Instance;

    [Header("UI")]
    public TextMeshProUGUI leaderboardText;
    public GameObject recordInputPanel;
    public InputField nameInput;

    private const string SAVE_KEY = "LEADERBOARD";

    private List<Record> records = new List<Record>();

    private float pendingValue;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadRecords();
        RefreshUI();
        ChangeRecordInputState(false);
    }
    public void ChangeRecordInputState(bool state)
    {
        recordInputPanel.SetActive(state);
        nameInput.text = "";
    }

    // Сабмит через инспектор на клаве
    public void SubmitRecord()
    {
        string playerName = string.IsNullOrWhiteSpace(nameInput.text)
            ? "Player"
            : nameInput.text.Trim();

        records.Add(new Record
        {
            name = playerName,
            value = pendingValue
        });

        records = records
            .OrderByDescending(r => r.value)
            .Take(10)
            .ToList();

        SaveRecords();
        RefreshUI();
        ChangeRecordInputState(false);
    }

    public void SendNewValue(float value)
    {
        if (value < 1)
            return;
        pendingValue = value;
        ChangeRecordInputState(true);
    }
    void SaveRecords()
    {
        var wrapper = new RecordsWrapper { records = records };
        PlayerPrefs.SetString(SAVE_KEY, JsonUtility.ToJson(wrapper));
        PlayerPrefs.Save();
    }

    void LoadRecords()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
            return;

        var list = JsonUtility.FromJson<RecordsWrapper>(
            PlayerPrefs.GetString(SAVE_KEY));

        records = list.records ?? new List<Record>();
    }

    public void RefreshUI()
    {
        string text = "<b>ЛИДЕРБОРД</b>\n";

        for (int i = 0; i < records.Count; i++)
        {
            text += $"{i + 1}. {records[i].name} — {records[i].value:F2}\n";
        }

        leaderboardText.text = text;
    }
}

[System.Serializable]
class RecordsWrapper
{
    public List<Record> records;
}
