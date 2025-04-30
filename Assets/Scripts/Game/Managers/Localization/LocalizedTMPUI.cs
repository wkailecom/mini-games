using TMPro;
using UnityEngine;


[AddComponentMenu("UI/Localized TextMesh Pro UI", 13)]
[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedTMPUI : LocalizedTextComponent<TextMeshProUGUI>
{
    protected override void SetText(TextMeshProUGUI text, string value)
    {
        text.text = value;
    }


}