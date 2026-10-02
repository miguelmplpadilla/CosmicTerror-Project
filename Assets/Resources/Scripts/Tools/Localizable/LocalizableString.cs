using System;
using UnityEngine;
using UnityEngine.Localization.Settings;

[Serializable]
public class LocalizableString
{
    public string value
    {
        get
        {
            var locale = LocalizationSettings.SelectedLocale;
            string language = PlayerPrefs.GetString("language", locale?.Identifier.Code ?? "es");
            
            language = language.Split('-')[0];

            switch (language)
            {
                case "es":
                    return string.IsNullOrWhiteSpace(valueSpanish) ? valueEnglish : valueSpanish;
                case "en":
                    return string.IsNullOrWhiteSpace(valueEnglish) ? valueSpanish : valueEnglish;
                default:
                    return string.IsNullOrWhiteSpace(valueEnglish) ? valueSpanish : valueEnglish;
            }
        }

        set
        {
            var locale = LocalizationSettings.SelectedLocale;
            string language = PlayerPrefs.GetString("language", locale?.Identifier.Code ?? "es");
            
            language = language.Split('-')[0];

            switch (language)
            {
                case "es":
                    valueSpanish = value;
                    break;
                case "en":
                    valueEnglish = value;
                    break;
                default:
                    valueEnglish = value;
                    break;
            }
        }
    }

    public LocalizableString(string valueSpanish, string valueEnglish)
    {
        this.valueSpanish = valueSpanish;
        this.valueEnglish = valueEnglish;
    }

    [SerializeField, TextArea(3, 10)] private string valueSpanish;
    [SerializeField, TextArea(3, 10)] private string valueEnglish;
}
