using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct Move
{
    public readonly float x;
    public readonly float y;

    public Move(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
}

public struct Look
{
    public readonly float x;
    public readonly float y;

    public Look(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
}

class PlayerInputData
{
    public Move move;
    public Look look;
    public bool fire;
    public bool jump;
}
