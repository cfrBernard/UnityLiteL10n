using UnityEngine;
using UnityLiteL10n;

public class LanguageSwitcher : MonoBehaviour
{
    public void OnChangeToEN() => LocalizationManager.Instance.SetLanguage("EN");
    public void OnChangeToFR() => LocalizationManager.Instance.SetLanguage("FR");
    public void OnChangeToES() => LocalizationManager.Instance.SetLanguage("ES");
    public void OnChangeToJA() => LocalizationManager.Instance.SetLanguage("JA");
    public void OnChangeToKO() => LocalizationManager.Instance.SetLanguage("KO");
}
