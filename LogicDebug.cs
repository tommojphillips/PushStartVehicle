using System.Linq;
using System.Linq.Expressions;

using MSCLoader;

using TommoJProductions.ModApi;

using UnityEngine;
using UnityEngine.UI;

namespace TommoJProductions.PushStartVehicle
{
    public class LogicDebug : MonoBehaviour
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
        private readonly int top = 20;
        private readonly int left = 510;
        private readonly int playerStatsOffSet = 240;
        private GUIStyle guiStyle;

        private bool drawPushStartDebugEdits = true;
        private int myPhysicMaterialSelectedIndex;
        private MyPhysicMaterial[] stockPhysics;

        private void Start() 
        {
            stockPhysics = new MyPhysicMaterial[logic.carDynamics.physicMaterials.Length];
            MyPhysicMaterial pm;
            for (int i = 0; i < stockPhysics.Length; i++)
            {
                pm = logic.carDynamics.physicMaterials[i];
                stockPhysics[i] = new MyPhysicMaterial()
                {
                    grip = pm.grip,
                    isDirty = pm.isDirty,
                    isSkidMark = pm.isSkidMark,
                    isSkidSmoke = pm.isSkidSmoke,
                    rollingFriction = pm.rollingFriction,
                    staticFriction = pm.staticFriction,
                    surfaceType = pm.surfaceType
                };

                PhysicMaterial _pm = pm.physicMaterial;
                stockPhysics[i].physicMaterial = new PhysicMaterial("stock_" + _pm.name)
                {
                    bounceCombine = _pm.bounceCombine,
                    bounciness = _pm.bounciness,
                    dynamicFriction = _pm.dynamicFriction,
                    dynamicFriction2 = _pm.dynamicFriction2,
                    frictionCombine = _pm.frictionCombine,
                    frictionDirection2 = _pm.frictionDirection2,
                    staticFriction = _pm.staticFriction, 
                    staticFriction2 = _pm.staticFriction2, 
                    hideFlags = _pm.hideFlags
                };
            }

        }
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

