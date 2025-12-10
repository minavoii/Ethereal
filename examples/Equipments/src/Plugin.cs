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

    private async void Awake()
    {
        Logger = base.Logger;

        await APIManager.WaitUntilReady(EtherealAPI.Equipments);

        Equipments.Add(Accessory.RedArmor.Builder);

        if (Equipments.TryGet("Blade", ERarity.Common, out Equipment equipment))
            Weapon.BlueBlade.View(equipment);
    }
}
