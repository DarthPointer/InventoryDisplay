using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace InventoryDisplay
{
    [BepInPlugin("DarthPointer.InventoryDisplay", "Inventory Display", "0.0.0")]
    public class InventoryDisplayPlugin : BaseUnityPlugin
    {
        private const float DISPLAY_CELL_WIDTH = 30;
        private const float DISPLAY_CELL_HEIGHT = 30;

        private const float HORIZONTAL_PADDING = 5;
        private const float VERTICAL_PADDING = 5;

        private WeakReference _trackedSlugCat = new WeakReference(null);

        private Player TrackedSlugCat
        {
            get => _trackedSlugCat.Target as Player;

            set
            {
                _trackedSlugCat = new WeakReference(value);
            }
        }

        public void OnEnable()
        {
            On.Player.ctor += Player_ctor;
        }

        private void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            TrackedSlugCat = self;
        }

        public void OnGUI()
        {
            if (TrackedSlugCat != null)
            {
                int currentSlotIndex = 0;

                foreach (var grasp in TrackedSlugCat.grasps)
                {
                    DisplayObject(grasp?.grabbed?.abstractPhysicalObject, currentSlotIndex++, 0);
                }

                DisplayObject(TrackedSlugCat.spearOnBack?.spear?.abstractPhysicalObject, currentSlotIndex++, 0);

                DisplayObject(TrackedSlugCat.objectInStomach, 0, 1);
            }
        }

        private static float GetInsertionRatio(Vector2 sourcePixelSize, float targetWidth = DISPLAY_CELL_WIDTH, float targetHeight = DISPLAY_CELL_HEIGHT)
        {
            float widthRatio = targetWidth / sourcePixelSize.x;
            float heightRatio = targetHeight / sourcePixelSize.y;

            return Math.Min(1f,
                Math.Min(widthRatio, heightRatio));
        }

        private static Vector2 GetInsertionSizeAspectRatioPreserving(Vector2 sourcePixelSize, float targetWidth = DISPLAY_CELL_WIDTH, float targetHeight = DISPLAY_CELL_HEIGHT)
        {
            return sourcePixelSize * GetInsertionRatio(sourcePixelSize, targetWidth, targetHeight);
        }

        private void DisplayObject(AbstractPhysicalObject obj, int displaySlotX, int displaySlotY)
        {
            if (obj != null)
            {
                try
                {
                    FAtlasElement currentIcoAtlasElement = obj.GetObjectDisplayAtlasElement();

                    Vector2 insertedPixelSize = GetInsertionSizeAspectRatioPreserving(currentIcoAtlasElement.sourcePixelSize);
                    float xCenteringOffset = (DISPLAY_CELL_WIDTH - insertedPixelSize.x) / 2;
                    float yCenteringOffset = (DISPLAY_CELL_HEIGHT - insertedPixelSize.y) / 2;

                    GUI.DrawTextureWithTexCoords(
                        new Rect(
                            displaySlotX * (DISPLAY_CELL_WIDTH + HORIZONTAL_PADDING) + xCenteringOffset + HORIZONTAL_PADDING,
                            displaySlotY * (DISPLAY_CELL_HEIGHT + VERTICAL_PADDING) + yCenteringOffset + VERTICAL_PADDING,
                            insertedPixelSize.x,
                            insertedPixelSize.y),

                        currentIcoAtlasElement.atlas.texture,
                        new Rect(currentIcoAtlasElement.uvRect));
                }
                catch (Exception e)
                {
                    Logger.LogError($"Could not obtain ico for grabbed object {obj}");
                    Logger.LogError(e.ToString());
                }
            }
        }
    }
}
