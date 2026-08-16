using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;

class PreFix
{
    private class Node
    {
        public char Value { get; set; }
        public List<Node> Children { get; set; }
        public Node Parent { get; set; }
        public int Depth { get; set; }
        public int Count { get; set; }

        public Node(char value, int depth, Node parent)
        {
            Value = value;
            Children = new List<Node>();
            Depth = depth;
            Parent = parent;

        }

        public bool IsLeaf()
        {
            return Children.Count == 0;
        }

        public Node FindChildNode(char c)
        {
            foreach (var child in Children)
                if (child.Value == c)
                    return child;

            return null;
        }

        public void DeleteChildNode(char c)
        {
            for (var i = 0; i < Children.Count; i++)
                if (Children[i].Value == c)
                    Children.RemoveAt(i);
        }
    }

    public class PreFixTrie
    {

        static public Dictionary<string, int> diction = GetData.WordID;

        static public readonly Lazy <PreFixTrie> tr = new(()=> buildtire(diction));
        static public  PreFixTrie trie => tr.Value;


        WordRepository database = new();
        private readonly Node _root;

        public PreFixTrie()
        {
            _root = new Node('^', 0, null);
        }

        private Node Prefix(string s)
        {
            var currentNode = _root;
            var result = currentNode;
  
            foreach (var c in s)
            {
                currentNode = currentNode.FindChildNode(c);
                if (currentNode == null) break;

                result = currentNode;
            }


            return result;
        }

        public bool Search(string s)
        {
            var prefix = Prefix(s);
            return prefix.Depth == s.Length || prefix.FindChildNode('$') != null;
        }

        private void InsertRange(List<string> items)
        {
            for (int i = 0; i < items.Count; i++)
                Insert(items[i]);
        }

        private void Insert(string s)
        {
            var commonPrefix = Prefix(s);
            var current = commonPrefix;

            for (var i = current.Depth; i < s.Length; i++)
            {

                var newNode = new Node(s[i], current.Depth + 1, current);

                current.Children.Add(newNode);
                current = newNode;
            }

            current.Children.Add(new Node('$', current.Depth + 1, current));
        }

        public void Delete(string s)
        {
            if (Search(s))
            {
                var node =Prefix(s).FindChildNode('$');

                while (node.IsLeaf())
                {
                    var parent = node.Parent;
                    parent.DeleteChildNode(node.Value);
                    node = parent;
                }
            }
        }

        private void SetPreFixList(string word, List<string> l)
        {
            //check if word exists
            if (!Search(word))
            {

                l.Clear();

                return;
            }


            Node nodes =Prefix(word);

            string matched = word.Substring(0, Math.Min(word.Length, nodes.Depth));
            foreach (var node in nodes.Children)
            {
                if (node.Value == '$') continue;

                var current = matched + node.Value;


                if (node.Children.Any(f => f.Value != '$')) SetPreFixList(current, l);
                
                l.Add(current);
            }

        }


        private bool DoesHaveRoot(string W)
        {
            return Prefix(W).Parent != null ? true : false;
        }

        public List<string> GetPreFixList(string word)
        {
            
            List<string> l = new();
            SetPreFixList(word, l);

            
            return l;
        }

        public static PreFixTrie TrainEntry(List<string> wordslist)
        {
            PreFixTrie trie = new();
            trie.InsertRange(wordslist);
            return trie;
        }


        public List<string> GetCandidate(string word)
        {
            word = word.ToLower();
            var t = Stopwatch.StartNew();
            t.Start();

            List<string> items = trie.GetPreFixList(word);
            if (items == null || items.Count == 0) return new();
            var candidateWords = items
                 .OrderByDescending(f => GetFrq(f))
                 .Take(10)
                 .ToList();


            return candidateWords;

        }

        private static PreFixTrie buildtire(Dictionary<string,int> dic)
        {
            List<string> l = new();
            foreach (var s in dic)
            {
                l.Add(s.Key);
            }

            return TrainEntry(l);
        }

        private bool CheckWordIfTrue(string word)
        {
            Dictionary<string, int> dicword = GetData.WordID;
            Dictionary<int, (string, long)> dic = GetData.diction;
            if (dicword.TryGetValue(word, out int ID))
            {
                if (dic.TryGetValue(ID, out (string, long) val))
                {
                    return true;
                }
            }
            return false;
        }

        private long GetFrq(string word)
        {

            Dictionary<string, int> dicword = GetData.WordID;
            Dictionary<int, (string, long)> dic = GetData.diction;
            if (dicword.TryGetValue(word, out int ID))
            {
                if (dic.TryGetValue(ID, out (string, long) val))
                {
                    return val.Item2;
                }
            }
            return 0;

        }

    }


}

