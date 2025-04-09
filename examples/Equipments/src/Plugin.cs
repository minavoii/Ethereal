using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using Ethereal.API;

namespace ExampleEquipments;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Aethermancer.exe")]
[BepInDependency("minavoii.ethereal")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    internal static readonly string ExamplesPath = Path.Join(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
        "Ethereal.Examples"
    );

    internal static readonly string EquipmentsPath = Path.Join(ExamplesPath, "Equipments");

    private void Awake()
    {
        Logger = base.Logger;

        Equipments.Add(
            Accessory.RedArmor.descriptor,
            Accessory.RedArmor.localisationData,
            Accessory.RedArmor.customLanguageEntries
        );

        Equipments.Update("Blade", ERarity.Common, Weapon.BlueBlade.descriptor);
    }
}
