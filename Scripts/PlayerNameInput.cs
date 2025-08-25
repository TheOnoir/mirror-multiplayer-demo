using UnityEngine;
using TMPro;

public class PlayerNameInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private GameObject startPanel; // the panel with the nickname input 

    public void SaveName()
    {
        string name = inputField.text;

        if (string.IsNullOrWhiteSpace(name)) 
        {
            name = "Player_" + Random.Range(1000, 9999);
        }

        PlayerPrefs.SetString("PlayerName", name);
        PlayerPrefs.Save();
        startPanel.SetActive(false);
    }
}
