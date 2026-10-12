using UnityEngine;

namespace RogueNinjaChase.Core
{
    public interface INoiseListener
    {
        Vector3 Position { get; }
        void Hear(Vector3 position);
    }
}
