using System;
using System.Collections.Generic;
using Leipzig;
using UnityEngine;

[Serializable]
public class CoinData
{
    public Texture2D FrontTex;
    public Texture2D BackTex;

    public string[] Information;
    public float DiameterMM;
    public float Diameter => Mathf.Clamp(DiameterMM / 20f, 0.3f, 1.2f);
    public float Weight;
    public CoinMaterial CoinMat;
    
    public ManifestDeserialized OriginalManifest;

    
    public void SetOriginalManifest(ManifestDeserialized originalManifest)
    {
        OriginalManifest = originalManifest;
    }

    public void SetTextures(Texture2D frontTex, Texture2D backTex)
    {
        FrontTex = frontTex;
        BackTex = backTex;
    }

    public void SetMetadata(Metadata[] metadata)
    {
        List<string> i = new List<string>();

        foreach (Metadata md in metadata)
        {
            if (string.IsNullOrEmpty(md.label)) continue;

            switch (md.label)
            {
                case "weight (g)":
                    float.TryParse(md.value, out Weight);
                    break;
                case "diameter (mm)":
                    float.TryParse(md.value, out DiameterMM);
                    break;
                case "width (mm)":
                    float.TryParse(md.value, out DiameterMM);
                    break;
                case "height (mm)":
                    float.TryParse(md.value, out DiameterMM);
                    break;
                case "Material":
                    SetMaterial(md.value);
                    break;
                case "Material URIs": break;
                case "orientation (clock)": break;
                default:
                    i.Add(md.value);
                    break;
            }
        }

        Information = i.ToArray();
    }

    private void SetMaterial(string materialIdentifier)
    {
        CoinMat = CoinMaterial.Default;
    }
}