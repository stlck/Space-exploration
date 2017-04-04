using UnityEngine;
using System.Collections.Generic;

///////////////////////
//
// A simple 2D implementation of the best first search algoirithm (robotics motion planning)
//
public class BestFirstSearch
{
    ///////////////////////
    //
    // A search node - nodes are visited from a neighboring node in the grid
    //
    public class Node
    {
        public int x;           // the grid (x, y) coordinate for the node
        public int y;
        public Node parent;     // the neighboring node (the source of the visit)
        public float cost;

        public Node(int _x, int _y, float _cost, Node _parent)
        {
            x = _x;
            y = _y;
            cost = _cost;
            parent = _parent;
        }
    }

    private bool[,] m_collisiongrid;    // the collision grid (true == obstacle)
    private bool[,] m_visit;            // track the grid points that have already been visited in the search
    private int m_width;                // the width of the grid
    private int m_height;               // the height of the grid
    private int m_startx;               // the start (x, y) coordinate
    private int m_starty;
    private List<Node>[] m_open;        // the OPEN list (these are the promising nodes that we've visited)
    private int m_best;                 // the index in our OPEN list of the best (most promising) node (i.e. a priority queue)

    public BestFirstSearch() { }

    ///////////////////////
    //
    // Constructor - allocate space for the width x height grid 
    //	
    public BestFirstSearch(int width, int height)
    {
        m_collisiongrid = new bool[width, height];
        m_visit = new bool[width, height];
        m_width = width;
        m_height = height;

        resetOpen(width, height);
    }

    void resetOpen(int width, int height)
    {
        m_open = new List<Node>[width + height];
        for (int i = 0; i < width + height; i++)
            m_open[i] = new List<Node>();
    }

    ///////////////////////
    //
    // Add obstacles to the grid
    //	
    public void AddObstacle(int x, int y)
    {
        if (TestWidthBounds(x) && TestHeightBounds(y))
            m_collisiongrid[x, y] = true;
    }


    ///////////////////////
    //
    // Find a path from (startx, starty) to (goalx, goaly)
    //
    public List<Vector3> FindPath(int startx, int starty, int goalx, int goaly, float targetAlt)
    {
        var ret = new List<Vector3>();
        if (TestWidthBounds(startx) == false)
            return ret;
        if (TestHeightBounds(starty) == false)
            return ret;
        if (TestWidthBounds(goalx) == false)
            return ret;
        if (TestHeightBounds(goaly) == false)
            return ret;

        // we actually search from goal to start
        m_startx = startx;
        m_starty = starty;

        // clear the visit lookup
        System.Array.Clear(m_visit, 0, m_width * m_height);
        resetOpen(m_width, m_height);

        // initialize the best index to the max value (no best yet)
        m_best = m_width + m_height;

        // insert the node into the OPEN list (if it's legal)
        Insert(null, goalx, goaly);

        // pull the best node and start searching
        Node n = GetBest();
        while (n != null)
        {
            if (n.x == startx && n.y == starty)
                break;

            // visit the neighbors
            Visit(n, n.x - 1, n.y + 1);
            Visit(n, n.x, n.y + 1);
            Visit(n, n.x + 1, n.y + 1);
            Visit(n, n.x - 1, n.y);
            Visit(n, n.x + 1, n.y);
            Visit(n, n.x - 1, n.y - 1);
            Visit(n, n.x, n.y - 1);
            Visit(n, n.x + 1, n.y - 1);

            n = GetBest();
        }

        if (n == null)
        {
            Debug.LogError("No path found");
            return ret;
        }


        //Debug.Log("The following path was found:");
        while (n != null)
        {
            string coord = string.Format("{0}, {1}", n.x, n.y);
            //Debug.Log(coord);
            ret.Add(new Vector3(n.x, targetAlt, n.y));
            n = n.parent;
        }

        return ret;
    }


    ///////////////////////
    //
    // Visit a neighboring node and insert it into the OPEN list if it is valid
    //	
    void Visit(Node parent, int x, int y)
    {
        if (TestWidthBounds(x) && TestHeightBounds(y))
        {
            if (m_visit[x, y])
                return;

            if (m_collisiongrid[x, y])
                return;

            m_visit[x, y] = true;

            Insert(parent, x, y);
        }
    }

    ///////////////////////
    //
    // Helper function to sort the bins of our OPEN list
    //	
    private static int CompareNodes(Node x, Node y)
    {
        if (x.cost == y.cost)
            return 0;
        if (x.cost < y.cost)
            return 1;
        else
            return -1;
    }

    ///////////////////////
    //
    // Insert a node into the OPEN List (a hash table)
    //	
    void Insert(Node parent, int x, int y)
    {
        // the heuristic for the search is the distance to the start point
        float distance = Mathf.Sqrt((x - m_startx) * (x - m_startx) + (y - m_starty) * (y - m_starty));

        // also our hash function
        int index = (int)distance;

        m_open[index].Add(new Node(x, y, distance, parent));
        m_open[index].Sort(CompareNodes);

        // keep track of the best index
        if (index < m_best)
            m_best = index;
    }

    ///////////////////////
    //
    // Extract the best node (within a bucket the items are sorted with the best node at the end of the list)
    //	
    Node GetBest()
    {
        if (m_best < (m_width + m_height))
        {
            Node best = m_open[m_best][m_open[m_best].Count - 1];
            m_open[m_best].RemoveAt(m_open[m_best].Count - 1);

            // after removing the best node, if the bucket is empty
            // we need to find the new best bucket
            if (m_open[m_best].Count == 0)
            {
                // find the next best bucket
                while (m_best < (m_width + m_height))
                {
                    if (m_open[m_best].Count > 0)
                        break;
                    m_best++;
                }
            }

            return best;
        }

        return null;
    }

    ///////////////////////
    //
    // Helper functions to ensure the coordinates are valid
    //	
    bool TestWidthBounds(int q)
    {
        if (q >= 0 && q < m_width)
            return true;
        else
            return false;
    }

    ///////////////////////
    //
    // Helper functions to ensure the coordinates are valid
    //
    bool TestHeightBounds(int q)
    {
        if (q >= 0 && q < m_height)
            return true;
        else
            return false;
    }
}
