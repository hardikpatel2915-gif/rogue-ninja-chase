using RogueNinjaChase.Player.Model;
using UnityEngine;

namespace RogueNinjaChase.Player.Input
{
    public static class MoveInputComposition
    {
        public static MoveInput Create(Vector2 move, bool sprint, bool crouch)
        {
            return new MoveInput(move.x, move.y, sprint, crouch);
        }
    }
}
