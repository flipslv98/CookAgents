﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMap : MonoBehaviour
{

    private bool DEBUG = true;

    private readonly int maxWaitTime = 5;

    private List<GameObject> otherPlayers = new List<GameObject>();

    public GameObject ground;

    public GameObject specials;

    private List<Vector3> positions = new List<Vector3>();

    private List<Vector3> intersections = new List<Vector3>();

    private List<Vector3> stoves = new List<Vector3>();

    private List<Vector3> onionDisp = new List<Vector3>();

    private List<Vector3> cuttingBoards = new List<Vector3>();

    private List<Transform> onions = new List<Transform>();

    private List<Transform> tomatos = new List<Transform>();

    private List<Transform> lettuce = new List<Transform>();

    private List<Transform> pans = new List<Transform>();

    private List<Transform> plates = new List<Transform>();

    private List<Transform> soups = new List<Transform>();

    private Transform delivery;

    private Vector3 plateReturn;

    void Start()
    {
        this.ground = GameObject.FindGameObjectWithTag("Ground");
        this.specials = GameObject.Find("Specials");

        RunSpecials();

        foreach (Transform t in ground.transform)
        {
            positions.Add(new Vector3(t.position.x, t.position.y, -1));
        }

        foreach(Vector3 v in positions)
        {
            List<Vector3> temp = new List<Vector3>();
            foreach(Vector3 t in positions)
            {
                if (!v.Equals(t))
                {
                    if(AdjacentPositions(v, t))
                    {
                        temp.Add(t);
                    }
                }
            }
            if(temp.Count == 3)
            {
                foreach(Vector3 h in temp)
                {
                    if (!intersections.Contains(h))
                    {
                        intersections.Add(h);
                    }
                } 
            }
        }
    }

    private void RunSpecials()
    {
        foreach(Transform t in specials.transform)
        {
            switch (t.gameObject.name)
            {
                case "Stove":
                    this.stoves.Add(new Vector3(t.position.x, t.position.y, -1));
                    break;

                case "OnionDisp":
                    this.onionDisp.Add(new Vector3(t.position.x, t.position.y, -1));
                    break;

                case "CuttingBoard":
                    this.cuttingBoards.Add(new Vector3(t.position.x, t.position.y, -1));
                    break;

                case "Delivery":
                    this.delivery = t;
                    break;

                case "Return":
                    this.plateReturn = new Vector3(t.position.x, t.position.y, -1);
                    break;

                default:
                    break;
            }
        }
    }


    public List<List<Vector3>> FindPossiblePaths(Vector3 start, Vector3 goal)
    {
        List<Vector3> walkablePositions = this.FindClosestWalkablePosition(goal);
       
        List<List<Vector3>> allPossiblePaths = new List<List<Vector3>>();

        foreach(Vector3 w in walkablePositions)
        {
            List<List<Vector3>> lp = this.FindPaths(start, w, new List<Vector3>(), 5);

            foreach(List<Vector3> p in lp)
            {
                allPossiblePaths.Add(p);
            }
        }
        return allPossiblePaths;
        
    }



    private List<List<Vector3>> FindPaths(Vector3 start, Vector3 goal, List<Vector3> path, int wait)
    {
        if (start.Equals(goal))
        {
            path.Add(start);
            List<List<Vector3>> paths = new List<List<Vector3>>();
            paths.Add(path);
            return paths;
        }
        List<List<Vector3>> possiblePaths = new List<List<Vector3>>();

        foreach(Vector3 v in positions)
        {
            if(AdjacentPositions(start, v)) //Adjacent square he can walk
            {
                int ocurrences = OcurrencesInPath(path, v);
                if(ocurrences == 0)        // path where it is coming from
                {
                    int newWaitTime = 5;
                    if (intersections.Contains(v)) { newWaitTime = 0; }
                    List<Vector3> pathCopy = new List<Vector3>(path);
                    pathCopy.Add(start);
                    List<List<Vector3>> pathsReturned = FindPaths(v, goal, pathCopy, newWaitTime);
                    foreach(List<Vector3> p in pathsReturned)
                    {
                        possiblePaths.Add(p);
                    }
                }/*
                //take this out if dont want them to wait in intersections
                else if(ocurrences == 1 && intersections.Contains(start))   //means you just left an intersection and only passed in the intersection once
                {
                    List<Vector3> pathCopy = new List<Vector3>(path);
                    pathCopy.Add(start);
                    List<List<Vector3>> pathsReturned = FindPaths(v, goal, pathCopy, 0);
                    foreach (List<Vector3> p in pathsReturned)
                    {
                        possiblePaths.Add(p);
                    }
                    if(wait > 0)
                    {
                        pathCopy = new List<Vector3>(path);
                        pathCopy.Add(start);
                        pathsReturned = FindPaths(start, goal, pathCopy, wait - 1);
                        foreach (List<Vector3> p in pathsReturned)
                        {
                            possiblePaths.Add(p);
                        }
                    }
                }*/  
            }
        }
        return possiblePaths;

    }


    private List<Vector3> FindClosestWalkablePosition(Vector3 goal)
    {
        List<Vector3> walkablePositions = new List<Vector3>();

        if (positions.Contains(goal))
        {
            walkablePositions.Add(goal);
            return walkablePositions;
        }
        else
        {
            foreach(Vector3 p in positions)
            {
                float xDiference = Mathf.Abs(p.x - goal.x);
                float yDiference = Mathf.Abs(p.y - goal.y);

                if((xDiference + yDiference) <= 1)
                {
                    walkablePositions.Add(p);
                }
            }
            return walkablePositions;
        }
    }

    private int OcurrencesInPath(List<Vector3> path, Vector3 v)
    {
        int ocurrences = 0;
        foreach(Vector3 position in path)
        {
            if (position.Equals(v))
            {
                ocurrences++;
            }
        }
        return ocurrences;
    }

    private bool AdjacentPositions(Vector3 pos1, Vector3 pos2)
    {
        float xDiference = Mathf.Abs(pos2.x - pos1.x);
        float yDiference = Mathf.Abs(pos2.y - pos1.y);

        return (xDiference + yDiference) == 1;
    }


    public Vector3 GetPositionOf(GameObject desiredObject)   //FIXME Gameobject ??
    {
        //TODO
        return new Vector3();

    }

    public void AddOtherPlayer(GameObject player)
    {
        otherPlayers.Add(player);
    }




    public List<Action> GetPossibleActions(List<string> options)
    {
        List<Action> possibleActions = new List<Action>();

        for(int i = 0; i < options.Count; i++)
        {
            switch (options[i])
            {
                case "deliverOnionSoup":
                    foreach (Transform soup in this.soups)
                    {
                        if (soup.parent.parent.Equals(this.gameObject.transform) && soup.gameObject.GetComponent<Soup>().type() == Item.type.onion)   //player has it
                        {
                            Action newAction = new Action("deliverOnionSoup", new Vector3(this.delivery.position.x, this.delivery.position.y, -1), new Vector3());
                            possibleActions.Add(newAction);
                        }
                    }
                    break;

                case "getOnionSoup":
                    foreach(Transform soup in this.soups)
                    {
                        if (!InPlayer(soup.parent))     //free
                        {
                            foreach(Transform p in this.plates) //verify that is in a plate or a pan, TODO
                            {

                            }
                            Action newAction = new Action("getOnionSoup", new Vector3(soup.position.x, soup.position.y, -1), new Vector3());
                            possibleActions.Add(newAction);
                        }
                    }
                    break;

                case "cutOnion":
                    //TODO
                    foreach(Transform onion in this.onions)
                    {
                        if(onion.gameObject.GetComponent<Food>().IsCut() && !InPlayer(onion))
                        {
                            Action newAction = new Action("getCutOnion", new Vector3(onion.position.x, onion.position.y, -1), new Vector3());
                            possibleActions.Add(newAction);
                        }
                    }
                    break;

                case "getOnion":
                    foreach(Transform onion in this.onions)
                    {
                        if (!onion.gameObject.GetComponent<Food>().IsCut() && !InPlayer(onion))     //exists but not cut and not in player possession
                        {
                            Action newAction = new Action("getOnion", new Vector3(onion.position.x, onion.position.y, -1), new Vector3());
                            possibleActions.Add(newAction);
                        }
                    }
                    foreach(Vector3 v in this.onionDisp)
                    {
                        Action newAction = new Action("getOnion", new Vector3(v.x, v.y, -1), new Vector3());
                        possibleActions.Add(newAction);
                    }

                    break;

                default:
                    break;

            }
        }
        return possibleActions;
    }

    private bool InPlayer(Transform t)
    {
        foreach (GameObject g in otherPlayers)
        {
            if (g.transform.Equals(t.parent))
            {
                return true;
            }
        }
        return false;
    }

}   
