using UnityEngine;

namespace RogueNinjaChase.Player.Aim
{
    public interface IAimSource
    {
        Vector3 Forward { get; }
        Vector3 Right { get; }
    }
}
