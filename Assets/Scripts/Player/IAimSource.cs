using UnityEngine;

namespace RogueNinjaChase.Player
{
    public interface IAimSource
    {
        Vector3 Forward { get; }
        Vector3 Right { get; }
    }
}
