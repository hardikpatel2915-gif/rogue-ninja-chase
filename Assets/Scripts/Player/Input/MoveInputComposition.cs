using RogueNinjaChase.Player.Model;
using UnityEngine;

namespace RogueNinjaChase.Player.Input
{
    /// <summary>
    /// Stick X is strafe. Stick Y is forward. Both held flags are passed through.
    /// </summary>
    public static class MoveInputComposition
    {
        public static MoveInput Create(Vector2 move, bool sprint, bool crouch)
        {
            return new MoveInput(move.x, move.y, sprint, crouch);
        }
    }
}
