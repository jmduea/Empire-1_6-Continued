using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using Verse;

namespace FactionColonies.util
{
    /// <summary>
    /// Compatibility layer for Humanoid Alien Races (HAR) mod.
    /// Provides detection and integration for alien races in Empire faction colonies.
    /// </summary>
    public static class HARCompat
    {
        private const string HAR_PACKAGE_ID = "erdelf.HumanoidAlienRaces";
        private const string HAR_ALIEN_RACE_CLASS = "AlienRace.ThingDef_AlienRace";
        
        private static bool? _isHARLoaded;
        private static Type _alienRaceType;
        
        /// <summary>
        /// Checks if the Humanoid Alien Races mod is currently loaded.
        /// </summary>
        public static bool IsHARLoaded
        {
            get
            {
                if (_isHARLoaded == null)
                {
                    _isHARLoaded = FactionColonies.IsModLoaded(HAR_PACKAGE_ID);
                    if (_isHARLoaded.Value)
                    {
                        Log.Message("[Empire] Humanoid Alien Races detected - enabling alien race support");
                    }
                }
                return _isHARLoaded.Value;
            }
        }
        
        /// <summary>
        /// Gets the AlienRace.ThingDef_AlienRace type if HAR is loaded.
        /// </summary>
        private static Type AlienRaceType
        {
            get
            {
                if (_alienRaceType == null && IsHARLoaded)
                {
                    _alienRaceType = FactionColonies.returnUnknownTypeFromName(HAR_ALIEN_RACE_CLASS);
                    if (_alienRaceType == null)
                    {
                        Log.Warning("[Empire] HAR is loaded but AlienRace.ThingDef_AlienRace type not found");
                    }
                }
                return _alienRaceType;
            }
        }
        
        /// <summary>
        /// Checks if a ThingDef is an alien race from HAR.
        /// </summary>
        public static bool IsAlienRace(ThingDef thingDef)
        {
            if (!IsHARLoaded || AlienRaceType == null || thingDef == null)
            {
                return false;
            }
            
            return AlienRaceType.IsAssignableFrom(thingDef.GetType());
        }
        
        /// <summary>
        /// Checks if a PawnKindDef is a valid humanlike race (vanilla or alien) with proper configuration.
        /// </summary>
        public static bool IsValidHumanlikeRace(PawnKindDef pawnKindDef)
        {
            if (pawnKindDef?.race == null)
            {
                return false;
            }
            
            // Basic humanlike check
            if (pawnKindDef.race.race?.intelligence != Intelligence.Humanlike)
            {
                return false;
            }
            
            // Check market value (should have some value)
            if (pawnKindDef.race.BaseMarketValue == 0)
            {
                return false;
            }
            
            return true;
        }
    }
}
