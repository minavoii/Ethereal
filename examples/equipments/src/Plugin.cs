using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using Equipments.Weapons;

namespace Equipments;

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

        Ethereal.API.Equipment.Add(
            Accessories.RedArmor.descriptor,
            Accessories.RedArmor.localisationData,
            Accessories.RedArmor.customLanguageEntries
        );

        Ethereal.API.Equipment.Update("Blade", ERarity.Common, BlueBlade.descriptor);
    }
}
