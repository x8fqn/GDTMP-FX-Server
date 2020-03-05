//Code mostly by tmch :)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Noesis.Javascript; //can't use default arguments like > public void derp(string x = "blah"). it will require x to be filled :(

namespace Darkly.GDTMP
{
    internal class PluginJS : IDisposable
    {
        public readonly string Filename;
        public readonly string PluginName;
        public List<string> AddedCommands = new List<string>();
        public JavascriptContext Context;
        public List<string> HandledEvents = new List<string>();
        public List<Timeout> Intervals = new List<Timeout>();
        public List<Timeout> Timeouts = new List<Timeout>();
        private bool _initialized = false;
        private bool _stopping = false;

        public PluginJS(string filename)
        {
            Filename = filename;
            PluginName = Path.GetFileNameWithoutExtension(Filename);
            Context = dmultiplayersrv.tmch.PrepareContext(this);
        }

        public bool Initialized
        {
            get
            {
                return _initialized;
            }
            private set
            {
                _initialized = value;
            }
        }

        public bool Stopping
        {
            get
            {
                return _stopping;
            }
            private set
            {
                _stopping = value;
            }
        }

        public void Dispose()
        {
            Context.Dispose();
        }

        public void Start()
        {
            using (FileStream pluginstream = new FileStream(Filename, FileMode.Open))
            using (StreamReader pluginreader = new StreamReader(pluginstream))
                Context.Run(pluginreader.ReadToEnd());

            Initialized = true;
        }

        public void Stop()
        {
            foreach (string command in AddedCommands)
                dmultiplayersrv.RemoveCommand(command);

            Stopping = true;
            Context.TerminateExecution();
            Dispose();
        }
    }

    internal class PluginLoaderJS
    {
        public List<PluginJS> Plugins = new List<PluginJS>();

        public JavascriptContext PrepareContext(PluginJS plugin)
        {
            JavascriptContext boop = new JavascriptContext();
            boop.SetParameter("Server", new ServerObject(plugin));
            boop.SetParameter("pureServer", new dmultiplayersrv());
            return boop;
        }

        public void RunCode(string x)
        {
            JavascriptContext boop = PrepareContext(null);
            new Thread(() =>
            {
                try
                {
                    boop.Run(x);
                }
                catch (JavascriptException ex)
                {
                    dmultiplayersrv.PrintError("CODE RUN ERROR: " + ex.Message);
                }
            }).Start();
        }

        public void RunPlugin(string fimenale)
        {
            if (Path.GetExtension(fimenale) != dmultiplayersrv.JSPLUGIN_EXTENSION)
                throw new dMultiplayerException("Plugin must be of filetype \"" + dmultiplayersrv.JSPLUGIN_EXTENSION + "\".");

            foreach (PluginJS plugin in Plugins)
                if (plugin.Filename == fimenale)
                    throw new dMultiplayerException("The specified plugin is already running.");

            string pluginname = Path.GetFileNameWithoutExtension(fimenale);

            try
            {
                Plugins.Add(new PluginJS(fimenale));

                if (pluginname.ToLower().StartsWith("async_"))
                {
                    new Thread(() =>
                    {
                        StartPlugin(fimenale, pluginname);
                    }).Start();
                }
                else
                    StartPlugin(fimenale, pluginname);
            }
            catch (Exception ex)
            {
                dmultiplayersrv.PrintError("PLUGIN \"" + pluginname + "\" LOAD ERROR: " + ex.Message);
            }
        }

        public void StopPlugin(string fimenale)
        {
            foreach (PluginJS boop in Plugins)
            {
                if (boop.Filename == fimenale)
                {
                    boop.Stop();
                    Plugins.Remove(boop);
                    dmultiplayersrv.Print("Stopped " + Path.GetFileNameWithoutExtension(fimenale) + ".");
                    return;
                }
            }

            dmultiplayersrv.Print("The specified plugin isn't running.");
        }

        private void StartPlugin(string fimenale, string pluginname)
        {
            int index = Plugins.Count - 1;
            try
            {
                Plugins[index].Start();
                dmultiplayersrv.Print("Loaded " + pluginname + ".");
            }
            catch (JavascriptException doesnotcompute)
            {
                if (Plugins.Count > index && Plugins[index].Filename == fimenale && !Plugins[index].Stopping)
                    dmultiplayersrv.PrintError("PLUGIN \"" + pluginname + "\" RUN ERROR (LINE " + doesnotcompute.Line + "): " + doesnotcompute.Message);
            }
        }
    }

