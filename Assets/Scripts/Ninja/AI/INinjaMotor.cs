using UnityEngine;

namespace RogueNinjaChase.Ninja.AI
{
    public interface INinjaMotor
    {
        bool MoveTowards(Vector3 destination, float speed);
    }
}
