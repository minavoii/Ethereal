using System.IO;

namespace Equipments.Weapons;

internal static class BlueBlade
{
    internal const string Name = "Blue Blade";

    internal static readonly Ethereal.API.Equipment.EquipmentDescriptor descriptor = new()
    {
        name = Name,
        icon = Ethereal.API.Icon.LoadFromImage(
            Ethereal.API.Icon.IconType.Equipment,
            Path.Join(Plugin.EquipmentsPath, "Equipment_Blade_Blue.png"),
            "BladeBlue"
        ),
        price = 100,
    };
}
