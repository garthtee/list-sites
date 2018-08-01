using Autofac;
using GarthToland.ListSites.Services;
using Newtonsoft.Json;
using System;
using System.IO;

namespace GarthToland.ListSites
{
    class Program
    {
        private static readonly string SETTINGS_FILE = "settings.json";
        private static string _localhostPath = "C:/xampp/htdocs/";

        private static void Main(string[] args)
        {
            var container = InversionOfControl.BuildContainer(LoadSettings());

            using (var scope = container.BeginLifetimeScope())
            {
                scope.Resolve<IListSitesService>().GenerateSiteList();
            }
#if DEBUG
            Console.ReadLine();
#endif
        }

        private static Settings LoadSettings()
        {
            string settingsJson = null;

            if (!ConstructSettingsFile())
            {
                return null;
            }

            settingsJson = File.ReadAllText(SETTINGS_FILE);

            return JsonConvert.DeserializeObject<Settings>(settingsJson);
        }

        private static bool ConstructSettingsFile()
        {
            ISettings settings = null;
            int attempts = 1;

            Console.WriteLine($"Enter path of htdocs [{_localhostPath}]:");
            var input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input))
                _localhostPath = input;

            while (settings == null)
            {
                if (attempts > 4)
                    return false;
                else if (attempts > 1)
                    Console.WriteLine($"Attempt {attempts} at building the settings file...");

                settings = new Settings { LocalHostPath = _localhostPath };
                attempts++;
            }

            File.WriteAllText(SETTINGS_FILE, JsonConvert.SerializeObject(settings, Formatting.Indented));

            Console.WriteLine($"You can find the settings file at {Path.GetFullPath(SETTINGS_FILE)}.");

            return true;
        }
    }
}