    internal class ServerObject : dmultiplayersrv
    {
        private PluginJS plugin;

        public ServerObject(PluginJS plugin)
        {
            this.plugin = plugin;
        }

        public void addCommand(string command, string handlerfunctionname)
        {
            addCommand(command, handlerfunctionname, null, null);
        }

        public void addCommand(string command, string handlerfunctionname, string helpitemargumenttext, string helpitemtext)
        {
            AddCommand(command, (args) =>
            {
                try
                {
                    plugin.Context.CallFunction(handlerfunctionname, args);
                }
                catch (Exception doesnotcompute)
                {
                    PrintError("PLUGIN \"" + plugin.PluginName + "\" COMMAND ERROR: " + doesnotcompute.Message);
                }
            }, helpitemargumenttext, helpitemtext);
            plugin.AddedCommands.Add(command);
        }

        public void addCustomSetting(string settingname, bool isprivate, object defaultvalue)
        {
            AddCustomSetting(settingname, isprivate, defaultvalue);
        }

        public void addEventHandler(string eventname, string handlerfunctionname)
        {
            AddEventHandler(eventname, (args) =>
            {
                try
                {
                    object retval = plugin.Context.CallFunction(handlerfunctionname, args);
                    if (retval is bool)
                        return (bool)retval;
                }
                catch (Exception doesnotcompute)
                {
                    PrintError("PLUGIN \"" + plugin.PluginName + "\" EVENT ERROR: " + doesnotcompute.Message);
                }
                return false;
            });
            plugin.HandledEvents.Add(eventname);
        }

        public void Ban(int playerid)
        {
            Command("ban " + playerid);
        }

        public void broadcastCustomData(string dataid, Array dataarray)
        {
            BroadcastData(dataid, dataarray.Cast<string>().ToArray());
        }

        public void broadcastMessage(string message)
        {
            BroadcastData("MSG", new string[] { message });
        }

        public void clearInterval(int intervalid)
        {
            if (plugin.Intervals.Count > intervalid)
                plugin.Intervals[intervalid].Cancel = true;
        }

        public void clearTimeout(int timeoutid)
        {
            if (plugin.Timeouts.Count > timeoutid)
                plugin.Timeouts[timeoutid].Cancel = true;
        }

        public string getClient(int playerid)
        {
            foreach (Client client in Clients)
            {
                if (client.ID == playerid)
                {
                    JSONWriter writer = new JSONWriter();

                    writer.WriteStartObject();
                    writer.WritePropertyName("ID");
                    writer.WriteValue(client.ID);
                    writer.WritePropertyName("AvgCosts");
                    writer.WriteValue(client.AvgCosts);
                    writer.WritePropertyName("AvgIncome");
                    writer.WriteValue(client.AvgIncome);
                    writer.WritePropertyName("AvgScore");
                    writer.WriteValue(client.AvgScore);
                    writer.WritePropertyName("Boss");
                    writer.WriteValue(client.Boss);
                    writer.WritePropertyName("Cash");
                    writer.WriteValue(client.Cash);
                    writer.WritePropertyName("Code");
                    writer.WriteValue(client.Code);
                    writer.WritePropertyName("CurrentWeek");
                    writer.WriteValue(client.CurrentWeek);
                    writer.WritePropertyName("Employees");
                    writer.WriteValue(client.Employees);
                    writer.WritePropertyName("Fans");
                    writer.WriteValue(client.Fans);
                    writer.WritePropertyName("FavouriteGenre");
                    writer.WriteValue(client.FavouriteGenre);
                    writer.WritePropertyName("GameCount");
                    writer.WriteValue(client.GameCount);
                    writer.WritePropertyName("HighScore");
                    writer.WriteValue(client.HighScore);
                    writer.WritePropertyName("IP");
                    writer.WriteValue(client.Context.ConnectionInfo.ClientIpAddress);

                    writer.WritePropertyName("Mods");
                    writer.WriteStartArray();
                    foreach (ClientMod mod in client.Mods)
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("ID");
                        writer.WriteValue(mod.ID);
                        writer.WritePropertyName("Name");
                        writer.WriteValue(mod.Name);
                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();

                    writer.WritePropertyName("Name");
                    writer.WriteValue(client.Name);
                    writer.WritePropertyName("Op");
                    writer.WriteValue(client.Op);
                    writer.WritePropertyName("PlatformCount");
                    writer.WriteValue(client.PlatformCount);
                    writer.WritePropertyName("Port");
                    writer.WriteValue(client.Context.ConnectionInfo.ClientPort);
                    writer.WritePropertyName("ResearchPoints");
                    writer.WriteValue(client.ResearchPoints);
                    writer.WriteEndObject();

                    return writer.GetString();
                }
            }

            return string.Empty;
        }

