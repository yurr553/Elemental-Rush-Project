using UnityEngine;

public class PowerInventory : MonoBehaviour
{
    [Header("UI")]
    public Transform powerContainer;

    [System.Serializable]
    public class PowerEntry
    {
        public string tagName;
        public GameObject prefab;
        public int defaultUsesPerIcon = 1;
    }

    [Header("Map matched tag -> prefab")]
    public PowerEntry[] entries;

    public void AddIcons(string tagName, int count)
    {
        if (count <= 0) return;

        var entry = FindEntry(tagName);
        if (entry == null || entry.prefab == null)
        {
            Debug.LogWarning($"[PowerInventory] No prefab mapped for tag '{tagName}'.");
            return;
        }

        int addUses = Mathf.Max(1, entry.defaultUsesPerIcon) * count;

        var existing = FindExistingIcon(tagName);
        if (existing != null)
        {
            existing.remainingUses += addUses;
            existing.UpdateRemainingUsesText();

            existing.transform.SetAsLastSibling();
            return;
        }
        SpawnIcon(entry, tagName, addUses);
    }

    private void SpawnIcon(PowerEntry entry, string powerId, int startingUses)
    {
        var go = Instantiate(entry.prefab, powerContainer);
        var drag = go.GetComponent<TestingToTakeAwayTile>();
        if (drag != null)
        {
            drag.powerId = powerId;      
            drag.remainingUses = Mathf.Max(1, startingUses);
            drag.UpdateRemainingUsesText();
        }
    }

    private TestingToTakeAwayTile FindExistingIcon(string powerId)
    {
        foreach (Transform child in powerContainer)
        {
            var icon = child.GetComponent<TestingToTakeAwayTile>();
            if (icon != null && icon.powerId == powerId)
                return icon;
        }
        return null;
    }

    private PowerEntry FindEntry(string tagName)
    {
        foreach (var e in entries)
            if (e.tagName == tagName) return e;
        return null;
    }
}
