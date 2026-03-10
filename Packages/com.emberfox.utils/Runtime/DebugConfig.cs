using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "DebugConfig")]
public class DebugConfig : CustomSettings<DebugConfig>
{
    public bool instantBattle;
    public bool superPoweredHero;
}
