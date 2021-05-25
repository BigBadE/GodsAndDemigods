using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using OldWorldGods.Source.Defs;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace OldWorldGods.Base
{
    public class Gods : WorldComponent
    {
        private List<God> gods;
        //Float between 0 and 100
        private Dictionary<WorldObject, float> detection;
        
        public List<God> AllGods => gods;

        public Gods(World world) : base(world)
        {
            Harmony harmony = new Harmony("bigbade.OldWorldGods.Core");

            if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Linux)
            {
                FixLinuxHarmonyCrash(harmony);
            }

            harmony.PatchAll();
            
            gods = DefDatabase<GodDef>.AllDefsListForReading.Select(def => new God(def)).ToList();
            detection = new Dictionary<WorldObject, float>();
        }

        public float GetDetection(WorldObject worldObject)
        {
            return detection.TryGetValue(worldObject, out var detectionLevel) ? detectionLevel : 0;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref gods, "gods", LookMode.Deep);
            Scribe_Collections.Look(ref detection, "detection", LookMode.Reference, LookMode.Value);
        }
        
        
        // Fix a crash related to a harmony bug on Linux
        // This gets all patches Empire makes, gets the ones that would crash on Linux, and fixes them
        static void FixLinuxHarmonyCrash(Harmony harmony)
        {
            bool WouldCrash(MethodInfo method)
            {
                if (method == null || !method.IsVirtual || method.IsAbstract || method.IsFinal)
                {
                    return false;
                }

                byte[] bytes = method.GetMethodBody()?.GetILAsByteArray();
                return bytes == null || bytes.Length == 0 || (bytes.Length == 1 && bytes.First() == 0x2A);
            }

            IEnumerable<MethodInfo> methods = typeof(Gods).Assembly.GetTypes()
                    .Where(t => t.IsClass && !typeof(Delegate).IsAssignableFrom(t))
                    .Where(t => t.GetCustomAttributes(typeof(HarmonyPatch)).Any())
                    .SelectMany(t =>
                    {
                        HarmonyPatch patch = (HarmonyPatch) Attribute.GetCustomAttribute(t, typeof(HarmonyPatch));
                        MethodInfo[] m = patch.info.declaringType.GetMethods(BindingFlags.Public |
                                                                             BindingFlags.NonPublic |
                                                                             BindingFlags.Instance |
                                                                             BindingFlags.DeclaredOnly);
                        return m.Where(met => met.Name == patch.info.methodName);
                    })
                    .Where(WouldCrash);

            foreach (MethodInfo i in methods)
            {
                // Patching methods without any Prefixes/Postfixes before actually patching them fixes it. Idk why
                harmony.Patch(i);
            }
        }
    }
}
