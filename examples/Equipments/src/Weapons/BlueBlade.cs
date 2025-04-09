using System.IO;

namespace Equipments.Weapons;

internal static class BlueBlade
{
    internal const string Name = "Blue Blade";

    internal static readonly Ethereal.API.Equipments.EquipmentDescriptor descriptor = new()
    {
        name = Name,
        icon = Ethereal.API.Sprites.LoadFromImage(
            Ethereal.API.Sprites.SpriteType.Equipment,
            Path.Join(Plugin.EquipmentsPath, "Equipment_Blade_Blue.png")
        ),
        price = 100,
    };
}
