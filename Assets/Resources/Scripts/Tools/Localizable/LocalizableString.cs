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

            string language = PlayerPrefs.GetString("language", locale.Identifier.Code);

            switch (language)
            {
                case "es":
                    return valueSpanish;
                case "en":
                    return valueEnglish;
                default:
                    return valueEnglish;
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