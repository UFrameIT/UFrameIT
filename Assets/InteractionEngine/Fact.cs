using UnityEngine;

public class Fact
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
    public int Pid1, Pid2;
}
public class AngleFact : Fact
{
    public int Pid1, Pid2, Pid3;
}
public class OnLineFact : Fact
{
    public int Pid1, Pid2, Pid3;
}

