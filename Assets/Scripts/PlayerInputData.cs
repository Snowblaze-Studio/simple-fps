using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct Move
{
    readonly int x;
    readonly int y;
}

public struct Look
{
    readonly int x;
    readonly int y;
}

class PlayerInputData
{
    public Move move;
    public Look look;
    public bool fire;
    public bool jump;
}
