using System.IO;
using Ethereal.API;
using Ethereal.Classes.View;

namespace ExampleEquipments.Weapon;

internal static class BlueBlade
{
    internal const string Name = "Blue Blade";

    internal static EquipmentView View(Equipment equipment) =>
        new(equipment)
        {
            Name = Name,
            Price = 100,
            Icon = Sprites.LoadFromImage(
                Sprites.SpriteType.Equipment,
                Path.Join(Plugin.EquipmentsPath, "Equipment_Blade_Blue.png")
            ),
        };
}
