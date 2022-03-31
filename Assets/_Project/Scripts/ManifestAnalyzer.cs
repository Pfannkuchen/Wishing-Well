using System.Collections.Generic;
using System.Threading.Tasks;
using Leipzig;
using Newtonsoft.Json;
using UnityEngine;

public class ManifestAnalyzer : MonoBehaviour
{
    [SerializeField] private Dictionary<string, int> _allMetadata;
    private Dictionary<string, int> AllMetadata => _allMetadata ??= new Dictionary<string, int>();

    public void Analyze(List<Manifest> allManifests)
    {
        /*
        if (allManifests == null || allManifests.Count <= 0)
        {
            Debug.LogError("allManifests is null or empty!");
            return;
        }
        
        AsyncAnalyzation(allManifests);
        */
    }
/*
    public async Task AsyncAnalyzation(List<Manifest> allManifests)
    {
        foreach (Manifest m in allManifests)
        {
            // deserialize manifest
            string mJSON = await DataProvider.GetDataBaseJSON(m.Id);
            ManifestDeserialized mDeserialized = JsonConvert.DeserializeObject<ManifestDeserialized>(mJSON);
        }
    }
    */
}