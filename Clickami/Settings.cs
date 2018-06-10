using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace Clickami
{
    internal class Settings
    {
        public static string PATHTODIRECTORY = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Clickami";
        public static string PATHTOSETTINGSXML = PATHTODIRECTORY + "\\settings.xml";

        public const string APPLICATIONNAME = "Clickami";
        public static int xCoord = 0;
        public static int yCoord = 0;
        public static double delay = 0.5;
        public static int loops = 1;
        public static bool isInfinitely = false;
        public static bool isTopmost = true;

        internal static string GetVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.FileVersion;
        }

        internal static void Read()
        {
            try
            {
                XDocument settings = XDocument.Load(Settings.PATHTOSETTINGSXML);
                XElement settingsElement = settings.Element("settings");
                if (settingsElement.Attribute("version").Value == Settings.GetVersion())
                {
                    try
                    {
                        Settings.xCoord = int.Parse(settingsElement.Element("coordinates").Attribute("x").Value);
                        Settings.yCoord = int.Parse(settingsElement.Element("coordinates").Attribute("y").Value);
                        Settings.delay = double.Parse(settingsElement.Element("delay").Value, System.Globalization.CultureInfo.InvariantCulture);
                        Settings.loops = int.Parse(settingsElement.Element("loops").Value);
                        Settings.isInfinitely = bool.Parse(settingsElement.Element("isInfinitely").Value);
                        Settings.isTopmost = bool.Parse(settingsElement.Element("isTopmost").Value);
                    }
                    catch (NullReferenceException)
                    {
                        Settings.CorruptionDetected();
                    }
                    catch (FormatException)
                    {
                        Settings.CorruptionDetected();
                    }
                }
                else
                {
                    MessageBox.Show("Old version " + settingsElement.Attribute("version").Value + " detected. Loading default values.", "Old version detected.", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (NullReferenceException) // Must be intercepted here again.
            {
                Settings.CorruptionDetected();
            }
            catch (XmlException)
            {
                Settings.CorruptionDetected();
            }
            catch (DirectoryNotFoundException)
            {
                // Do nothing / ignore
            }
        }

        internal static void Write()
        {
            XAttribute applicationName = new XAttribute("applicationName", Settings.APPLICATIONNAME);
            XAttribute version = new XAttribute("version", Settings.GetVersion());
            XAttribute x = new XAttribute("x", Settings.xCoord);
            XAttribute y = new XAttribute("y", Settings.yCoord);
            XElement coordinates = new XElement("coordinates", x, y);
            XElement delay = new XElement("delay", Settings.delay);
            XElement loops = new XElement("loops", Settings.loops);
            XElement isInfinitely = new XElement("isInfinitely", Settings.isInfinitely);
            XElement isTopmost = new XElement("isTopmost", Settings.isTopmost);
            XDocument settings = new XDocument(
                new XElement("settings",
                    applicationName,
                    version,
                    coordinates,
                    delay,
                    loops,
                    isInfinitely,
                    isTopmost
                )
            );
            Directory.CreateDirectory(Settings.PATHTODIRECTORY);
            settings.Save(Settings.PATHTOSETTINGSXML);
        }

        internal static void LoadDefault()
        {
            Settings.xCoord = 0;
            Settings.yCoord = 0;
            Settings.delay = 0.5;
            Settings.loops = 1;
            Settings.isInfinitely = false;
            Settings.isTopmost = true;
        }

        private static void CorruptionDetected()
        {
            Settings.LoadDefault();
            MessageBox.Show("Settings file corrupted! Loading default values.", "Settings file corrupted!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}