namespace Hackathon
{
    public class RandomWishlistGenerator : IWishListGenerator
    {
        static readonly Random Rng = new Random();

        private static void Shuffle<T>(List<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n -= 1;
                var k = Rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        public List<Employee> CreateWishlist<T>(List<T> participants)
        {
            var participantsCopy = new List<Employee>((IEnumerable<Employee>)participants);
            Shuffle(participantsCopy);
            return participantsCopy;
        }
    }
}