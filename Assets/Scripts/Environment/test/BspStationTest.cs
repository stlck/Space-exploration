using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class BspStationTest : MonoBehaviour {

    int[,] map;
    public int iterations = 2;
    BspCell root;
    public int size = 60;
    float timer = 0;

	// Use this for initialization
	void Start () {
        map = new int[size, size];
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if(timer > .5f)
        {
            timer = 0f;
            generate();
        }
	}

    void generate()
    {
        map = new int[size, size];
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                map[i, j] = 0;

        root = new BspCell(size/2 ,size/2, size, size);
        var cite = iterations;
        split(root, cite);
        toMap(root);
    }

    void toMap(BspCell cell)
    {
        if (cell.child1 != null && cell.child2 != null)
        {
            toMap(cell.child1);
            toMap(cell.child2);
        }
        else
        {
            for (int i = cell.x - (cell.w - 1) / 2; i < cell.x + (cell.w -1) / 2; i++)
                for (int j = cell.y - (cell.h-1) / 2; j < cell.y + (cell.h - 1) / 2; j++)
                {
                    map[i, j] = 1;
                }
        }
    }

    void split(BspCell cell, int i)
    {
        i --;
        if (i == 0)
            return;

        var dir = Random.value;
        if(dir > 0.5f)
        {
            var nCell1 = new BspCell(cell.x - cell.w/4, cell.y, cell.w / 2, cell.h);
            cell.child1 = nCell1;
            split(nCell1, i);

            var nCell2 = new BspCell(cell.x + cell.w/4, cell.y, cell.w / 2, cell.h);
            cell.child2 = nCell2;
            split(nCell2, i);
        }
        else
        {
            var nCell1 = new BspCell(cell.x , cell.y - cell.h /4, cell.w, cell.h / 2);
            cell.child1 = nCell1;
            split(nCell1, i);

            var nCell2 = new BspCell(cell.x, cell.y + cell.h / 4, cell.w, cell.h / 2);
            cell.child2 = nCell2;
            split(nCell2, i);
        }
    }

    void connect(BspCell cell1, BspCell cell2)
    {

    }

    void OnDrawGizmos()
    {
        if (map == null)
            return;
        //if(root != null)
        //{
        //    showCell(root);
        //}
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                if (map[i, j] == 1)
                    Gizmos.DrawCube(Vector3.forward * j + Vector3.right * i, Vector3.one * .8f);
    }

    void showCell(BspCell cell)
    {
        if(cell.child1 != null && cell.child2 != null)
        {
            showCell(cell.child1);
            showCell(cell.child2);
        }
        else
        {
            Gizmos.DrawCube(Vector3.right * cell.x + Vector3.forward * cell.y, Vector3.up + Vector3.right * (cell.w -1 ) + Vector3.forward * (cell.h - 1));
        }
    }
}

public class BspCell
{
    public int x;
    public int y;

    public int w;
    public int h;

    public BspCell child1;
    public BspCell child2;
    public BspCell(int _x, int _y, int _w, int _h)
    {
        x = _x;
        y = _y;
        w = _w;
        h = _h;
    }
}

/*
public class Node<T>
{
    // Private member-variables
    private T data;
    private NodeList<T> neighbors = null;

    public Node() { }
    public Node(T data) : this(data, null) { }
    public Node(T data, NodeList<T> neighbors)
    {
        this.data = data;
        this.neighbors = neighbors;
    }

    public T Value
    {
        get
        {
            return data;
        }
        set
        {
            data = value;
        }
    }

    protected NodeList<T> Neighbors
    {
        get
        {
            return neighbors;
        }
        set
        {
            neighbors = value;
        }
    }
}

public class NodeList<T> : Collection<Node<T>>
{
    public NodeList() : base() { }

    public NodeList(int initialSize)
    {
        // Add the specified number of items
        for (int i = 0; i < initialSize; i++)
            base.Items.Add(default(Node<T>));
    }

    public Node<T> FindByValue(T value)
    {
        // search the list for the value
        foreach (Node<T> node in Items)
            if (node.Value.Equals(value))
                return node;

        // if we reached here, we didn't find a matching node
        return null;
    }
}

public class BinaryTreeNode<T> : Node<T>
{
    public BinaryTreeNode() : base() { }
    public BinaryTreeNode(T data) : base(data, null) { }
    public BinaryTreeNode(T data, BinaryTreeNode<T> left, BinaryTreeNode<T> right)
    {
        base.Value = data;
        NodeList<T> children = new NodeList<T>(2);
        children[0] = left;
        children[1] = right;

        base.Neighbors = children;
    }

    public BinaryTreeNode<T> Left
    {
        get
        {
            if (base.Neighbors == null)
                return null;
            else
                return (BinaryTreeNode<T>)base.Neighbors[0];
        }
        set
        {
            if (base.Neighbors == null)
                base.Neighbors = new NodeList<T>(2);

            base.Neighbors[0] = value;
        }
    }

    public BinaryTreeNode<T> Right
    {
        get
        {
            if (base.Neighbors == null)
                return null;
            else
                return (BinaryTreeNode<T>)base.Neighbors[1];
        }
        set
        {
            if (base.Neighbors == null)
                base.Neighbors = new NodeList<T>(2);

            base.Neighbors[1] = value;
        }
    }
}

public class BinaryTree<T>
{
    private BinaryTreeNode<T> root;

    public BinaryTree()
    {
        root = null;
    }

    public virtual void Clear()
    {
        root = null;
    }

    public BinaryTreeNode<T> Root
    {
        get
        {
            return root;
        }
        set
        {
            root = value;
        }
    }
}
*/