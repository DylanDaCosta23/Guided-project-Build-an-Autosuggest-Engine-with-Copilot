public class TrieNode
{
    public Dictionary<char, TrieNode> Children { get; set; }
    public bool IsEndOfWord { get; set; }

    public char _value;

    public TrieNode(char value = ' ')
    {
        Children = new Dictionary<char, TrieNode>();
        IsEndOfWord = false;
        _value = value;
    }

    public bool HasChild(char c)
    {
        return Children.ContainsKey(c);
    }
}

public class Trie
{
    private TrieNode root;

    public Trie()
    {
        root = new TrieNode();
    }

    // Search for a word in the trie
public bool Search(string word) 
{
    TrieNode current = root;
    foreach (char c in word) 
    {
        if (!current.HasChild(c)) 
        {
            return false;
        }
        current = current.Children[c];
    }
    return current.IsEndOfWord;
}

    public bool Insert(string word)
    {
        TrieNode current = root;
        // For each character in the word, traverse the Trie
        foreach (char c in word)
        {
            // If the character node does not exist, create it
            if (!current.HasChild(c))
            {
                // Create a new TrieNode for the character
                current.Children[c] = new TrieNode(c);
            }
            // Move to the child node
            current = current.Children[c];
        }
        // Mark the end of the word
        if (current.IsEndOfWord)
        {
            // The word already exists in the Trie
            return false;
        }
        // Mark the current node as the end of a word
        current.IsEndOfWord = true;
        // Successfully inserted the word
        return true;
    }
    
    /// <summary>
    /// Retrieves all dictionary words stored in the trie that begin with the specified <paramref name="prefix"/>.
    /// </summary>
    /// <param name="prefix">
    /// The prefix to search for. If the prefix characters do not exist sequentially in the trie,
    /// an empty <see cref="List{T}"/> is returned.
    /// </param>
    /// <returns>
    /// A list of words (strings) that start with the given prefix. Returns an empty list if:
    /// 1) <paramref name="prefix"/> is null or empty (in which case all words from root could be returned depending on upstream validation), or
    /// 2) The prefix path does not exist in the trie.
    /// </returns>
    /// <remarks>
    /// This method first traverses the trie following each character of the prefix. If at any point a character
    /// node is missing, no suggestions are possible and an empty list is returned. Otherwise, a depth-first
    /// collection of all terminal words beneath the last prefix node is performed.
    /// Time complexity:
    ///   O(p + k) where p = length of prefix, k = total characters in the suggested words gathered.
    /// Space complexity:
    ///   O(k) for the list of results.
    /// </remarks>
    /// <example>
    /// var suggestions = trie.AutoSuggest("ap");
    /// // Possible result: ["apple", "apply", "apt"]
    /// </example>
    public List<string> AutoSuggest(string prefix)
    {
        TrieNode currentNode = root;
        foreach (char c in prefix)
        {
            if (!currentNode.HasChild(c))
            {
                return new List<string>();
            }
            currentNode = currentNode.Children[c];
        }
        return GetAllWordsWithPrefix(currentNode, prefix);
    }

    private List<string> GetAllWordsWithPrefix(TrieNode root, string prefix)
    {
        List<string> words = new List<string>();
        if (root.IsEndOfWord)
        {
            words.Add(prefix);
        }

        foreach (var child in root.Children)
        {
            words.AddRange(GetAllWordsWithPrefix(child.Value, prefix + child.Key));
        }

        return words;
    }

    // Helper method to delete a word from the trie by recursively removing its nodes
    private bool _delete(TrieNode root, string word, int index)
{
    // Base case: we've reached the end of the word
    if (index == word.Length)
    {
        // If the current node is not the end of a word, we can't delete it
        if (!root.IsEndOfWord)
        {
            return false;
        }
        // Otherwise, we can delete it by setting the end of word flag to false
        root.IsEndOfWord = false;
        // And returning true to indicate that the node can be deleted
        return true;
    }
    // Recursive case: we haven't reached the end of the word yet
    char currentChar = word[index];
    // If the current node doesn't have a child with the current character, we can't delete it
    if (!root.HasChild(currentChar))
    {
        return false;
    }
    // Otherwise, we can delete it if its child can be deleted
    bool canDelete = _delete(root.Children[currentChar], word, index + 1);
    // If the child can be deleted and the current node is not the end of a word, we can delete it
    if (canDelete && !root.IsEndOfWord)
    {
        root.Children.Remove(currentChar);
        return true;
    }
    // Otherwise, we can't delete it
    return false;
}

    public bool DeleteWord(string word)
    {
        return _delete(root, word, 0);
    }

    // Delete a word from the trie
public bool Delete(string word)
{
    TrieNode current = root;
    foreach (char c in word)
    {
        if (!current.HasChild(c))
        {
            // Word doesn't exist in trie
            return false;
        }
        current = current.Children[c];
    }
    if (!current.IsEndOfWord)
    {
        // Word doesn't exist in trie
        return false;
    }
    // Word exists in trie
    // Set IsEndOfWord to false
    current.IsEndOfWord = false;
    return true;
}

    public List<string> GetAllWords()
    {
        return GetAllWordsWithPrefix(root, "");
    }

    public void PrintTrieStructure()
    {
        Console.WriteLine("\nroot");
        _printTrieNodes(root);
    }

    private void _printTrieNodes(TrieNode root, string format = " ", bool isLastChild = true) 
    {
        if (root == null)
            return;

        Console.Write($"{format}");

        if (isLastChild)
        {
            Console.Write("└─");
            format += "  ";
        }
        else
        {
            Console.Write("├─");
            format += "│ ";
        }

        Console.WriteLine($"{root._value}");

        int childCount = root.Children.Count;
        int i = 0;
        var children = root.Children.OrderBy(x => x.Key);

        foreach(var child in children)
        {
            i++;
            bool isLast = i == childCount;
            _printTrieNodes(child.Value, format, isLast);
        }
    }

    public List<string> GetSpellingSuggestions(string word)
    {
        char firstLetter = word[0];
        List<string> suggestions = new();
        List<string> words = GetAllWordsWithPrefix(root.Children[firstLetter], firstLetter.ToString());
        
        foreach (string w in words)
        {
            int distance = LevenshteinDistance(word, w);
            if (distance <= 2)
            {
                suggestions.Add(w);
            }
        }

        return suggestions;
    }

    private int LevenshteinDistance(string s, string t)
    {
        int m = s.Length;
        int n = t.Length;
        int[,] d = new int[m + 1, n + 1];

        if (m == 0)
        {
            return n;
        }

        if (n == 0)
        {
            return m;
        }

        for (int i = 0; i <= m; i++)
        {
            d[i, 0] = i;
        }

        for (int j = 0; j <= n; j++)
        {
            d[0, j] = j;
        }

        for (int j = 1; j <= n; j++)
        {
            for (int i = 1; i <= m; i++)
            {
                int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
                d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
            }
        }

        return d[m, n];
    }
}