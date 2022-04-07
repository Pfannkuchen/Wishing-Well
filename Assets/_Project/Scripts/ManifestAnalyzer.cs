using System.Collections.Generic;
using System.Threading.Tasks;
using Leipzig;
using Newtonsoft.Json;
using UnityEngine;

public class ManifestAnalyzer : MonoBehaviour
{
    private Dictionary<string, int> _allMetadata;
    private Dictionary<string, int> AllMetadata => _allMetadata ??= new Dictionary<string, int>();

    [SerializeField, TextArea(5, 20)] private string _result;

    public void Analyze(List<Manifest> allManifests)
    {
        // allow for easy enable/disable in the Scene
        if (!gameObject.activeInHierarchy) return;

        if (allManifests == null || allManifests.Count <= 0)
        {
            Debug.LogError("allManifests is null or empty!");
            return;
        }

        AsyncAnalyzation(allManifests);
    }

    private async Task AsyncAnalyzation(List<Manifest> allManifests)
    {
        int count = 0;
        foreach (Manifest m in allManifests)
        {
            count++;

            // deserialize manifest
            string mJSON = await DataProvider.GetDataBaseJSON(m.Id);
            ManifestDeserialized mDeserialized = JsonConvert.DeserializeObject<ManifestDeserialized>(mJSON);

            if (mDeserialized == null || mDeserialized.metadata == null || mDeserialized.metadata.Count <= 0) continue;

            foreach (Metadata md in mDeserialized.metadata)
            {
                if (DataProvider.ApplicationQuit.IsCancellationRequested) return;
                if (md == null) continue;

                if (AllMetadata.ContainsKey(md.label))
                {
                    AllMetadata[md.label] = AllMetadata[md.label] + 1;
                }
                else
                {
                    AllMetadata.Add(md.label, 1);
                }
            }
            
            if (count % 50 == 0)
            {
                LogResult(count, allManifests.Count);
            }
        }

        LogResult(count, allManifests.Count);

        Debug.Log($"{gameObject.name} -> Metadata Analysis complete.");
    }

    private void LogResult(int currentManifestIndex, int manifestCount)
    {
        _result = $"Analyzed Manifest {currentManifestIndex} of {manifestCount}\n";
        _result += $"Metadata count: {AllMetadata.Count}\n";
        _result += "----------\n";

        foreach (KeyValuePair<string, int> entry in AllMetadata)
        {
            _result += ($"{entry.Value.ToString()}x {entry.Key}\n");
        }
    }
}