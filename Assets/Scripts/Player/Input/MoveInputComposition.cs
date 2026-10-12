using RogueNinjaChase.Player.Model;
using UnityEngine;

namespace RogueNinjaChase.Player.Input
{
    /// <summary>
    /// Stick X is strafe. Stick Y is forward. Sprint and crouch are held. Jump is one press.
    /// </summary>
    public static class MoveInputComposition
    {
        public static MoveInput Create(Vector2 move, bool sprint, bool crouch, bool jump = false)
        {
            return new MoveInput(move.x, move.y, sprint, crouch, jump);
        }
    }
}
