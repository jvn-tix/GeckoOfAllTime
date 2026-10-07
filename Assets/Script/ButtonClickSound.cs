using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClickSound : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(PlayClick);
    }

    private void PlayClick()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();
    }
}