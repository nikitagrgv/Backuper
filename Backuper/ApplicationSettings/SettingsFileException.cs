using System;

namespace Backuper.ApplicationSettings
{
    internal abstract class SettingsFileException : ApplicationException
    {
        private readonly string _settingsFilename;

        protected SettingsFileException(string message, string settingsFilename)
            : base(message)
        {
            _settingsFilename = settingsFilename;
        }

        public string GetSettingsFilename()
        {
            return _settingsFilename;
        }
    }

    internal class BadSettingsFileException : SettingsFileException
    {
        public BadSettingsFileException(string settingsFilename)
            : base("Bad settings file", settingsFilename)
        {
        }
    }

    internal class NotFoundSettingsFileException : SettingsFileException
    {
        public NotFoundSettingsFileException(string settingsFilename)
            : base("Settings file not found", settingsFilename)
        {
        }
    }
}