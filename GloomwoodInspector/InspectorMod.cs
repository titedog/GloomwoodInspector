using System;
using GloomwoodInspector;
using MelonLoader;
using UnityEngine;
using Gloomwood;
using Gloomwood.Players;
using UnityEngine.Windows;
using Gloomwood.RuntimeConsole.Commands;
using Gloomwood.VFX;
using HarmonyLib;
using Gloomwood.RuntimeConsole;
using Gloomwood.AI;
using Gloomwood.Entity.Items;
using Gloomwood.Entity.Weapons;
using Gloomwood.Entity;
using Gloomwood.Saving;
using GloomwoodInspector.Commands;
using static HarmonyLib.AccessTools;
using System.Collections.Generic;

[assembly: MelonInfo(typeof(InspectorMod), "GloomwoodInspector", "0.1", "polyskull")]
namespace GloomwoodInspector
{
    public class InspectorMod : MelonMod
    {
        private static bool quitting = false;
        private static KeyCode tpToCursor = KeyCode.Keypad7;
        private static LayerMask cursorLayer = LayerHelper.CreateMask(new LayerTypes[]
        {
            LayerTypes.AI,
            LayerTypes.PhysicsLarge,
            LayerTypes.PhysicsMedium,
            LayerTypes.PhysicsSmall,
            LayerTypes.World,
            LayerTypes.Platform,
            LayerTypes.Placement,
            LayerTypes.Door,
            LayerTypes.Room,
            LayerTypes.Clipping,
            LayerTypes.Interactive,
            LayerTypes.Hitbox
        });

        public override void OnInitializeMelon()
        {
            MelonEvents.OnGUI.Subscribe(DrawMenu, 1);
        }

        public override void OnApplicationQuit()
        {
            // I hate this but its to prevent the game from not being able to close.
            quitting = true;
        }

        public override void OnLateUpdate()
        {
            if(!quitting)
            {
                if (UnityEngine.Input.GetKeyDown(tpToCursor) && GameManager.Player != null)
                {
                    //GameManager.Player.position = GameManager.Player.TargetPoint;

                    UnityEngine.Vector3 cameraPosition = GameManager.Player.Camera.CameraPosition;
                    UnityEngine.Vector3 worldPoint = GameManager.Player.Interact.GetWorldPoint(1000f);
                    if(Physics.Linecast(cameraPosition, worldPoint, out ReusableArrayHolder.RaycastHits[0], cursorLayer))
                    {
                        GameManager.Player.position = ReusableArrayHolder.RaycastHits[0].point;
                    }
                }
            }
        }

        private void DrawMenu()
        {
            if(!quitting)
            {
                GUI.Label(new Rect(0, 0, 500, 50), "GloomwoodInspector " + this.Info.Version);
                PlayerEntity? player = GameManager.Player;
                if (player != null)
                {
                    // Every property to draw
                    string[] stats =
                    {
                        "x: " + player.Position.x,
                        "y: " + player.Position.y,
                        "z: " + player.Position.z,
                        "Target x: " + player.TargetPoint.x,
                        "Target y: " + player.TargetPoint.y,
                        "Target z: " + player.TargetPoint.z
                    };

                    int y = 0;
                    foreach (string s in stats)
                    {
                        y += 15;
                        GUI.Label(new Rect(0, y, 500, 50), s);
                    }
                }
                else
                {
                    // null case
                    GUI.Label(new Rect(0, 15, 500, 50), "Cannot display further information; player is null.");
                }
            }
        }
    }

    // Patch to insert custom commands at runtime.
    [HarmonyPatch(typeof(ConsoleController), "Awake")]
    public class Patch
    {
        static FieldRef<ConsoleController, Dictionary<string, ConsoleCommand>> commandsRef =
        AccessTools.FieldRefAccess<ConsoleController, Dictionary<string, ConsoleCommand>>("commands");

        public static void Postfix(ConsoleController __instance)
        {
            Dictionary<string, ConsoleCommand> cmds = commandsRef(__instance);
            cmds[CoordTp.name] = CoordTp.GetCommand();
        }
    }
}