        public string getClients()
        {
            JSONWriter writer = new JSONWriter();

            writer.WriteStartArray();

            foreach (Client client in Clients)
                writer.WriteRaw(getClient((int)client.ID) + ',');

            writer.WriteEndArray();

            return writer.GetString();
        }

        public int getCommandHandlerCount(string command)
        {
            return GetCommandHandlerCount(command);
        }

        public string getCustomSetting(string settingname)
        {
            JSONWriter writer = new JSONWriter();

            writer.WriteStartArray();
            foreach (CustomSetting setting in CustomSettings)
            {
                if (setting.SettingName == settingname.ToLower())
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("Name");
                    writer.WriteValue(setting.SettingName);
                    writer.WritePropertyName("Private");
                    writer.WriteValue(setting.Private);
                    writer.WritePropertyName("Value");
                    writer.WriteValue(setting.Value);
                    writer.WriteEndObject();

                    break;
                }
            }
            writer.WriteEndArray();

            return writer.GetString();
        }

        public string getCustomSettings()
        {
            JSONWriter writer = new JSONWriter();

            writer.WriteStartArray();
            foreach (CustomSetting setting in CustomSettings)
                writer.WriteRaw(getCustomSetting(setting.SettingName));

            writer.WriteEndArray();

            return writer.GetString();
        }

        public string getExtension(string filenamewithoutextension)
        {
            foreach (PluginNET plugin in extloader.Plugins)
            {
                if (plugin.PluginName == filenamewithoutextension)
                {
                    JSONWriter writer = new JSONWriter();

                    writer.WriteStartObject();
                    writer.WritePropertyName("PluginName");
                    writer.WriteValue(plugin.PluginName);
                    writer.WriteEndObject();

                    return writer.GetString();
                }
            }

            return string.Empty;
        }

        public string getExtensions()
        {
            JSONWriter writer = new JSONWriter();

            writer.WriteStartArray();

            foreach (PluginNET plugin in extloader.Plugins)
                writer.WriteRaw(getExtension(plugin.PluginName) + ',');

            writer.WriteEndArray();

            return writer.GetString();
        }

        public int getPlayerCount()
        {
            return GetPlayerCount();
        }

        public string getPlugin(string filenamewithoutextension)
        {
            foreach (PluginJS plugin in tmch.Plugins)
            {
                if (plugin.PluginName == filenamewithoutextension)
                {
                    JSONWriter writer = new JSONWriter();

                    writer.WriteStartObject();
                    writer.WritePropertyName("PluginName");
                    writer.WriteValue(plugin.PluginName);

                    writer.WritePropertyName("AddedCommands");
                    writer.WriteStartArray();

                    foreach (string addedcommand in plugin.AddedCommands)
                        writer.WriteValue(addedcommand);

                    writer.WriteEndArray();

                    writer.WritePropertyName("HandledEvents");
                    writer.WriteStartArray();

                    foreach (string handledevent in plugin.HandledEvents)
                        writer.WriteValue(handledevent);

                    writer.WriteEndArray();
                    writer.WriteEndObject();

                    return writer.GetString();
                }
            }

            return string.Empty;
        }

        public string getPlugins()
        {
            JSONWriter writer = new JSONWriter();

            writer.WriteStartArray();

            foreach (PluginJS plugin in tmch.Plugins)
                writer.WriteRaw(getPlugin(plugin.PluginName) + ',');

            writer.WriteEndArray();

            return writer.GetString();
        }

        public object getPrivateSetting(string settingname)
        {
            FieldInfo field = PrivateSettings.GetType().GetField(settingname);
            return field.GetValue(PrivateSettings);
        }

        public string getPrivateSettings()
        {
            JSONWriter writer = new JSONWriter();

            writer.WriteStartObject();
            foreach (FieldInfo field in PrivateSettings.GetType().GetFields())
            {
                writer.WritePropertyName(field.Name);
                writer.WriteValue(field.GetValue(PrivateSettings));
            }
            writer.WriteEndObject();

            return writer.GetString();
        }

        public object getSetting(string settingname)
        {
            FieldInfo field = Settings.GetType().GetField(settingname);
            return field.GetValue(Settings);
        }

