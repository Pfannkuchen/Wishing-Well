using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class AdditionalInfoInstance : MonoBehaviour
{
    private TMP_Text _text;
    private TMP_Text Text => _text ??= GetComponent<TMP_Text>();

    private void OnEnable()
    {
        UpdateContent();
        AdditionalInfoManager.InfoUpdated += UpdateContent;
    }

    private void OnDisable()
    {
        AdditionalInfoManager.InfoUpdated -= UpdateContent;
    }

    private void UpdateContent()
    {
        if (Text == null)
        {
            Debug.LogError("Text is null!");
            return;
        }

        string s = "<b><u>Externe Daten</u></b>" + "\n";
        s += "\n";
        s += "Datenbank-Größe: " + AdditionalInfoManager.Parameter.DatabaseSize.GetValue() + " kB" + "\n";
        s += "Datenbank-Ladezeit: " + AdditionalInfoManager.Parameter.DatabaseLoadTime.GetValue() + " ms" + "\n";
        s += "Münzen-Größe: " + AdditionalInfoManager.Parameter.CoinManifests.GetValue() + "\n";
        s += "\n";
        s += "\n";
        s += "\n";
        s += "<b><u>Lokale Daten</u></b>" + "\n";
        s += "\n";
        s += "Sitzungen: " + AdditionalInfoManager.Parameter.Sessions.GetValue() + "\n";
        s += "Generierte Münzen: " + AdditionalInfoManager.Parameter.CoinsLoaded.GetValue() + "\n";
        s += "Geworfene Münzen: " + AdditionalInfoManager.Parameter.CoinsThrown.GetValue() + "\n";
    }
}
