using System.Collections.ObjectModel;

namespace CleanFood
{
    class TrieNode
    {
        public TrieNode[] next = new TrieNode[28];
        public int count = 0;
        public ObservableCollection<String> displayNames = new ObservableCollection<String>();

        public bool ContainsKey(int i)
        {
            return next[i] != null;            
        }

        public void Put(int i, TrieNode node)
        {
            next[i] = node;            
        }

        public TrieNode Get(int i)
        {
            return next[i];
        }
    }

    public class NameTrie
    {
        TrieNode root;

        public NameTrie()
        {
            root = new TrieNode();
        }

        public void Insert(string word, string displayName)
        {
            TrieNode node = root;
            word = word.ToLower();
            int j;
            foreach (char c in word.ToCharArray())
            {
                if (c == ' ')
                {
                    j = 26;
                }
                else if (c == ',')
                {
                    j = 27;
                }
                else
                {
                    j = c-'a';
                }

                if ((j < 0) || (j > 27))
                {
                    Console.WriteLine("########### " + j + " is out of bounds!");
                }

                if (node.ContainsKey(j) == false)
                {
                    node.Put(j, new TrieNode());
                }
                node = node.Get(j);
                node.displayNames.Add(displayName);
                node.count++;
            }
        }

        public ObservableCollection<String> GetNames(string entry)                // modify this to store facility.DisplayNames directly instead of Indexes
        {
            TrieNode node = root;
            int j;
            foreach (char c in entry.ToCharArray())
            {
                if (c == ' ')
                {
                    j = 26;
                }
                else if (c == ',')
                {
                    j = 27;
                }
                else
                {
                    j = c - 'a';
                }

                if (node.ContainsKey(j) == false)
                {
                    return new ObservableCollection<String>(); // Entry not found
                }
                node = node.Get(j);
            }
            return node.displayNames;
        }
    }
}

