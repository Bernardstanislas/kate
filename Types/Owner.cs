namespace Kate.Types
{
    public enum Owner
    {
        Me,
        Opponent,
        Humans,
        Neutral
    }

    public static class OwnerExtensions
    {
        public static Owner Opposite(this Owner owner)
        {
            return owner == Owner.Me ? Owner.Opponent : owner == Owner.Opponent ? Owner.Me : owner;
        }
    }
}