            if (Input.GetKeyDown(KeyCode.Alpha0) && cInput.GetButton("Finger"))
            {
                guiDebug = !guiDebug;
            }
        }
        private void drawDebugValues()
        {
            using (new GUILayout.AreaScope(new Rect(left, top, valuesWidth, height - top), "", guiStyle))
            {
                ModClient.drawProperty("VALUES");

                // DRIVETRAIN
                float wheelAngularVelocity = Mathf.Max(logic.drivetrain.poweredWheels[0].angularVelocity, logic.drivetrain.poweredWheels[1].angularVelocity);
                float wheelRpm = wheelAngularVelocity * logic.drivetrain.angularVelo2RPM;
                float ratio = logic.drivetrain.ratio;
                float clutchRpm = wheelRpm * ratio;

                ModClient.drawProperty("------------DRIVETRAIN-----------");
                ModClient.drawProperty("wheel angular velocity", wheelAngularVelocity.round(2));
                ModClient.drawProperty("wheel rpm", wheelRpm.round(2));
                ModClient.drawProperty("clutch rpm", clutchRpm.round(2));
                ModClient.drawProperty("clutch drag impulse", logic.drivetrain.clutchDragImpulse.round(2));
                ModClient.drawProperty("clutch drag impulse", logic.getClutchDragImpulse());
                ModClient.drawProperty("Start Torque", logic.drivetrain.startTorque);
                ModClient.drawProperty("Torque", logic.drivetrain.torque);
                ModClient.drawProperty("Clutch pos", logic.drivetrain.clutch.GetClutchPosition());
                ModClient.drawProperty("Clutch Drag Impluse", logic.clutchDragImplulse);
                ModClient.drawProperty("Engine Friction Factor", logic.drivetrain.engineFrictionFactor);
                ModClient.drawProperty("current physic material", logic.drivetrain.poweredWheels[0].physicMaterial.name);
            }
        }
        private void drawDebugValues2()
        {
            using (new GUILayout.AreaScope(new Rect(left + 50 + valuesWidth, top, valuesWidth, height - top), "", guiStyle))
            {
                ModClient.drawProperty("VALUES 2");
            }
        }
        private void drawDebugEdits()
        {
            using (new GUILayout.AreaScope(new Rect(left + (50 + valuesWidth) * 2, top, editsWidth, height - top), "", guiStyle))
            {                
                ModClient.drawPropertyBool("Draw Push Start Debug Edits", ref drawPushStartDebugEdits);                                
            }
        }
        private void drawDebugEdits2()
        {
            using (new GUILayout.AreaScope(new Rect(left + (50 + valuesWidth) * 2 + editsWidth, top, editsWidth + 150, height - top), "", guiStyle))
            {
                if (drawPushStartDebugEdits)
                {
                    ModClient.drawPropertyBool("Will engine start", ref logic.engineWillStart);
                    ModClient.drawPropertyBool("Push start logic enabled", ref logic.pushStartLogicEnabled);
                    ModClient.drawPropertyBool("Starter help enabled", ref logic.starterHelpActive);
                    ModClient.drawPropertyEdit("engine friction factor", ref logic.engineFrictionFactor);
                }

                ModClient.drawProperty("Physic Materials");
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
                    using (new GUILayout.HorizontalScope())
                    {
                        using (new GUILayout.VerticalScope())
                        {
                            ModClient.drawProperty("My Physics Material");
                            ModClient.drawProperty($"{myPhysicMaterialSelectedIndex + 1} {mpm[myPhysicMaterialSelectedIndex].surfaceType}");
                            ModClient.drawPropertyEdit("Rolling Friction", ref mpm[myPhysicMaterialSelectedIndex].rollingFriction);
                            ModClient.drawPropertyEdit("Static Friction", ref mpm[myPhysicMaterialSelectedIndex].staticFriction);
                            ModClient.drawPropertyEdit("Grip", ref mpm[myPhysicMaterialSelectedIndex].grip);
                            ModClient.drawProperty("Physics Material");

                            mpm[myPhysicMaterialSelectedIndex].physicMaterial.bounceCombine = ModClient.drawPropertyEnum(mpm[myPhysicMaterialSelectedIndex].physicMaterial.bounceCombine, "Bounce combine");
                            mpm[myPhysicMaterialSelectedIndex].physicMaterial.frictionCombine = ModClient.drawPropertyEnum(mpm[myPhysicMaterialSelectedIndex].physicMaterial.frictionCombine, "Friction combine");
                            mpm[myPhysicMaterialSelectedIndex].physicMaterial.staticFriction = ModClient.drawPropertyEdit("Static friction", mpm[myPhysicMaterialSelectedIndex].physicMaterial.staticFriction);
                            mpm[myPhysicMaterialSelectedIndex].physicMaterial.staticFriction2 = ModClient.drawPropertyEdit("Static friction 2", mpm[myPhysicMaterialSelectedIndex].physicMaterial.staticFriction2);
                            mpm[myPhysicMaterialSelectedIndex].physicMaterial.dynamicFriction = ModClient.drawPropertyEdit("Dynamic friction", mpm[myPhysicMaterialSelectedIndex].physicMaterial.dynamicFriction);
                            mpm[myPhysicMaterialSelectedIndex].physicMaterial.dynamicFriction2 = ModClient.drawPropertyEdit("Dynamic friction 2", mpm[myPhysicMaterialSelectedIndex].physicMaterial.dynamicFriction2);
                            mpm[myPhysicMaterialSelectedIndex].physicMaterial.bounciness = ModClient.drawPropertyEdit("Bounciness", mpm[myPhysicMaterialSelectedIndex].physicMaterial.bounciness);
                        }
                        using (new GUILayout.VerticalScope())
                        {
                            ModClient.drawProperty($"defaults");
                            ModClient.drawProperty("-");
                            ModClient.drawProperty($"({stockPhysics[myPhysicMaterialSelectedIndex].rollingFriction})");
                            ModClient.drawProperty($"({stockPhysics[myPhysicMaterialSelectedIndex].staticFriction})");
                            ModClient.drawProperty($"({stockPhysics[myPhysicMaterialSelectedIndex].grip})");
                            ModClient.drawProperty("-");
                            ModClient.drawProperty("-");
                            ModClient.drawProperty($"({stockPhysics[myPhysicMaterialSelectedIndex].physicMaterial.bounceCombine})");
                            ModClient.drawProperty("-");
                            ModClient.drawProperty($"({stockPhysics[myPhysicMaterialSelectedIndex].physicMaterial.frictionCombine})");
                            ModClient.drawProperty($"({stockPhysics[myPhysicMaterialSelectedIndex].physicMaterial.staticFriction})");
                            ModClient.drawProperty($"({stockPhysics[myPhysicMaterialSelectedIndex].physicMaterial.staticFriction2})");
                            ModClient.drawProperty($"({stockPhysics[myPhysicMaterialSelectedIndex].physicMaterial.dynamicFriction})");
                            ModClient.drawProperty($"({stockPhysics[myPhysicMaterialSelectedIndex].physicMaterial.dynamicFriction2})");
                            ModClient.drawProperty($"({stockPhysics[myPhysicMaterialSelectedIndex].physicMaterial.bounciness})");
                        }
                    }
                }
            }
        }
    }
}