using System.IO;
using Ethereal.API;

namespace ExampleEquipments.Weapon;

internal static class BlueBlade
{
    internal const string Name = "Blue Blade";

    internal static readonly Equipments.EquipmentDescriptor descriptor = new()
    {
        Name = Name,
        Price = 100,
        Icon = Sprites.LoadFromImage(
            Sprites.SpriteType.Equipment,
            Path.Join(Plugin.EquipmentsPath, "Equipment_Blade_Blue.png")
        ),
    };
}
