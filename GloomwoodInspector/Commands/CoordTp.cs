using Gloomwood.RuntimeConsole;
using Gloomwood;
using System;
using UnityEngine;

namespace GloomwoodInspector.Commands
{
    public class CoordTp
    {
        public static ConsoleCommand GetCommand()
        {
            return new ConsoleCommand(CoordTp.name, CoordTp.description, CoordTp.usage, new ConsoleCommandCallback(CoordTp.Execute));
        }

        public static string Execute(params string[] args)
        {
            if (GameManager.Player == null)
            {
                return "Unable to execute: player is null.";
            }

            if (args.Length != 3)
            {
                return "Failed to teleport to coordinates.";
            }

            if (!float.TryParse(args[0], out float x) || !float.TryParse(args[1], out float y) || !float.TryParse(args[2], out float z))
            {
                return "Unable to parse coordinates. (Are they valid floats?)";
            }

            GameManager.Player.position = new Vector3(x, y, z);
            return String.Format("Teleported to coordinates x: {0}, y: {1}, z: {2}", x, y, z);
        }

        public static readonly string name = "COORDTP";
        public static readonly string description = "Teleports you to the given XYZ coordinates.";
        public static readonly string usage = "COORDTP [float] [float] [float]";
    }
}
