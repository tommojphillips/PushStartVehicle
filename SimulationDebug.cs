using System.Linq;

using MSCLoader;

using TommoJProductions.ModApi;
using TommoJProductions.ModApi.Database;

using UnityEngine;
using UnityEngine.UI;

using static TommoJProductions.ModApi.ModClient;

namespace TommoJProductions.PushStartVehicle
{
    public class SimulationDebug : MonoBehaviour
    {
        internal VehiclePushStartLogic logic;

#if DEBUG
        private bool guiDebug = true;
#else 
        private bool guiDebug = false;
#endif
        private readonly int fontSize = Screen.width / 135;
        private readonly int editsWidth = 240;
        private readonly int valuesWidth = 180;
        private readonly int height = Screen.height;
        private readonly int top = 200;
        private readonly int left = 50;
        private readonly int playerStatsOffSet = 240;
        private GUIStyle guiStyle;

        private bool drawPushStartDebugEdits = true;
        private int myPhysicMaterialSelectedIndex;

        private void Update() 
        {
            // Written, 13.11.2022

            guiToggle();
        }
        private void OnGUI()
        {
            if (guiDebug)
            {
                guiStyle = new GUIStyle
                {
                    alignment = TextAnchor.MiddleLeft,
                    padding = new RectOffset(0, 0, 0, 0)
                };
                guiStyle.normal.textColor = Color.white;
                guiStyle.hover.textColor = Color.blue;
                guiStyle.fontSize = fontSize;

                drawDebugValues();
                drawDebugValues2();
                drawDebugEdits();
                drawDebugEdits2();
            }
        }

        private void guiToggle()
        {
            // Written, 15.01.2022

            if (cInput.GetButtonDown("DrivingMode") && cInput.GetButton("Finger"))
            {
                guiDebug = !guiDebug;
            }
        }
        private void drawDebugValues()
        {
            using (new GUILayout.AreaScope(new Rect(left, top - 150, valuesWidth, height - top + 150), "", guiStyle))
            {
                drawProperty("VALUES");
                                
            }
        }
        private void drawDebugValues2()
        {
            using (new GUILayout.AreaScope(new Rect((left * 2) + valuesWidth, playerStatsOffSet, valuesWidth, height - top + 150), "", guiStyle))
            {
                drawProperty("VALUES 2");
            }
        }
        private void drawDebugEdits()
        {
            using (new GUILayout.AreaScope(new Rect((left * 3) + (valuesWidth * 2), playerStatsOffSet, editsWidth, height - playerStatsOffSet), "", guiStyle))
            {                
                drawPropertyBool("Draw Push Start Debug Edits", ref drawPushStartDebugEdits);                                
            }
        }
        private void drawDebugEdits2()
        {
            using (new GUILayout.AreaScope(new Rect((left * 4) + (valuesWidth * 3), top - 150, editsWidth, height - playerStatsOffSet), "", guiStyle))
            {
                if (drawPushStartDebugEdits)
                {
                    drawPropertyBool("Will engine start", ref logic.engineWillStart);
                    drawPropertyBool("Push start logic enabled", ref logic.pushStartLogicEnabled);
                    drawProperty("Start Torque", logic.drivetrain.startTorque);
                    drawProperty("Torque", logic.drivetrain.torque);
                    drawProperty("Clutch pos", logic.drivetrain.clutch.GetClutchPosition());
                    drawProperty("Clutch Drag Impluse", logic.clutchDragImplulse);
                    drawProperty("Engine Friction Factor", logic.drivetrain.engineFrictionFactor);
                }

                drawProperty("Physic Materials");
                MyPhysicMaterial[] mpm = logic.carDynamics.physicMaterials;

                using (new GUILayout.HorizontalScope())
                {
                    for (int i = 0; i < mpm.Length; i++)
                    {
                        if (GUILayout.Button(mpm[i].surfaceType.ToString()))
                        {
                            myPhysicMaterialSelectedIndex = i;
                        }

                    }
                }

                if (myPhysicMaterialSelectedIndex > -1)
                {
                    drawProperty($"{myPhysicMaterialSelectedIndex + 1} ({mpm[myPhysicMaterialSelectedIndex].surfaceType})");
                    drawPropertyEdit("Rolling Friction", ref mpm[myPhysicMaterialSelectedIndex].rollingFriction);
                    drawPropertyEdit("Static Friction", ref mpm[myPhysicMaterialSelectedIndex].staticFriction);
                    drawPropertyEdit("Grip", ref mpm[myPhysicMaterialSelectedIndex].grip);
                    mpm[myPhysicMaterialSelectedIndex].physicMaterial.bounceCombine = drawPropertyEnum(mpm[myPhysicMaterialSelectedIndex].physicMaterial.bounceCombine, "Bounce combine");
                    mpm[myPhysicMaterialSelectedIndex].physicMaterial.frictionCombine = drawPropertyEnum(mpm[myPhysicMaterialSelectedIndex].physicMaterial.frictionCombine, "Friction combine");
                    
                    if (GUILayout.Button("Apply Physic Materials"))
                    {
                        logic.carDynamics.physicMaterials = mpm;
                    }
                }
            }
        }
    }
}