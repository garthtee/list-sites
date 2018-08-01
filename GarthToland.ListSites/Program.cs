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
            var settings = LoadSettings();
            IContainer container = null;

            if (settings != null)
            {
                container = InversionOfControl.BuildContainer(settings);
                return;
            }

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
            Console.WriteLine($"Enter path of htdocs [{_localhostPath}]:");
            var input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input))
                _localhostPath = input;

            var settings = new Settings { LocalHostPath = _localhostPath };

            File.WriteAllText(SETTINGS_FILE, JsonConvert.SerializeObject(settings, Formatting.Indented));

            if (!File.Exists(SETTINGS_FILE))
                return null;

            Console.WriteLine($"You can find the settings file at {Path.GetFullPath(SETTINGS_FILE)}.");

            var settingsJson = File.ReadAllText(SETTINGS_FILE);

            return JsonConvert.DeserializeObject<Settings>(settingsJson);
        }
    }
}
