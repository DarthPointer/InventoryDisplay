using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace InventoryDisplay
{
    static class GraspedObjectExtension
    {
        public static FAtlasElement GetObjectDisplayAtlasElement(this AbstractPhysicalObject obj)
        {
            return Futile.atlasManager.GetElementWithName(obj.GetSpriteName());
        }

        public static string GetSpriteName(this AbstractPhysicalObject obj)
        {
            return IconSymbol.CreateIconSymbol(obj.GetObjectIconData(), null).spriteName;
        }

        private static IconSymbol.IconSymbolData GetObjectIconData(this AbstractPhysicalObject obj)
        {
            if (obj.type != AbstractPhysicalObject.AbstractObjectType.Creature)
            {
                return new IconSymbol.IconSymbolData(CreatureTemplate.Type.BigEel, obj.type, 0);
            }

            else return GetCreatureIconData(obj as AbstractCreature);
        }

        private static IconSymbol.IconSymbolData GetCreatureIconData(this AbstractCreature abstractCreature)
        {
            if (abstractCreature.creatureTemplate.type == CreatureTemplate.Type.Centipede)
            {
                float num = Centipede.GenerateSize(abstractCreature);
                if (num < 0.255f)
                {
                    return new IconSymbol.IconSymbolData(abstractCreature.creatureTemplate.type, AbstractPhysicalObject.AbstractObjectType.Creature, 1);
                }
                if (num < 0.6f)
                {
                    return new IconSymbol.IconSymbolData(abstractCreature.creatureTemplate.type, AbstractPhysicalObject.AbstractObjectType.Creature, 2);
                }
                return new IconSymbol.IconSymbolData(abstractCreature.creatureTemplate.type, AbstractPhysicalObject.AbstractObjectType.Creature, 3);
            }

            return new IconSymbol.IconSymbolData(abstractCreature.creatureTemplate.type, abstractCreature.type, 0);
        }
    }
}
