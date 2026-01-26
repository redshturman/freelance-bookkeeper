using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace FreelanceBookkeeper.Configuration
{
    /// <summary>
    /// Represents a set of configuration settings for an application or component.
    /// </summary>
    public class Config
    {
        private static readonly string ConfigPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "FreelanceBookkeeper",
            "config.json");

        public int MonthsToShow { get; set; } = 3;
        
        public string Email { get; set; } = string.Empty;
        
        // SMTP Configuration
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public string SmtpUsername { get; set; } = string.Empty;
        public string SmtpPassword { get; set; } = string.Empty;
        public bool SmtpUseSsl { get; set; } = true;

        // Guardar configuración
        public void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);
            File.WriteAllText(ConfigPath, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
        }

        // Cargar configuración
        public static Config Load()
        {
            if (!File.Exists(ConfigPath))
                return new Config();

            var json = File.ReadAllText(ConfigPath);
            return JsonSerializer.Deserialize<Config>(json) ?? new Config();
        }
    }
}