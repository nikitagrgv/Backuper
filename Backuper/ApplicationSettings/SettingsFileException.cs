using System;

namespace Backuper.ApplicationSettings
{
    internal class SettingsFileException : ApplicationException
    {
        private readonly string _settingsFileName;

        public SettingsFileException(string message, string settingsFileName)
            : base(message)
        {
            _settingsFileName = settingsFileName;
        }

        public string GetSettingsFileName()
        {
            return _settingsFileName;
        }
    }

    internal class BadJsonFileException : SettingsFileException
    {
        public BadJsonFileException(string settingsFileName)
            : base("Bad settings json file", settingsFileName)
        {
        }
    }

    internal class NotFoundSettingsFileException : SettingsFileException
    {
        public NotFoundSettingsFileException(string settingsFileName)
            : base("Settings file not found", settingsFileName)
        {
        }
    }
}