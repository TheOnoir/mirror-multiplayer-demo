using UnityEngine;
using TMPro;

public class InGameConsole : MonoBehaviour
{
    public TMP_Text logText; // UI TextMeshPro on Canvas 

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        logText.text += logString + "\n";
    }
}