        public string getSettings()
        {
            JSONWriter writer = new JSONWriter();

            writer.WriteStartObject();
            foreach (FieldInfo field in Settings.GetType().GetFields())
            {
                writer.WritePropertyName(field.Name);
                writer.WriteValue(field.GetValue(Settings));
            }
            writer.WriteEndObject();

            return writer.GetString();
        }

        public string getVersion()
        {
            return SERVERVERSION;
        }

        public void Kick(int playerid)
        {
            dmultiplayersrv.Kick(playerid);
        }

        public void KickR(int playerid, string reason, string consoledisplayreason)
        {
            Kick(playerid, reason, consoledisplayreason);
        }

        public void Log(string text)
        {
            Print(text);
        }

        public void logError(string text)
        {
            PrintError(text);
        }

        public void logWarning(string text)
        {
            PrintWarning(text);
        }

        public void runCommand(string command)
        {
            Command(command);
        }

        public void sendCustomData(int playerid, string dataid, Array dataarray)
        {
            SendData(playerid, dataid, dataarray.Cast<string>().ToArray());
        }

        public void sendMessage(int playerid, string message)
        {
            SendData(playerid, "PRIVMSG", new string[] { playerid.ToString(), message });
        }

        public void setCustomSetting(string settingname, object value)
        {
            SetCustomSetting(settingname, value);
        }

        public int setInterval(string functionname, int milliseconds)
        {
            return setInterval(functionname, milliseconds, new object[] { });
        }

        public int setInterval(string functionname, int milliseconds, Array args)
        {
            int intervalindex = plugin.Intervals.Count;
            plugin.Intervals.Add(new Timeout());

            new Thread(() =>
            {
                while (!plugin.Intervals[intervalindex].Cancel)
                {
                    Thread.Sleep(milliseconds);
                    if (!plugin.Intervals[intervalindex].Cancel)
                        plugin.Context.CallFunction(functionname, args);
                }
            }).Start();

            return intervalindex;
        }

        public void setPrivateSetting(string settingname, object value)
        {
            SetPrivateSetting(settingname, value);
        }

        public void setSetting(string settingname, object value)
        {
            SetSetting(settingname, value);
        }

        public int setTimeout(string functionname, int milliseconds)
        {
            return setTimeout(functionname, milliseconds, new object[] { });
        }

        public int setTimeout(string functionname, int milliseconds, Array args)
        {
            int timeoutindex = plugin.Timeouts.Count;
            plugin.Timeouts.Add(new Timeout());

            new Thread(() =>
            {
                Thread.Sleep(milliseconds);
                if (!plugin.Timeouts[timeoutindex].Cancel)
                    plugin.Context.CallFunction(functionname, args);
            }).Start();

            return timeoutindex;
        }

        private class JSONWriter
        {
            private StringBuilder jsonstringbuilder;

            public JSONWriter()
            {
                jsonstringbuilder = new StringBuilder(string.Empty);
            }

            public string GetString()
            {
                RemoveStringBuilderTrailingComma();
                return jsonstringbuilder.ToString();
            }

            public void WriteEndArray()
            {
                RemoveStringBuilderTrailingComma();
                jsonstringbuilder.Append("],");
            }

            public void WriteEndObject()
            {
                RemoveStringBuilderTrailingComma();
                jsonstringbuilder.Append("},");
            }

            public void WritePropertyName(string name)
            {
                jsonstringbuilder.Append('"' + name + "\":");
            }

            public void WriteRaw(string value)
            {
                jsonstringbuilder.Append(value);
            }

            public void WriteStartArray()
            {
                jsonstringbuilder.Append('[');
            }

            public void WriteStartObject()
            {
                jsonstringbuilder.Append('{');
            }

            public void WriteValue(object value)
            {
                if (value == null)
                    jsonstringbuilder.Append("null,");

                Type valuetype = value.GetType();
                double tempdouble;

                if (valuetype == typeof(string))
                    jsonstringbuilder.Append('"' + (string)value + "\",");
                else if (valuetype == typeof(bool))
                    jsonstringbuilder.Append(value.ToString().ToLower() + ',');
                else if (double.TryParse(value.ToString(), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out tempdouble))
                    jsonstringbuilder.Append(value.ToString() + ',');
                else
                    jsonstringbuilder.Append("undefined,");
            }

            private void RemoveStringBuilderTrailingComma()
            {
                string jsonstring = jsonstringbuilder.ToString();
                if (jsonstring[jsonstring.Length - 1] == ',')
                    jsonstringbuilder.Remove(jsonstring[jsonstring.Length - 1], 1);
            }
        }
    }

    internal class Timeout
    {
        public bool Cancel = false;
    }
}