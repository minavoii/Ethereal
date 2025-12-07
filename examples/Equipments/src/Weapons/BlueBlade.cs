using System.IO;
using Ethereal.API;

namespace ExampleEquipments.Weapon;

internal static class BlueBlade
{
    private const string Name = "Blue Blade";

    internal static readonly Equipments.EquipmentDescriptor Descriptor = new()
    {
        Name = Name,
        Price = 100,
        Icon = Sprites.LoadFromImage(
            Sprites.SpriteType.Equipment,
            Path.Join(Plugin.EquipmentsPath, "Equipment_Blade_Blue.png")
        ),
    };
}
