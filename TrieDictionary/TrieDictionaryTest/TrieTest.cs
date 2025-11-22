namespace TrieDictionaryTest;

[TestClass]
public class TrieTest
{
    // Test that a word is inserted in the trie
    [TestMethod]
    public void TestInsertWord()
    {
        Trie trie = new Trie();
        bool inserted = trie.Insert("hello");
        Assert.IsTrue(inserted);
        Assert.IsTrue(trie.Search("hello"));
    }

// Test that a word is deleted from the trie
[TestMethod]
public void TestDeleteWord()
{
    Trie trie = new Trie();
    trie.Insert("hello");
    bool deleted = trie.Delete("hello");
    Assert.IsTrue(deleted);
    Assert.IsFalse(trie.Search("hello"));
}

// Test that a word is not inserted twice in the trie
[TestMethod]
public void TestInsertDuplicateWord()
{
    Trie trie = new Trie();
    trie.Insert("hello");
    bool insertedAgain = trie.Insert("hello");
    Assert.IsFalse(insertedAgain);
}

// Test that a word is deleted from the trie
[TestMethod]
public void TestDeleteNonExistentWord()
{
    Trie trie = new Trie();
    bool deleted = trie.Delete("hello");
    Assert.IsFalse(deleted);
}

// Test that a word is not deleted from the trie if it is not present
[TestMethod]
public void TestDeletePrefixWord()
{
    Trie trie = new Trie();
    trie.Insert("hello");
    bool deleted = trie.Delete("hel");
    Assert.IsFalse(deleted);
    Assert.IsTrue(trie.Search("hello"));
}

// Test that a word is deleted from the trie if it is a prefix of another word
[TestMethod]
public void TestDeleteWordWithPrefix()
{
    Trie trie = new Trie();
    trie.Insert("hello");
    trie.Insert("helium");
    bool deleted = trie.Delete("hello");
    Assert.IsTrue(deleted);
    Assert.IsFalse(trie.Search("hello"));
    Assert.IsTrue(trie.Search("helium"));
}

// Test AutoSuggest for the prefix "cat" not present in the 
// trie containing "catastrophe", "catatonic", and "caterpillar"
[TestMethod]
public void TestAutoSuggestPrefixNotPresent()
{
    Trie trie = new Trie();
    trie.Insert("catastrophe");
    trie.Insert("catatonic");
    trie.Insert("caterpillar");
    List<string> suggestions = trie.AutoSuggest("cat");
    Assert.AreEqual(3, suggestions.Count);
    CollectionAssert.Contains(suggestions, "catastrophe");
    CollectionAssert.Contains(suggestions, "catatonic");
    CollectionAssert.Contains(suggestions, "caterpillar");
}


// Test GetSpellingSuggestions for a word not present in the trie
[TestMethod]
public void TestGetSpellingSuggestions()
{
    Trie trie = new Trie();
    trie.Insert("hello");
    trie.Insert("hell");
    trie.Insert("help");
    List<string> suggestions = trie.GetSpellingSuggestions("helo");
    Assert.AreEqual(3, suggestions.Count);
    CollectionAssert.Contains(suggestions, "hello");
    CollectionAssert.Contains(suggestions, "hell");
    CollectionAssert.Contains(suggestions, "help");
}
}