using System;
using FactionColonies.util;
using RimWorld;
using UnityEngine;
using Verse;

namespace FactionColonies
{
    /// <summary>
    /// Window for managing special job worker assignments (Enforcer, Entertainer, Builder)
    /// </summary>
    public class FCSpecialJobsWindow : Window
    {
        public override Vector2 InitialSize => new Vector2(450f, 420f);

        private readonly SettlementFC settlement;

        public FCSpecialJobsWindow(SettlementFC settlement)
        {
            this.settlement = settlement;
            forcePause = false;
            draggable = true;
            doCloseX = true;
            preventCameraMotion = false;
        }

        public override void DoWindowContents(Rect inRect)
        {
            GameFont fontBefore = Text.Font;
            TextAnchor anchorBefore = Text.Anchor;

            // Header
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(new Rect(0, 0, inRect.width, 35), "SpecialJobs".Translate());

            // Worker availability info
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            
            int totalResourceWorkers = settlement.getTotalWorkers();
            int totalSpecialWorkers = settlement.getTotalSpecialJobWorkers();
            int totalAssigned = totalResourceWorkers + totalSpecialWorkers;
            int maxAvailable = (int)settlement.workersUltraMax;
            int idleWorkers = settlement.getIdleWorkers();
            
            Widgets.Label(new Rect(10, 40, inRect.width - 20, 25), 
                "AvailableWorkers".Translate() + ": " + (maxAvailable - totalAssigned) + "/" + maxAvailable);
            Widgets.Label(new Rect(10, 65, inRect.width - 20, 25),
                "IdleWorkers".Translate() + ": " + idleWorkers + " (" + "ConstructionBonus".Translate() + ": -" + 
                Math.Round((1 - settlement.getIdleWorkerConstructionMultiplier()) * 100) + "%)");

            // Construction time bonus display
            Text.Font = GameFont.Tiny;
            double totalConstructionBonus = (1 - settlement.getTotalConstructionTimeMultiplier()) * 100;
            Widgets.Label(new Rect(10, 90, inRect.width - 20, 20),
                "TotalConstructionSpeedBonus".Translate() + ": -" + Math.Round(totalConstructionBonus) + "% " + "ConstructionTime".Translate());

            Text.Font = GameFont.Small;
            
            int yOffset = 120;
            int rowHeight = 85;
            int buttonWidth = 30;
            int buttonHeight = 24;
            
            // Enforcer Job
            DrawJobRow(inRect, yOffset, "Enforcer", settlement.enforcerWorkers, 
                "EnforcerDesc".Translate(), "enforcer", buttonWidth, buttonHeight);
            
            // Entertainer Job
            yOffset += rowHeight;
            DrawJobRow(inRect, yOffset, "Entertainer", settlement.entertainerWorkers,
                "EntertainerDesc".Translate(15 * settlement.settlementLevel), "entertainer", buttonWidth, buttonHeight);
            
            // Builder Job
            yOffset += rowHeight;
            DrawJobRow(inRect, yOffset, "Builder", settlement.builderWorkers,
                "BuilderDesc".Translate(50 * settlement.settlementLevel, Math.Round((1 - settlement.getBuilderConstructionMultiplier()) * 100), 
                    Math.Round((1 - settlement.getBuilderUpkeepMultiplier()) * 100)), 
                "builder", buttonWidth, buttonHeight);

            // Special job upkeep display
            yOffset += rowHeight;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.DrawHighlight(new Rect(10, yOffset, inRect.width - 20, 30));
            Widgets.Label(new Rect(10, yOffset, inRect.width - 20, 30),
                "SpecialJobUpkeep".Translate() + ": " + Math.Round(settlement.getSpecialJobUpkeep()) + " " + "Silver".Translate());

            Text.Font = fontBefore;
            Text.Anchor = anchorBefore;
        }

        private void DrawJobRow(Rect inRect, int y, string jobName, int currentWorkers, 
            string description, string jobType, int buttonWidth, int buttonHeight)
        {
            Rect jobRect = new Rect(10, y, inRect.width - 20, 80);
            Widgets.DrawMenuSection(jobRect);

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            
            // Job name
            Widgets.Label(new Rect(15, y + 5, 200, 25), jobName.Translate());
            
            // Worker count and buttons
            Text.Anchor = TextAnchor.MiddleCenter;
            Rect workerRect = new Rect(inRect.width - 150, y + 5, 130, 25);
            
            // Decrease button
            if (Widgets.ButtonText(new Rect(workerRect.x, workerRect.y, buttonWidth, buttonHeight), "<"))
            {
                int change = -1 * Modifiers.GetModifier;
                settlement.changeSpecialJobWorkers(jobType, change);
            }
            
            // Worker count display
            Widgets.Label(new Rect(workerRect.x + buttonWidth + 5, workerRect.y, 50, buttonHeight), 
                currentWorkers.ToString());
            
            // Increase button
            if (Widgets.ButtonText(new Rect(workerRect.x + buttonWidth + 60, workerRect.y, buttonWidth, buttonHeight), ">"))
            {
                int change = 1 * Modifiers.GetModifier;
                settlement.changeSpecialJobWorkers(jobType, change);
            }
            
            // Description
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.UpperLeft;
            Widgets.Label(new Rect(15, y + 32, inRect.width - 40, 45), description);
        }
    }
}
