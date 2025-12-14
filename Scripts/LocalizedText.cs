using TMPro;
using UnityEngine;
using UnityLiteL10n;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string key;

    private TextMeshProUGUI _text;
    private bool _subscribed = false;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void Start() => TrySubscribe();

    private void OnEnable() => TrySubscribe();

    private void OnDisable()
    {
        if (_subscribed && LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
            _subscribed = false;
        }
    }

    private void TrySubscribe()
    {
        if (_subscribed || LocalizationManager.Instance == null) return;

        LocalizationManager.Instance.OnLanguageChanged += UpdateText;
        _subscribed = true;
        UpdateText();
    }

    public void UpdateText()
    {
        if (_text == null || string.IsNullOrEmpty(key)) return;
        _text.text = LocalizationManager.Instance.Get(key);
    }

    public void SetKey(string newKey)
    {
        key = newKey;
        UpdateText();
    }
}
