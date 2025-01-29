namespace Gripe.Api.Utils;

public static class FuzzySearch
{
    public static List<T> MatchAll<T>(
        string searchTerm, List<T> list,
        Func<T, string> propertySelector,
        int maxDistance = 2, bool ignoreCase = true)
    {
        if (list == null)
            throw new ArgumentNullException(nameof(list));

        // Return all items if the search term is empty or null
        if (string.IsNullOrEmpty(searchTerm))
            return new List<T>(list);

        // Preprocess the search term
        string processedSearchTerm = ignoreCase ?
            searchTerm.Trim().ToLowerInvariant() :
            searchTerm.Trim();

        var resultsWithDistance = new List<Tuple<T, int>>();

        foreach (var item in list)
        {
            var prop = propertySelector(item);
            if (item == null || prop.GetType() != typeof(string))
                continue;

            string? itemStr = prop as string;
            string processedItem = ignoreCase ?
                itemStr!.Trim().ToLowerInvariant() :
                itemStr!.Trim();

            int distance = LevenshteinDistance(processedSearchTerm, processedItem);
            if (distance <= maxDistance)
            {
                resultsWithDistance.Add(Tuple.Create(item, distance));
            }
        }

        return resultsWithDistance
            .OrderBy(t => t.Item2)
            .Select(t => t.Item1)
            .ToList();
    }

    private static int LevenshteinDistance(string a, string b)
    {
        if (string.IsNullOrEmpty(a))
            return string.IsNullOrEmpty(b) ? 0 : b.Length;
        if (string.IsNullOrEmpty(b))
            return a.Length;

        // Use a single array to store costs, reducing space complexity
        int[] costs = new int[b.Length + 1];
        for (int i = 0; i <= b.Length; i++)
            costs[i] = i;

        for (int i = 1; i <= a.Length; i++)
        {
            costs[0] = i;
            int previousDiagonal = i - 1;
            for (int j = 1; j <= b.Length; j++)
            {
                int temp = costs[j];
                if (a[i - 1] == b[j - 1])
                {
                    costs[j] = previousDiagonal;
                }
                else
                {
                    costs[j] = Math.Min(Math.Min(costs[j - 1], costs[j]), previousDiagonal) + 1;
                }
                previousDiagonal = temp;
            }
        }

        return costs[b.Length];
    }
}