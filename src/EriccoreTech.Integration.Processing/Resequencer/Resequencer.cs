namespace EriccoreTech.Integration.Processing {
    public sealed class Resequencer {

        public IReadOnlyList<T> Reorder<T>(IEnumerable<T> items, Func<T, long> sequence) {
            return items.OrderBy(sequence).ToArray();
        }

    }
}
