using UnityEngine;
using System.Collections.Generic;
using System;

public class UIRegistry
{
    Dictionary<string, Node> nodes = new Dictionary<string, Node>();

    public void Register(string key, Node value)
    {
        nodes.Add(key, value);
    }

    public Node Get(string key)
    { 
        Node result;
        if (nodes.TryGetValue(key, out result))
        {
            return result;
        }
        else
        {
            throw new Exception($"there is no value presented for {key} key in ui-registry.");
        }
    }

    [System.Serializable]
    public class Node
    {
        public string Name;
        public GameObject Element;

        public Node(string name, GameObject element)
        {
            Name = name;
            Element = element;
        }
    }
}