﻿using UnityEngine;

public abstract class Fact
{
    public int Id;
    public GameObject Representation;

}
//I am not sure if we ever need to attach these to an object, so one script for all for now...
public class PointFact : Fact
{
    public Vector3 Point;
    public Vector3 Normal;
}
public class LineFact : Fact
{
    //Id's of the 2 Point-Facts that are connected
    public int Pid1, Pid2;
    
}
public class AngleFact : Fact
{
    //Id's of the 3 Point-Facts, where Pid2 is the point, where the angle is
    public int Pid1, Pid2, Pid3;
}
public class OnLineFact : Fact
{
    //Id's of the 3 Point-Facs that are on one line
    public int Pid1, Pid2, Pid3;
}

