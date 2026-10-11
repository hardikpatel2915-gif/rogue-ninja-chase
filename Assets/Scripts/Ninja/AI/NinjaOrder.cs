namespace RogueNinjaChase.Ninja.AI
{
    public readonly struct NinjaOrder
    {
        public readonly NinjaState State;
        public readonly NinjaDestination Destination;
        public readonly float Speed;

        public NinjaOrder(NinjaState state, NinjaDestination destination, float speed)
        {
            State = state;
            Destination = destination;
            Speed = speed;
        }
    }
}
