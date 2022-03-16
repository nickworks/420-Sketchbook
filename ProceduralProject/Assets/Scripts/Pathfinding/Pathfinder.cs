using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinder
{
    public class Node {

        public Vector3 position;

        public float G {get; private set;}
        public float H {get; private set;}
        public float F {
            get {
                return G + H;
            }
        }
        public float moveCost = 1;
        
        public List<Node> neighbors = new List<Node>();

        private Node _parent;
        public Node parent {
            get {
                return _parent;
            }
            set {
                _parent = value;
                if(_parent != null){
                    G = _parent.G + moveCost;
                } else {
                    G = 0;
                }
            }
        }
        public void DoHeuristic(Node end){
            Vector3 d = end.position - this.position;
            H = d.magnitude;
        }
    }
    public static List<Node> Solve(Node start, Node end){

        List<Node> open = new List<Node>();
        List<Node> closed = new List<Node>();

        start.parent = null;
        open.Add(start);

        // 1. travel from start to end
        while(open.Count > 0){

            // find node in OPEN list with SMALLEST F value
            float bestF = 0;
            Node current = null;
            foreach(Node n in open){
                if(n.F < bestF || current == null){
                    current = n;
                    bestF = n.F;
                }
            }

            if(current == end){
                break;
            }
            bool isDone = false;
            foreach(Node neighbor in current.neighbors){
                
                if(!closed.Contains(neighbor)){ // node not in CLOSED:

                    if(!open.Contains(neighbor)){ // node not in OPEN:

                        open.Add(neighbor);
                        neighbor.parent = current;
                        if(neighbor == end){
                            isDone = true;
                        }
                        neighbor.DoHeuristic(end);

                    } else { // node already in OPEN:

                        // if this path to neighbor has lower G
                        // than previous path to neighbor...
                        if(current.G + neighbor.moveCost < neighbor.G ){
                            neighbor.parent = current;
                        }
                    }
                }
            }
            if(isDone) break;
        }


        // 2. travel from end to start, building path
        List<Node> path = new List<Node>();
        /*
        Node temp = end;
        while(temp != null){
            path.Add(temp);
            temp = temp.parent;
        }
        */
        for(Node temp = end; temp != null; temp = temp.parent){
            path.Add(temp);
        }

        // 3. reverse path
        path.Reverse();

        return path;

    }    
}
/*

Dijkstra - 
    Keep a list of OPEN nodes
    Foreach node:
        Record how far node is from start
        Add neighbors to OPEN list
            If is END, return chain of nodes
        Move to CLOSED list

Greedy Best-Search - 
    Keep a list of OPEN nodes
    Pick one node most likely to be closer to END // Heuristic
        Add neighbors to OPEN list
            If is END, return chain of nodes
        Move to CLOSED list

A* - 
    Keep a list of OPEN nodes
    Pick one node with lowest cost (cost = how far start + how far from end)
        Add neighbors to OPEN list

            Record how far node is from start
            If is END, return chain of nodes
        Move to CLOSED list

    F = G + H
    Heurestic
        Euclidean (line to end)
        Manhattan (dx + dy)

*/
