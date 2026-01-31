using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogButtonColor : MonoBehaviour
{
    [SerializeField] private Color colorNormal = Color.white;
    [SerializeField] private Color colorDisabled = Color.grey;

    Button ownerButton;
    TextMeshProUGUI text;

    private void Start()
    {
        ownerButton = transform.parent.GetComponent<Button>();
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (ownerButton.interactable)
        {
            if (text) text.color = colorNormal;
        }
        else
        {
            if (text) text.color = colorDisabled;
        }
    }
}
