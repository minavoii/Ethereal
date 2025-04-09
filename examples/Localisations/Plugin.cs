using System.Timers;
using BepInEx;
using BepInEx.Logging;
using Ethereal.API;

namespace ExampleLocalisations;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Aethermancer.exe")]
[BepInDependency("minavoii.ethereal")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    internal static System.Timers.Timer timer = new() { Interval = 5000 };

    private void Awake()
    {
        Logger = base.Logger;

        Localisation.AddLocalisedText(Text.Apple.entry);
        Localisation.AddLocalisedText(TextExtra.Pear.entry, TextExtra.Pear.extras);

        // We're using a timer here because GameController may not be initialized yet
        // We are only calling `Loca.Localize()` here to check that it worked,
        //   but you only have to call `AddLocalisedText()`
        timer.Elapsed += new ElapsedEventHandler(TestLocalisations);
        timer.Start();
    }

    /// <summary>
    /// Display our custom localisation to see if they got successfully added. <para/>
    /// This is only for debugging, you do not need to run this.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TestLocalisations(object sender, ElapsedEventArgs e)
    {
        try
        {
            var currentLanguage = GameSettingsController.Instance.CurrentLanguage;
            GameSettingsController.Instance.CurrentLanguage = ELanguage.French;

            Logger.LogInfo(
                $"French localisation test: {Text.Apple.Original} -> {Loca.Localize(Text.Apple.Original)}"
            );
            Logger.LogInfo(
                $"French localisation test: {TextExtra.Pear.Original} -> {Loca.Localize(TextExtra.Pear.Original)}"
            );

            timer.Stop();

            GameSettingsController.Instance.CurrentLanguage = currentLanguage;
        }
        catch { }
    }
}
