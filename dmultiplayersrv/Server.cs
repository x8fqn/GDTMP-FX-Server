using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Fleck;
using NATUPNPLib;

namespace Darkly.GDTMP
{
    [Serializable]
    public class dMultiplayerException : Exception
    {
        public dMultiplayerException()
        {
        }

        public dMultiplayerException(string message)
            : base(message)
        {
        }
    }

    public class dmultiplayersrv
    {
        #region Debug/Release settings

#if DEBUG
    private const int MAXIMUM_CONNECTIONS_PER_IP = 20;
    private const int MAXIMUM_INPUT_LENGTH = 8192;
    private const bool CATCH_EXCEPTIONS = false;
    private const bool EXTRA_VERBOSE = true;
    private const bool HIDE_DATA = false;
    private const bool SEND_SELF = true;
    private const bool OFFICIAL = true;
        private const bool ALLOW_LIST_CMDS = true;
        private const bool PRINT_STACK_TRACES = true;
#else
        private const bool ALLOW_LIST_CMDS = false;
        private const bool CATCH_EXCEPTIONS = true;
        private const bool EXTRA_VERBOSE = false;
        private const bool HIDE_DATA = true;
        private const int MAXIMUM_CONNECTIONS_PER_IP = 5;
        private const int MAXIMUM_INPUT_LENGTH = 4096;
        private const bool OFFICIAL = false;
        private const bool PRINT_STACK_TRACES = false;
        private const bool SEND_SELF = false;
#endif

        #endregion Debug/Release settings

        public const string BACKUPFOLDER = "old";
        public const string BANNEDIPSFILE = "bannedips.txt";
        public const string CLIENTSAVEFOLDER = "clients";
        public const string CONFIGFOLDER = "config";
        public const string CUSTOMSETTINGSFILE = "customsettings.txt";
        public const string ENABLEDEXTENSIONSFILE = "extensions.txt";
        public const string ENABLEDPLUGINSFILE = "plugins.txt";
        public const string EXTENSIONFOLDER = "extensions";
        public const string JSPLUGIN_EXTENSION = ".js";
        public const string MACROFOLDER = "macros";
        public const string MINVERSION = "0.5.3";
        public const string NETPLUGIN_EXTENSION = ".dll";
        public const string OPPEDIPSFILE = "oppedips.txt";
        public const string PLUGINFOLDER = "plugins";
        public const string PRIVATESETTINGSFILE = "privatesettings.txt";
        public const string SEPERATOR = "\xFA";
        public const string SEPERATOR2 = "\xFB";
        public const string SEPERATOR3 = "\xFC";
        public const string SEPERATOR4 = "\xFD";
        public const string SERVERID = "133333337";
        public const string SERVERVERSION = "1.5.5.0";
        public const string SETTINGSFILE = "settings.txt";
        public const string STARTUPMACROFILE = "startup.txt";
        public static List<string> BannedIPs = new List<string>();
        public static long ClientCount = 0;
        public static List<Client> Clients = new List<Client>();
        public static int CurrentPort;
        public static List<CustomSetting> CustomSettings = new List<CustomSetting>();
        public static bool ExternalApp = false;
        public static List<string> OppedIPs = new List<string>();
        public static long PlayerCount = 0;
        public static PrivSettings PrivateSettings = new PrivSettings();
        public static bool ServerVisible = false;
        public static ServerSettings Settings = new ServerSettings();

        internal static PluginLoaderNET extloader;

        internal static CommandMacro thegreat;
        internal static PluginLoaderJS tmch;

        private const string OLDBANNEDIPSFILE = "banned_ips.txt";
        private const string OLDOPPEDIPSFILE = "opped_ips.txt";
        private const string PRINTSEPERATOR = "\", \"";
        private static IPEndPoint[] activeconnections;
        private static bool autoportforwarded = false;
        private static bool closing = false;
        private static string cmdbuffer = string.Empty;
        private static ConsoleEventDelegate ctrlhandler = new ConsoleEventDelegate(OnClose);
        private static WebSocketServer dServer;
        private static List<string> enabledextensions = new List<string>();
        private static List<string> enabledplugins = new List<string>();
        private static List<KeyValuePair<string, string>> extrahelpitems = new List<KeyValuePair<string, string>>();
        private static bool lineclear = false;
        private static object lockobj = new object();
        private static MD5 md5;
        private static bool newerversion = false;
        private static List<KeyValuePair<string, Action<string[]>>> plugincommands = new List<KeyValuePair<string, Action<string[]>>>();
        private static List<KeyValuePair<string, Func<object[], bool>>> plugineventhandlers = new List<KeyValuePair<string, Func<object[], bool>>>();
        private static IStaticPortMappingCollection portmappings;
        private static Timer refreshtimer;
        private static bool serverinitialized = false;
        private static Timer updatetimer;
        private static UPnPNATClass upnpnat;

        private delegate bool ConsoleEventDelegate(int ctrlType);

        public static void AddCommand(string command, Action<string[]> handler, string helpitemargumenttext = null, string helpitemtext = null)
        {
            foreach (KeyValuePair<string, Action<string[]>> existinghandler in plugincommands)
                if (existinghandler.Key == command)
                    throw new dMultiplayerException("There is already a handler for \"" + command + "\"");

            plugincommands.Add(new KeyValuePair<string, Action<string[]>>(command, handler));

            if (helpitemtext != null)
                extrahelpitems.Add(new KeyValuePair<string, string>(command + (!string.IsNullOrEmpty(helpitemargumenttext) ? " " + helpitemargumenttext : string.Empty), helpitemtext));
        }

        public static void AddCustomSetting(string settingname, bool isprivate, object defaultvalue)
        {
            string lowercasename = settingname.ToLower();

            foreach (CustomSetting customsetting in CustomSettings)
            {
                if (customsetting.SettingName == lowercasename)
                {
                    if (customsetting.addedonload)
                        return;
                    else
                        throw new dMultiplayerException("Custom setting \"" + lowercasename + "\" already exists as a custom setting! (Setting names are case-insensitive)");
                }
            }

            if (!isprivate)
            {
                if (Settings.GetType().GetField(lowercasename) != null)
                    throw new dMultiplayerException("Custom setting \"" + lowercasename + "\" already exists as a regular setting! (Setting names are case-insensitive)");
            }
            else
            {
                if (PrivateSettings.GetType().GetField(lowercasename) != null)
                    throw new dMultiplayerException("Custom setting \"" + lowercasename + "\" already exists as a private setting! (Setting names are case-insensitive)");
            }

            CustomSettings.Add(new CustomSetting(lowercasename, isprivate, defaultvalue));
            SaveSettings();
        }

        public static void AddEventHandler(string eventname, Func<object[], bool> handler)
        {
            plugineventhandlers.Add(new KeyValuePair<string, Func<object[], bool>>(eventname, handler));
        }

        public static void BroadcastData(string dataid, string[] dataarray)
        {
            SendAll(dataid + SEPERATOR + string.Join(SEPERATOR, dataarray));
        }

        public static void Command(string command, IWebSocketConnection callback = null)
        {
            string[] splitcommand = command.Split(' ');

            if (FireEvent(Events.BEFORECOMMAND, command))
                return;

            switch (splitcommand[0].ToLower())
            {
                case "addtolist":
                    if (ALLOW_LIST_CMDS)
                    {
                        if (callback == null || (callback != null && Settings.extendedopprivs))
                        {
                            if (splitcommand.Length > 1)
                            {
                                WebClient alwc = new WebClient();
                                alwc.DownloadStringCompleted += alwc_DownloadStringCompleted;
                                alwc.DownloadStringAsync(new Uri("http://data.gdtmp.tk/addtolist?nick=" + splitcommand[1]));
                            }
                            else
                                PrintInvalidSyntax(callback);
                        }
                        else
                            PrintConsoleOnlyCommand(callback);
                    }
                    else
                        PrintC("This feature is no longer supported.", callback);

                    break;

                case "ban":
                    if (splitcommand.Length > 1)
                    {
                        try
                        {
                            long id = ConvertToLong(splitcommand[1]);
                            string ip = Clients[GetIndex(id)].Context.ConnectionInfo.ClientIpAddress;

                            if (!BannedIPs.Contains(ip))
                            {
                                BannedIPs.Add(ip);
                                SaveSettings();
                                PrintC("Banned client.", callback);
                                FireEvent(Events.ONPLAYERBAN, id);
                            }
                            else
                                PrintC("Client already banned.", callback);

                            if (splitcommand.Length > 2)
                                Kick(id, "You have been banned (" + splitcommand[2] + ")", "Banned client (" + splitcommand[2] + ")");
                            else
                                Kick(id, "You have been banned", "Banned client");
                        }
                        catch
                        {
                            PrintC("Couldn't ban client. (Not connected?)", callback);
                        }
                    }
                    else
                        PrintInvalidSyntax(callback);

                    break;

                case "banip":
                    if (splitcommand.Length > 1)
                    {
                        string ip = splitcommand[1];

                        if (!BannedIPs.Contains(ip))
                        {
                            BannedIPs.Add(ip);
                            SaveSettings();
                            PrintC("Banned IP.", callback);
                        }
                        else
                            PrintC("IP already banned.", callback);
                    }
                    else
                        PrintInvalidSyntax(callback);

                    break;

                case "banned":
                    PrintC("Banned IPs:", callback);
                    PrintLine(callback);

                    if (BannedIPs.Count > 0)
                        foreach (string ip in BannedIPs)
                            PrintC(ip, callback);
                    else
                        PrintC("No IPs banned.", callback);

                    PrintLine(callback);
                    break;

                case "c":
                    if (callback == null || (callback != null && Settings.extendedopprivs))
                        if (splitcommand.Length > 1)
                            tmch.RunCode(command.Split(new char[] { ' ' }, 2)[1]);
                        else
                            PrintInvalidSyntax(callback);
                    else
                        PrintConsoleOnlyCommand(callback);

                    break;

                case "deop":
                    if (callback == null || (callback != null && Settings.extendedopprivs))
                    {
                        if (splitcommand.Length > 1)
                        {
                            try
                            {
                                long id = ConvertToLong(splitcommand[1]);
                                int index = GetIndex(id);
                                string ip = Clients[index].Context.ConnectionInfo.ClientIpAddress;

                                if (Clients[GetIndex(id)].Op)
                                {
                                    Clients[GetIndex(id)].Op = false;

                                    if (OppedIPs.Contains(ip))
                                    {
                                        OppedIPs.Remove(ip);
                                        SaveSettings();
                                    }
                                    PrintC("De-opped client.", callback);

                                    Clients[index].Context.Send("OPSTATUS" + SEPERATOR + "false" + SEPERATOR + SERVERID);
                                }
                                else
                                    PrintC("Client isn't opped.", callback);
                            }
                            catch
                            {
                                PrintC("Couldn't de-op client. (Not connected?)", callback);
                            }
                        }
                        else
                            PrintInvalidSyntax(callback);
                    }
                    else
                        PrintConsoleOnlyCommand(callback);

                    break;

                case "deopip":
                    if (callback == null || (callback != null && Settings.extendedopprivs))
                    {
                        if (splitcommand.Length > 1)
                        {
                            string ip = splitcommand[1];

                            if (OppedIPs.Contains(ip))
                            {
                                OppedIPs.Remove(ip);
                                SaveSettings();
                                PrintC("De-opped IP.", callback);
                            }
                            else
                                PrintC("IP isn't opped.", callback);
                        }
                        else
                            PrintInvalidSyntax(callback);
                    }
                    else
                        PrintConsoleOnlyCommand(callback);

                    break;

                case "disableext":
                    if (callback == null || (callback != null && Settings.extendedopprivs))
                    {
                        if (splitcommand.Length > 1)
                        {
                            string pluginname = command.Split(new char[] { ' ' }, 2)[1];
                            if (pluginname.EndsWith(NETPLUGIN_EXTENSION))
                                pluginname = pluginname.Remove(pluginname.IndexOf(NETPLUGIN_EXTENSION));

                            DisableExtension(pluginname);
                        }
                        else
                            PrintInvalidSyntax(callback);
                    }
                    else
                        PrintConsoleOnlyCommand(callback);

                    break;

                case "disableplugin":
                    if (callback == null || (callback != null && Settings.extendedopprivs))
                    {
                        if (splitcommand.Length > 1)
                        {
                            string pluginname = command.Split(new char[] { ' ' }, 2)[1];
                            if (pluginname.EndsWith(JSPLUGIN_EXTENSION))
                                pluginname = pluginname.Remove(pluginname.IndexOf(JSPLUGIN_EXTENSION));

                            DisablePlugin(pluginname);
                        }
                        else
                            PrintInvalidSyntax(callback);
                    }
                    else
                        PrintConsoleOnlyCommand(callback);

                    break;

                case "enableext":
                    if (callback == null || (callback != null && Settings.extendedopprivs))
                    {
                        if (splitcommand.Length > 1)
                        {
                            string pluginname = command.Split(new char[] { ' ' }, 2)[1];
                            if (pluginname.EndsWith(NETPLUGIN_EXTENSION))
                                pluginname = pluginname.Remove(pluginname.IndexOf(NETPLUGIN_EXTENSION));

                            if (File.Exists(Path.Combine(EXTENSIONFOLDER, pluginname + NETPLUGIN_EXTENSION)))
                                EnableExtension(pluginname);
                            else
                                PrintC("Extension file doesn't exist or is not " + NETPLUGIN_EXTENSION, callback);
                        }
                        else
                            PrintInvalidSyntax(callback);
                    }
                    else
                        PrintConsoleOnlyCommand(callback);

                    break;

                case "enableplugin":
                    if (callback == null || (callback != null && Settings.extendedopprivs))
                    {
                        if (splitcommand.Length > 1)
                        {
                            string pluginname = command.Split(new char[] { ' ' }, 2)[1];
                            if (pluginname.EndsWith(JSPLUGIN_EXTENSION))
                                pluginname = pluginname.Remove(pluginname.IndexOf(JSPLUGIN_EXTENSION));

                            if (File.Exists(Path.Combine(PLUGINFOLDER, pluginname + JSPLUGIN_EXTENSION)))
                                EnablePlugin(pluginname);
                            else
                                PrintC("Plugin file doesn't exist or is not " + JSPLUGIN_EXTENSION, callback);
                        }
                        else
                            PrintInvalidSyntax(callback);
                    }
                    else
                        PrintConsoleOnlyCommand(callback);

                    break;

                case "exit":
                    if (callback == null || (callback != null && Settings.extendedopprivs))
                    {
                        PrintC("Unregistering server...", callback);
                        UnregisterServer(true, false);
                        PrintC("Saving settings...", callback);
                        SaveSettings();
                        PrintC("Exiting...", callback);
                        dServer.Dispose();
                        OnClose(1337);
                        Environment.Exit(0);
                    }
                    else
                        PrintConsoleOnlyCommand(callback);

                    break;

                case "help":
                    PrintC("List of commands:", callback);
                    PrintLine(callback);
                    PrintC("ban <playerid> <reason (optional)> - Kicks and bans a player's IP", callback);
                    PrintC("banip <ip> - Kicks and bans a player by their IP", callback);
                    PrintC("banned - Shows list of banned IPs", callback);
                    PrintC("c <code> - Runs in-line JavaScript", callback);
                    PrintC("deop <playerid> - Removes operator powers from a player", callback);
                    PrintC("deopip <ip> - Removes operator powers from the specified IP", callback);
                    PrintC("disableext <filename> - Disables and terminates an extension", callback);
                    PrintC("disableplugin <filename> - Disables and terminates a plugin", callback);
                    PrintC("enableext <filename> - Enables and runs an " + NETPLUGIN_EXTENSION + " extension", callback);
                    PrintC("enableplugin <filename> - Enables and runs a " + JSPLUGIN_EXTENSION + " plugin", callback);
                    PrintC("exit - Saves settings and shuts down the server", callback);
                    PrintC("help - Shows a list of commands", callback);
                    PrintC("kick <playerid> <reason (optional)> - Kicks a player", callback);
                    PrintC("msg <message> - Broadcasts a message to all players", callback);
                    PrintC("myip - Shows your public IP", callback);
                    PrintC("op <playerid> - Gives a player operator powers", callback);
                    PrintC("opped - Shows list of opped IPs", callback);
                    PrintC("opip <ip> - Gives a specific IP operator powers (requires permaopip)", callback);
                    PrintC("players - Shows a list of online players", callback);
                    PrintC("playerinfo <playerid> - Shows details about the specified player", callback);
                    PrintC("playermods <playerid> - Shows enabled mods of the specified player", callback);
                    PrintC("privmsg <playerid> <message> - Send message to the specified player", callback);
                    PrintC("runmacro <filename> - Runs a .txt macro from the \"macros\" folder", callback);
                    PrintC("set - Shows information about changing settings", callback);
                    PrintC("settings - Shows current setting values", callback);
                    PrintC("site - Navigates to the official forum thread", callback);
                    PrintC("stats - Shows connection stats", callback);
                    PrintC("unban <ip> - Unbans the specified IP", callback);
                    PrintC("update - Checks for updates", callback);
                    PrintLine(callback);

                    if (extrahelpitems.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> helpitem in extrahelpitems)
                            PrintC(helpitem.Key + " - " + helpitem.Value, callback);

                        PrintLine(callback);
                    }
                    break;

                case "kick":
                    if (splitcommand.Length > 1)
                    {
                        if (splitcommand.Length > 2)
                            Kick(ConvertToLong(splitcommand[1]), splitcommand[2]);
                        else
                            Kick(ConvertToLong(splitcommand[1]));
                    }
                    else
                        PrintInvalidSyntax(callback);

                    break;

                case "msg":
                    if (splitcommand.Length > 1)
                    {
                        string message = command.Split(new char[] { ' ' }, 2)[1];
                        SendAll("MSG" + SEPERATOR + message);
                        PrintC("Message sent.", callback);
                    }
                    else
                        PrintInvalidSyntax(callback);

                    break;

                case "myip":
                    if (callback == null || (callback != null && Settings.extendedopprivs))
                        MyIP();
                    else
                        PrintConsoleOnlyCommand(callback);

                    break;

                case "op":
                    if (callback == null || (callback != null && Settings.extendedopprivs))
                    {
                        if (splitcommand.Length > 1)
                        {
                            try
                            {
                                long id = ConvertToLong(splitcommand[1]);
                                int index = GetIndex(id);
                                string ip = Clients[index].Context.ConnectionInfo.ClientIpAddress;

                                if (!Clients[index].Op)
                                {
                                    Clients[index].Op = true;

                                    if (Settings.permaopip && !OppedIPs.Contains(ip))
                                    {
                                        OppedIPs.Add(ip);
                                        SaveSettings();
                                        FireEvent(Events.ONPLAYEROP, id);
                                    }
                                    PrintC("Opped client.", callback);

                                    Clients[index].Context.Send("OPSTATUS" + SEPERATOR + "true" + SEPERATOR + SERVERID);
                                }
                                else
                                    PrintC("Client already op.", callback);
                            }
                            catch
                            {
                                PrintC("Couldn't op client. (Not connected?)", callback);
                            }
                        }
                        else
                            PrintInvalidSyntax(callback);
                    }
                    else
                        PrintConsoleOnlyCommand(callback);

                    break;

                case "opip":
                    if (callback == null || (callback != null && Settings.extendedopprivs))
                    {
                        if (Settings.permaopip)
                        {
                            if (splitcommand.Length > 1)
                            {
                                string ip = splitcommand[1];

                                if (!OppedIPs.Contains(ip))
                                {
                                    OppedIPs.Add(ip);
                                    SaveSettings();
                                    PrintC("Opped IP.", callback);
                                }
                                else
                                    PrintC("IP is already opped.", callback);
                            }
                            else
                                PrintInvalidSyntax(callback);
                        }
                        else
                            PrintC("The server has been set to not allow permanent opping.", callback);
                    }
                    else
                        PrintConsoleOnlyCommand(callback);

                    break;

                case "opped":
                    PrintC("Opped IPs:", callback);
                    PrintLine(callback);

                    if (OppedIPs.Count > 0)
                        foreach (string ip in OppedIPs)
                            PrintC(ip, callback);
                    else
                        PrintC("No IPs opped.", callback);

                    PrintLine(callback);
                    break;

                case "playerinfo":
                    try
                    {
                        if (splitcommand.Length > 1)
                        {
                            Client client = Clients[GetIndex(ConvertToLong(splitcommand[1]))];

                            PrintC("Company name: {0}", callback, client.Name);
                            PrintC("Company boss: {0}", callback, client.Boss);
                            PrintC("Cash: {0}", callback, ShortNumber(client.Cash));
                            PrintC("Fans: {0}", callback, ShortNumber(client.Fans));
                            PrintC("Research points: {0}", callback, client.ResearchPoints);
                            PrintC("Current week: {0}", callback, WeekString(client.CurrentWeek));
                            PrintC("Employees: {0}", callback, client.Employees);
                            PrintC("Platforms released: {0}", callback, client.PlatformCount);
                            PrintC("Games released: {0}", callback, client.GameCount);
                            PrintC("Favourite genre: {0}", callback, client.FavouriteGenre);
                            PrintC("Avg. game costs: {0}", callback, client.AvgCosts);
                            PrintC("Avg. game income: {0}", callback, client.AvgIncome);
                            PrintC("Avg. game income: {0}", callback, client.AvgIncome - client.AvgCosts);
                            PrintC("Avg. game score: {0}", callback, client.AvgScore);
                            PrintC("Highscore: {0}", callback, client.HighScore);
                            PrintLine(callback);
                            PrintC("Op: {0}", callback, client.Op ? "Yes" : "No");
                            PrintC("IP and port: {0}", callback, GetIP(client.Context));
                        }
                        else
                            PrintInvalidSyntax(callback);
                    }
                    catch
                    {
                        Print("Couldn't display information (invalid ID?)");
                    }

                    break;

                case "playermods":
                    try
                    {
                        if (splitcommand.Length > 1)
                        {
                            Client client = Clients[GetIndex(ConvertToLong(splitcommand[1]))];

                            PrintC("Mods enabled for player with ID {0}:", callback, client.ID);
                            PrintLine(callback);

                            if (client.Mods.Count > 0)
                                foreach (ClientMod mod in client.Mods)
                                    PrintC("{0} ({1})", callback, mod.Name, mod.ID);
                            else
                                PrintC("No mods enabled, not even GDTMP, it seems like. But it's probably not true :P", callback);

                            PrintLine(callback);
                        }
                        else
                            PrintInvalidSyntax(callback);
                    }
                    catch
                    {
                        Print("Couldn't display information (invalid ID?)");
                    }

                    break;

                case "players":
                    PrintC("Player list:", callback);
                    PrintLine(callback);

                    int playersshown = 0;
                    foreach (Client client in Clients)
                    {
                        PrintC("ID: {0}" + (client.Name != null ? "   Name: {1}" : string.Empty) + "IP: {2}   Op: {3}", callback, GetID(client.Context), (client.Name != null ? client.Name : string.Empty), GetIP(client.Context), client.Op);
                        playersshown++;
                    }

                    if (playersshown < 1)
                        PrintC("No players connected.", callback);

                    PrintLine(callback);
                    break;

                case "privmsg":
                    try
                    {
                        string message = command.Split(new char[] { ' ' }, 3)[2];

                        if (splitcommand.Length > 2)
                            Clients[GetIndex(ConvertToLong(splitcommand[1]))].Context.Send("PRIVMSG" + SEPERATOR + splitcommand[1] + SEPERATOR + message + SEPERATOR + SERVERID);
                        else
                            PrintInvalidSyntax(callback);

                        Print("Private message sent.");
                    }
                    catch
                    {
                        Print("Couldn't send message (invalid ID?)");
                    }

                    break;

                case "rmfromlist":
                    if (ALLOW_LIST_CMDS)
                    {
                        if (callback == null || (callback != null && Settings.extendedopprivs))
                        {
                            WebClient rmwc = new WebClient();
                            rmwc.DownloadStringCompleted += rmwc_DownloadStringCompleted;
                            rmwc.DownloadStringAsync(new Uri("http://data.gdtmp.tk/rmfromlist"));
                        }
                        else
                            PrintConsoleOnlyCommand(callback);
                    }
                    else
                        PrintC("This feature is no longer supported.", callback);

                    break;

                case "runmacro":
                    if (callback == null || (callback != null && Settings.extendedopprivs))
                    {
                        if (splitcommand.Length > 1)
                        {
                            string[] splitonce = command.Split(new char[] { ' ' }, 2);
                            string macrofile = Path.Combine(MACROFOLDER, splitonce[1] + (!splitonce[1].EndsWith(".txt") ? ".txt" : string.Empty));
                            if (File.Exists(macrofile) && Path.GetExtension(macrofile) == ".txt")
                                thegreat.RunByFile(macrofile);
                            else
                                PrintC("Macro file doesn't exist or is not .txt", callback);
                        }
                        else
                            PrintInvalidSyntax(callback);
                    }
                    else
                        PrintConsoleOnlyCommand(callback);

                    break;

                case "runext":
                    PrintC("This feature is no longer supported, please use \"enableext <filename>\" instead.", callback);
                    break;

                case "runplugin":
                    PrintC("This feature is no longer supported, please use \"enableplugin <filename>\" instead.", callback);
                    break;

                case "sendraw":
                    if (OFFICIAL && splitcommand.Length > 1)
                        SendAll(splitcommand[1].Replace(":,:", SEPERATOR));
                    else
                        PrintInvalidSyntax(callback);

                    break;

                case "sendrawid":
                    if (OFFICIAL && splitcommand.Length > 2)
                        Clients[GetIndex(ConvertToLong(splitcommand[1]))].Context.Send(splitcommand[2].Replace(":,:", SEPERATOR) + SEPERATOR + SERVERID);
                    else
                        PrintInvalidSyntax(callback);

                    break;

                case "set":
                    if (callback == null || (callback != null && Settings.extendedopprivs))
                    {
                        if (splitcommand.Length > 1)
                        {
                            switch (splitcommand[1])
                            {
                                case "help":
                                    ShowSetHelp(callback);
                                    break;

                                default:
                                    string[] setcommand = command.Split(new char[] { ' ' }, 3);
                                    if (setcommand.Length > 2)
                                    {
                                        try
                                        {
                                            SetSetting(setcommand[1], setcommand[2]);
                                            PrintC("Set \"{0}\" to \"{1}\"", callback, setcommand[1], setcommand[2]);
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                SetCustomSetting(setcommand[1], setcommand[2], false);
                                                PrintC("Set custom setting \"{0}\" to \"{1}\"", callback, setcommand[1], setcommand[2]);
                                            }
                                            catch
                                            {
                                                PrintC("No such setting/invalid value.", callback);
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                        else
                            ShowSetHelp(callback);
                    }
                    else
                        PrintConsoleOnlyCommand(callback);

                    break;

                case "settings":
                    foreach (FieldInfo field in Settings.GetType().GetFields())
                        PrintC("{0}: {1}", callback, field.Name, field.GetValue(Settings));

                    if (CustomSettings.Count > 0)
                    {
                        PrintLine(callback);
                        foreach (CustomSetting setting in CustomSettings)
                            if (!setting.Private)
                                PrintC("{0}: {1}", callback, setting.SettingName, setting.Value);
                    }

                    break;

                case "site":
                    if (callback == null || (callback != null && Settings.extendedopprivs))
                        Process.Start("http://forum.greenheartgames.com/t/wip-gdtmp-multiplayer-mod-0-5-9-1-5-5-0/10509");
                    else
                        PrintConsoleOnlyCommand(callback);

                    break;

                case "stats":
                    PrintC("Total player connections: {0}", callback, PlayerCount);
                    PrintC("Total polling connections: {0}", callback, ClientCount - PlayerCount);
                    break;

                case "stopext":
                    PrintC("This feature is no longer supported, please use \"disableext <filename>\" instead.", callback);
                    break;

                case "stopplugin":
                    PrintC("This feature is no longer supported, please use \"disableplugin <filename>\" instead.", callback);
                    break;

                case "unban":
                    if (splitcommand.Length > 1)
                    {
                        string ip = splitcommand[1];

                        if (BannedIPs.Contains(ip))
                        {
                            BannedIPs.Remove(ip);
                            SaveSettings();
                            PrintC("Unbanned IP.", callback);
                        }
                        else
                            PrintC("IP isn't banned.", callback);
                    }
                    else
                        PrintInvalidSyntax(callback);

                    break;

                case "update":
                    CheckUpdate(false);
                    break;

                default:
                    foreach (KeyValuePair<string, Action<string[]>> plugincommand in plugincommands)
                    {
                        if (plugincommand.Key == splitcommand[0])
                        {
                            plugincommand.Value(splitcommand.Skip(1).ToArray());
                            break;
                        }
                    }

                    PrintC("Unknown command. Type \"help\" for a list of commands.", callback);
                    break;
            }

            FireEvent(Events.AFTERCOMMAND, command);
        }

        public static void DisableExtension(string pluginname, bool stop = true)
        {
            if (enabledextensions.Contains(pluginname))
            {
                enabledextensions.Remove(pluginname);
                SaveSettings();
            }

            if (stop)
            {
                foreach (PluginNET extension in extloader.Plugins)
                {
                    if (extension.PluginName == pluginname)
                    {
                        extloader.StopPlugin(Path.Combine(EXTENSIONFOLDER, pluginname + NETPLUGIN_EXTENSION));
                        return;
                    }
                }
            }
        }

        public static void DisablePlugin(string pluginname, bool stop = true)
        {
            if (enabledplugins.Contains(pluginname))
            {
                enabledplugins.Remove(pluginname);
                SaveSettings();

                if (stop)
                    tmch.StopPlugin(Path.Combine(PLUGINFOLDER, pluginname + JSPLUGIN_EXTENSION));
            }

            if (stop)
            {
                foreach (PluginJS plugin in tmch.Plugins)
                {
                    if (plugin.PluginName == pluginname)
                    {
                        tmch.StopPlugin(Path.Combine(PLUGINFOLDER, pluginname + JSPLUGIN_EXTENSION));
                        return;
                    }
                }
            }
        }

        public static void EnableExtension(string pluginname, bool start = true)
        {
            if (!enabledextensions.Contains(pluginname))
            {
                enabledextensions.Add(pluginname);
                SaveSettings();
            }

            if (start)
            {
                foreach (PluginNET extension in extloader.Plugins)
                    if (extension.PluginName == pluginname)
                        return;

                extloader.RunPlugin(Path.Combine(EXTENSIONFOLDER, pluginname + NETPLUGIN_EXTENSION));
            }
        }

        public static void EnablePlugin(string pluginname, bool start = true)
        {
            if (!enabledplugins.Contains(pluginname))
            {
                enabledplugins.Add(pluginname);
                SaveSettings();
            }

            if (start)
            {
                foreach (PluginJS plugin in tmch.Plugins)
                    if (plugin.PluginName == pluginname)
                        return;

                tmch.RunPlugin(Path.Combine(PLUGINFOLDER, pluginname + JSPLUGIN_EXTENSION));
            }
        }

        public static bool ExtensionEnabled(string pluginname)
        {
            return enabledextensions.Contains(pluginname);
        }

        public static bool ExtensionRunning(string pluginname)
        {
            foreach (PluginNET Extension in extloader.Plugins)
                if (Extension.PluginName == pluginname)
                    return true;

            return false;
        }

        public static int GetCommandHandlerCount(string command)
        {
            int retval = 0;

            foreach (KeyValuePair<string, Action<string[]>> existinghandler in plugincommands)
                if (existinghandler.Key == command)
                    retval++;

            return retval;
        }

        public static long GetID(IWebSocketConnection context)
        {
            foreach (Client client in Clients)
                if (GetIP(client.Context) == GetIP(context))
                    return client.ID;

            return -1;
        }

        public static int GetIndex(long id)
        {
            for (int i = 0; i < Clients.Count; i++)
                if (Clients[i].ID == id)
                    return i;

            return -1;
        }

        public static int GetPlayerCount()
        {
            int _playercount = 0;
            foreach (Client client in Clients)
                if (client.Name != null)
                    _playercount++;

            return _playercount;
        }

        public static void InitServer(TextWriter consoleoutput = null)
        {
            if (serverinitialized)
                return;

            if (consoleoutput != null)
            {
                Console.SetOut(consoleoutput);
                ExternalApp = true;
            }
            else if (CATCH_EXCEPTIONS)
                AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            Print("dmultiplayersrv {0}", SERVERVERSION);
            Print("Client version required to join: {0}", MINVERSION);

            Print("Loading settings...");
            LoadSettings();
            CurrentPort = Settings.port;

            bool startserver = true;
            try
            {
                CheckPort(true);

                if (Settings.autopf)
                    UPnPForward();
            }
            catch (dMultiplayerException ex)
            {
                Print(ex.Message);
                startserver = false;
            }

            MyIP();
            CheckUpdate(true);
            updatetimer = new Timer((e) =>
            {
                CheckUpdate(false);
            }, null, 1800000, 1800000); //30 minutes

            md5 = MD5.Create();

            if (startserver)
            {
                Print("Starting server...");
                dServer = new WebSocketServer("ws://127.0.0.1:" + CurrentPort + "/");

                try
                {
                    TextWriter prevout = Console.Out;
                    Console.SetOut(TextWriter.Null);
                    dServer.Start(socket =>
                    {
                        socket.OnOpen = () => OnConnected(socket);
                        socket.OnClose = () => OnDisconnect(socket);
                        socket.OnMessage = message => OnReceive(socket, message);
                    });
                    Console.SetOut(prevout);
                }
                catch (SocketException)
                {
                    PrintError("Ran into a socket error, the port you are trying to run the server on is probably in use. Try \"set port <1-65535>\" to change the port and restart the server software.");
                }
            }

            ServerVisible = Settings.srvbrowser;
            if (Settings.srvbrowser)
                RegisterServer();
            else
                UnregisterServer(false);

            refreshtimer = new Timer((e) =>
            {
                RefreshClients();
            }, null, 2000, 2000);

            AppDomain.CurrentDomain.AssemblyResolve += (sender, e) =>
            {
                string assemblyname = e.Name.Substring(0, e.Name.IndexOf(','));

                if (assemblyname == "Noesis.Javascript")
                {
                    if (IntPtr.Size > 8)
                        Print("Sorry, there were no 128-bit+ processors in 2014.");
                    else
                        return Assembly.LoadFrom(Path.Combine((IntPtr.Size == 8 ? "x64" : "x86"), assemblyname + NETPLUGIN_EXTENSION));
                }

                return null;
            };

            thegreat = new CommandMacro();
            extloader = new PluginLoaderNET();
            tmch = new PluginLoaderJS();

            bool pluginorextensionlistchanged = false;

            if (enabledextensions.Count > 0)
            {
                Print("Loading extensions...");

                List<string> extensionstoremove = new List<string>();
                foreach (string extensionfile in enabledextensions)
                {
                    string filename = Path.Combine(EXTENSIONFOLDER, extensionfile + NETPLUGIN_EXTENSION);
                    if (File.Exists(filename))
                        extloader.RunPlugin(filename);
                    else
                    {
                        Print("Skipping {0}, file not found.", extensionfile);
                        extensionstoremove.Add(extensionfile);
                    }
                }

                foreach (string extension in extensionstoremove)
                {
                    enabledplugins.Remove(extension);
                    pluginorextensionlistchanged = true;
                }
            }

            if (enabledplugins.Count > 0)
            {
                Print("Loading plugins...");

                List<string> pluginstoremove = new List<string>();
                foreach (string pluginfile in enabledplugins)
                {
                    string filename = Path.Combine(PLUGINFOLDER, pluginfile + JSPLUGIN_EXTENSION);
                    if (File.Exists(filename))
                        tmch.RunPlugin(filename);
                    else
                    {
                        Print("Skipping {0}, file not found.", pluginfile);
                        pluginstoremove.Add(pluginfile);
                    }
                }

                foreach (string plugin in pluginstoremove)
                {
                    enabledplugins.Remove(plugin);
                    pluginorextensionlistchanged = true;
                }
            }

            if (pluginorextensionlistchanged)
                SaveSettings(false);

            Print("Running startup macro...");
            thegreat.RunByFile(Path.Combine(MACROFOLDER, STARTUPMACROFILE));

            FireEvent(Events.ONSERVERSTART);

            Print("Ready!");
            serverinitialized = true;

            if (!ExternalApp)
            {
                Console.Write("> ");
                while (true)
                    WaitForInput();
            }
        }

        public static void Kick(long id, string kickmsg = null, string consolemsg = null, IWebSocketConnection callback = null)
        {
            try
            {
                Kick(Clients[GetIndex(id)].Context, kickmsg, consolemsg, callback);
            }
            catch
            {
                Print("Couldn't kick client. (Not connected?)");
            }
        }

        public static bool OnClose(int ctrlType)
        {
            closing = true;

            if (ctrlType != 1337)
            {
                Print("Unregistering server...");
                UnregisterServer(true, false);
                Print("Exiting...");
                dServer.Dispose();
            }

            if (ctrlType > 1)
                UPnPUnforward();

            FireEvent(Events.ONSERVERSTOP);

            return false;
        }

        public static bool PluginEnabled(string pluginname)
        {
            return enabledplugins.Contains(pluginname);
        }

        public static bool PluginRunning(string pluginname)
        {
            foreach (PluginJS plugin in tmch.Plugins)
                if (plugin.PluginName == pluginname)
                    return true;

            return false;
        }

        public static void Print(string text, params object[] args)
        {
            Print(text, false, false, args);
        }

        public static void Print(string text, bool canhide, bool reqextraverbose, params object[] args)
        {
            Print(text, canhide, reqextraverbose, true, args);
        }

        public static void Print(string text, bool canhide, bool reqextraverbose, bool newline, params object[] args)
        {
            if ((canhide && HIDE_DATA) || (reqextraverbose && !EXTRA_VERBOSE)) return;

            string output = string.Format("[" + DateTime.Now.ToString("T") + "] " + text, args).Replace('\n', ' ');

            lock (lockobj)
            {
                bool moveback = false;
                int left = 0;
                if (!ExternalApp)
                {
                    left = Console.CursorLeft;
                    if (left > 0)
                    {
                        Console.CursorLeft = 0;
                        Console.MoveBufferArea(0, Console.CursorTop, Console.BufferWidth, 1, 0, Console.CursorTop + (int)Math.Ceiling((double)(output.Length + 1) / Console.BufferWidth));
                        moveback = true;
                    }
                }

                if (newline)
                    Console.WriteLine(output);
                else
                    Console.Write(output);

                if (!ExternalApp && moveback && newline)
                    Console.CursorLeft = left;
            }
        }

        public static void PrintError(string text, params string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Print(text, args);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void PrintInvalidSyntax(IWebSocketConnection callback = null)
        {
            PrintC("Invalid syntax. Type \"help\" for a list of commands and parameters.", callback);
        }

        public static void PrintLine(IWebSocketConnection context = null)
        {
            if (context != null) return;

            Print(string.Empty, false, false, false);

            if (!ExternalApp)
                while (Console.CursorLeft < Console.BufferWidth - 1)
                    Console.Write("-");
            else
                Console.Write("----------------------------");

            Console.WriteLine();
        }

        public static void PrintWarning(string text, params string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Print(text, args);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void RemoveCommand(string command)
        {
            foreach (KeyValuePair<string, Action<string[]>> existinghandler in plugincommands)
            {
                if (existinghandler.Key == command)
                {
                    plugincommands.Remove(existinghandler);
                    break;
                }
            }
        }

        public static void SaveSettings(bool send = true)
        {
            string bannedipsoutput = string.Empty;
            foreach (string ip in BannedIPs)
                bannedipsoutput += ip + "\r\n";

            if (bannedipsoutput.Length > 0)
                bannedipsoutput = bannedipsoutput.Remove(bannedipsoutput.LastIndexOf("\r\n"));

            using (StreamWriter bannedipswriter = new StreamWriter(Path.Combine(CONFIGFOLDER, BANNEDIPSFILE), false))
            {
                bannedipswriter.Write(bannedipsoutput);
                bannedipswriter.Flush();
            }

            string oppedipsoutput = string.Empty;
            foreach (string ip in OppedIPs)
                oppedipsoutput += ip + "\r\n";

            if (oppedipsoutput.Length > 0)
                oppedipsoutput = oppedipsoutput.Remove(oppedipsoutput.LastIndexOf("\r\n"));

            using (StreamWriter oppedipswriter = new StreamWriter(Path.Combine(CONFIGFOLDER, OPPEDIPSFILE), false))
            {
                oppedipswriter.Write(oppedipsoutput);
                oppedipswriter.Flush();
            }

            string settingsoutput = string.Empty;
            foreach (FieldInfo setting in Settings.GetType().GetFields())
                settingsoutput += setting.Name + ":" + setting.GetValue(Settings) + "\r\n";

            if (settingsoutput.Length > 0)
                settingsoutput = settingsoutput.Remove(settingsoutput.LastIndexOf("\r\n"));

            using (StreamWriter settingswriter = new StreamWriter(Path.Combine(CONFIGFOLDER, SETTINGSFILE), false))
            {
                settingswriter.Write(settingsoutput);
                settingswriter.Flush();
            }

            string privatesettingsoutput = string.Empty;
            foreach (FieldInfo setting in PrivateSettings.GetType().GetFields())
                privatesettingsoutput += setting.Name + ":" + setting.GetValue(PrivateSettings) + "\r\n";

            if (privatesettingsoutput.Length > 0)
                privatesettingsoutput = privatesettingsoutput.Remove(privatesettingsoutput.LastIndexOf("\r\n"));

            using (StreamWriter privatesettingswriter = new StreamWriter(Path.Combine(CONFIGFOLDER, PRIVATESETTINGSFILE), false))
            {
                privatesettingswriter.Write(privatesettingsoutput);
                privatesettingswriter.Flush();
            }

            string customsettingsoutput = string.Empty;
            foreach (CustomSetting setting in CustomSettings)
                customsettingsoutput += setting.SettingName + ":" + setting.Private.ToString() + ":" + setting.Value.ToString() + "\r\n";

            if (customsettingsoutput.Length > 0)
                customsettingsoutput = customsettingsoutput.Remove(customsettingsoutput.LastIndexOf("\r\n"));

            using (StreamWriter customsettingswriter = new StreamWriter(Path.Combine(CONFIGFOLDER, CUSTOMSETTINGSFILE), false))
            {
                customsettingswriter.Write(customsettingsoutput);
                customsettingswriter.Flush();
            }

            string enabledpluginsoutput = string.Empty;
            foreach (string plugin in enabledplugins)
                enabledpluginsoutput += plugin + "\r\n";

            if (enabledpluginsoutput.Length > 0)
                enabledpluginsoutput = enabledpluginsoutput.Remove(enabledpluginsoutput.LastIndexOf("\r\n"));

            using (StreamWriter enabledpluginswriter = new StreamWriter(Path.Combine(CONFIGFOLDER, ENABLEDPLUGINSFILE), false))
            {
                enabledpluginswriter.Write(enabledpluginsoutput);
                enabledpluginswriter.Flush();
            }

            string enabledextensionsoutput = string.Empty;
            foreach (string extension in enabledextensions)
                enabledextensionsoutput += extension + "\r\n";

            if (enabledextensionsoutput.Length > 0)
                enabledextensionsoutput = enabledextensionsoutput.Remove(enabledextensionsoutput.LastIndexOf("\r\n"));

            using (StreamWriter enabledextensionswriter = new StreamWriter(Path.Combine(CONFIGFOLDER, ENABLEDEXTENSIONSFILE), false))
            {
                enabledextensionswriter.Write(enabledextensionsoutput);
                enabledextensionswriter.Flush();
            }

            if (!ServerVisible && Settings.srvbrowser)
                RegisterServer();
            else if (ServerVisible && !Settings.srvbrowser)
                UnregisterServer();

            ServerVisible = Settings.srvbrowser;

            if (Settings.serversidesave)
                foreach (Client client in Clients)
                    client.PrepareSaveFiles();

            if (!autoportforwarded && Settings.autopf)
                UPnPForward();
            else if (autoportforwarded && !Settings.autopf)
                UPnPUnforward();

            SendSettings();
        }

        public static void SendData(long playerid, string dataid, string[] dataarray)
        {
            int index = GetIndex(playerid);
            if (index > -1)
                Clients[index].Context.Send(dataid + SEPERATOR + string.Join(SEPERATOR, dataarray));
        }

        public static void SendSettings(IWebSocketConnection context = null)
        {
            string settingstosend = "SETTINGS" + SEPERATOR + SERVERVERSION + SEPERATOR + Settings.timesync.ToString().ToLower() + SEPERATOR + Settings.syncconsoles.ToString().ToLower() + SEPERATOR + Settings.offlineconsoles.ToString().ToLower() + SEPERATOR + Settings.reviewbattle.ToString().ToLower() + SEPERATOR + Settings.serversidesave.ToString().ToLower() + SEPERATOR + SERVERID;

            if (context != null)
                context.Send(settingstosend);
            else
                SendAll(settingstosend);
        }

        public static void SetCustomSetting(string settingname, object value, bool allowprivate = true)
        {
            string lowercasename = settingname.ToLower();
            bool success = false;

            foreach (CustomSetting customsetting in CustomSettings)
            {
                if (customsetting.SettingName == lowercasename && (allowprivate || !customsetting.Private))
                {
                    customsetting.Value = value;
                    success = true;
                    break;
                }
            }

            if (success)
                SaveSettings();
            else
                throw new dMultiplayerException("No custom setting \"" + settingname + "\" found.");
        }

        public static void SetPrivateSetting(string settingname, object value)
        {
            if (!SetSettingByName(PrivateSettings, settingname, value))
                throw new dMultiplayerException("No private setting \"" + settingname + "\" found.");
        }

        public static void SetSetting(string settingname, object value)
        {
            if (!SetSettingByName(Settings, settingname, value))
                throw new dMultiplayerException("No setting \"" + settingname + "\" found.");
        }

        public static string ShortNumber(long number)
        {
            if (number > 999999) return string.Format("{0:f1}", (float)number / 1000000) + "M";
            else if (number > 999) return string.Format("{0:f1}", (float)number / 1000) + "K";
            else return number.ToString();
        }

        public static string WeekString(long weeknumber)
        {
            long year = weeknumber / 48 + 1;
            long month = weeknumber / 4 % 12 + 1;
            long week = weeknumber % 4 + 1;
            return "Y" + year + " M" + month + " W" + week;
        }

        private static void alwc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null && !e.Cancelled)
                    Print("Added your IP to the request list.");
                else
                    throw new Exception();
            }
            catch
            {
                Print("Failed to add server listing request.");
            }
        }

        private static void CheckPort(bool preparelistenerlist)
        {
            if (preparelistenerlist)
            {
                IPGlobalProperties ipproperties = IPGlobalProperties.GetIPGlobalProperties();
                activeconnections = ipproperties.GetActiveTcpListeners();
            }

            foreach (IPEndPoint endpoint in activeconnections)
            {
                if (endpoint.Port == CurrentPort)
                {
                    int nextport = CurrentPort != ushort.MaxValue ? CurrentPort + 1 : 1;

                    if (nextport == Settings.port)
                        throw new dMultiplayerException("No ports are available! The server is not going to be able to run!");

                    Print("Port {0} is in use, switching to port {1}...", CurrentPort, nextport);
                    CurrentPort = nextport;
                    CheckPort(false);
                    break;
                }
            }
        }

        private static void CheckUpdate(bool first)
        {
            try
            {
                string log = first ? "true" : "false";

                WebClient cuwc = new WebClient();
                cuwc.DownloadStringCompleted += cuwc_DownloadStringCompleted;
                cuwc.DownloadStringAsync(new Uri("http://GDevMP.foxgamingservers.com/sversion.php"));
            }
            catch
            {
            }
        }

        private static long ConvertToLong(string input)
        {
            try
            {
                return Convert.ToInt64(input);
            }
            catch
            {
                throw new dMultiplayerException("Invalid input.");
            }
        }

        private static void cuwc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                string ver = Regex.Replace(e.Result, "[^0-9.]", string.Empty);

                if (ver.Split('.').Length == 4 && new Version(ver).CompareTo(Assembly.GetEntryAssembly().GetName().Version) > 0)
                {
                    Print("A server update is available! ({0}) Type \"site\" to go to the official forum thread for downloads.", ver);
                    FireEvent(Events.ONUPDATEAVAILABLE, ver);
                }
                else if (new Version(ver).CompareTo(Assembly.GetEntryAssembly().GetName().Version) < 0)
                {
                    Print("Version hack detected.");
                    newerversion = true;
                }
                else
                    Print("No updates available.");
            }
            catch
            {
                Print("Failed to check for updates.");
            }
        }

        private static bool FireEvent(string eventname, params object[] args)
        {
            bool handled = false;

            foreach (KeyValuePair<string, Func<object[], bool>> plugincommand in plugineventhandlers)
            {
                if (plugincommand.Key == eventname)
                {
                    bool temphandled = plugincommand.Value(args);
                    if (temphandled)
                        handled = temphandled;
                }
            }

            return handled;
        }

        private static byte[] GenerateCoGameData(string[] basedata)
        {
            return md5.ComputeHash(Encoding.UTF8.GetBytes(string.Join(string.Empty, basedata)));
        }

        private static int GetConnectedPlayersFromIP(string ip)
        {
            int retval = 0;

            foreach (Client client in Clients)
                if (client.Context.ConnectionInfo.ClientIpAddress == ip)
                    retval++;

            return retval;
        }

        private static string GetIP(IWebSocketConnection context)
        {
            return context.ConnectionInfo.ClientIpAddress + ':' + context.ConnectionInfo.ClientPort;
        }

        private static void Kick(IWebSocketConnection context, string kickmsg = "", string consolemsg = "", IWebSocketConnection callback = null)
        {
            try
            {
                long id = GetID(context);
                int index = GetIndex(id);
                string ipport = GetIP(context);

                if (string.IsNullOrEmpty(kickmsg))
                    context.Send("KICK" + SEPERATOR + SERVERID);
                else
                    context.Send("KICK" + SEPERATOR + kickmsg + SEPERATOR + SERVERID);

                Clients.RemoveAt(index);

                if (consolemsg != "poll")
                {
                    if (string.IsNullOrEmpty(consolemsg))
                        PrintC("Kicked client {0} ({1}).", callback, ipport, id);
                    else
                        PrintC("Kicked client {0} ({1}). Reason: {2}", callback, ipport, id, consolemsg);

                    SendOthers(context, "DISCONN" + SEPERATOR + id + SEPERATOR + "kick" + SEPERATOR + SERVERID);
                }
                else
                    PrintC("Polling request returned, kicked client.", callback);

                System.Timers.Timer kicktimer = new System.Timers.Timer(500)
                {
                    AutoReset = false,
                    Enabled = true
                };
                kicktimer.Elapsed += (sender, e) =>
                {
                    context.Close();
                };
                kicktimer.Start();

                FireEvent(Events.ONPLAYERKICK, id, kickmsg, consolemsg);
            }
            catch
            {
                Print("Couldn't kick client. (Not connected?)");
            }
        }

        private static void LoadSettings()
        {
            if (!Directory.Exists(CONFIGFOLDER))
                Directory.CreateDirectory(CONFIGFOLDER);

            if (!Directory.Exists(CLIENTSAVEFOLDER))
                Directory.CreateDirectory(CLIENTSAVEFOLDER);

            if (!Directory.Exists(EXTENSIONFOLDER))
                Directory.CreateDirectory(EXTENSIONFOLDER);

            if (!Directory.Exists(PLUGINFOLDER))
                Directory.CreateDirectory(PLUGINFOLDER);

            if (!Directory.Exists(MACROFOLDER))
                Directory.CreateDirectory(MACROFOLDER);

            if (!Directory.Exists(BACKUPFOLDER))
                Directory.CreateDirectory(BACKUPFOLDER);

            if (File.Exists(OLDBANNEDIPSFILE) && !File.Exists(Path.Combine(CONFIGFOLDER, BANNEDIPSFILE)))
                File.Move(OLDBANNEDIPSFILE, Path.Combine(CONFIGFOLDER, BANNEDIPSFILE));
            if (File.Exists(OLDOPPEDIPSFILE) && !File.Exists(Path.Combine(CONFIGFOLDER, OPPEDIPSFILE)))
                File.Move(OLDOPPEDIPSFILE, Path.Combine(CONFIGFOLDER, OPPEDIPSFILE));
            if (File.Exists(SETTINGSFILE) && !File.Exists(Path.Combine(CONFIGFOLDER, SETTINGSFILE)))
                File.Move(SETTINGSFILE, Path.Combine(CONFIGFOLDER, SETTINGSFILE));
            if (File.Exists(PRIVATESETTINGSFILE) && !File.Exists(Path.Combine(CONFIGFOLDER, PRIVATESETTINGSFILE)))
                File.Move(PRIVATESETTINGSFILE, Path.Combine(CONFIGFOLDER, PRIVATESETTINGSFILE));
            if (File.Exists("Newtonsoft.Json.dll") && !File.Exists(Path.Combine(CONFIGFOLDER, "Newtonsoft.Json.dll")))
                File.Move("Newtonsoft.Json.dll", Path.Combine(BACKUPFOLDER, "Newtonsoft.Json.dll"));

            if (!File.Exists(Path.Combine(MACROFOLDER, STARTUPMACROFILE)))
            {
                using (StreamWriter startupmacrowriter = new StreamWriter(Path.Combine(MACROFOLDER, STARTUPMACROFILE), false))
                {
                    startupmacrowriter.Write("; This file is run on startup, after loading all plugins.\r\n; Any console commands can be used in macro files.\r\n; It is possible to automatically load other macros from here, but do NOT load plugins!");
                    startupmacrowriter.Flush();
                }
            }

            using (FileStream bannedipsstream = new FileStream(Path.Combine(CONFIGFOLDER, BANNEDIPSFILE), FileMode.OpenOrCreate))
            using (StreamReader bannedipsreader = new StreamReader(bannedipsstream))
                BannedIPs = new List<string>(bannedipsreader.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));

            using (FileStream oppedipsstream = new FileStream(Path.Combine(CONFIGFOLDER, OPPEDIPSFILE), FileMode.OpenOrCreate))
            using (StreamReader oppedipsreader = new StreamReader(oppedipsstream))
                OppedIPs = new List<string>(oppedipsreader.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));

            List<string> strsettings = new List<string>();
            using (FileStream settingsstream = new FileStream(Path.Combine(CONFIGFOLDER, SETTINGSFILE), FileMode.OpenOrCreate))
            using (StreamReader settingsreader = new StreamReader(settingsstream))
                strsettings = new List<string>(settingsreader.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));

            foreach (string setting in strsettings)
            {
                string[] splitsetting = setting.Split(':');
                try
                {
                    if (splitsetting.Length < 2) return;

                    if (!SetSettingByName(Settings, splitsetting[0], splitsetting[1], false))
                        Print("Setting \"{0}\" in file ignored, no such property in server.", splitsetting[0]);
                }
                catch
                {
                    Print("Setting \"{0}\" in file ignored, its value is probably invalid.", splitsetting[0]);
                }
            }

            List<string> strprivatesettings = new List<string>();
            using (FileStream privatesettingsstream = new FileStream(Path.Combine(CONFIGFOLDER, PRIVATESETTINGSFILE), FileMode.OpenOrCreate))
            using (StreamReader privatesettingsreader = new StreamReader(privatesettingsstream))
                strprivatesettings = new List<string>(privatesettingsreader.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));

            foreach (string setting in strprivatesettings)
            {
                string[] splitsetting = setting.Split(':');
                try
                {
                    if (splitsetting.Length < 2) throw new dMultiplayerException();

                    if (!SetSettingByName(PrivateSettings, splitsetting[0], splitsetting[1], false))
                        Print("Private setting \"{0}\" in file ignored, no such property in server.", splitsetting[0]);
                }
                catch
                {
                    Print("Private setting \"{0}\" in file ignored, its value is probably invalid.", splitsetting[0]);
                }
            }

            List<string> strcustomsettings = new List<string>();
            using (FileStream customsettingsstream = new FileStream(Path.Combine(CONFIGFOLDER, CUSTOMSETTINGSFILE), FileMode.OpenOrCreate))
            using (StreamReader customsettingsreader = new StreamReader(customsettingsstream))
                strcustomsettings = new List<string>(customsettingsreader.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));

            foreach (string setting in strcustomsettings)
            {
                string[] splitsetting = setting.Split(':');
                try
                {
                    if (splitsetting.Length < 3) throw new dMultiplayerException();
                    CustomSettings.Add(new CustomSetting(splitsetting[0], bool.Parse(splitsetting[1]), splitsetting[2], true));
                }
                catch
                {
                    Print("Custom setting \"{0}\" in file ignored, its value is probably invalid.", splitsetting[0]);
                }
            }

            using (FileStream enabledpluginsstream = new FileStream(Path.Combine(CONFIGFOLDER, ENABLEDPLUGINSFILE), FileMode.OpenOrCreate))
            using (StreamReader enabledpluginsreader = new StreamReader(enabledpluginsstream))
                enabledplugins = new List<string>(enabledpluginsreader.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));

            using (FileStream enabledextensionsstream = new FileStream(Path.Combine(CONFIGFOLDER, ENABLEDEXTENSIONSFILE), FileMode.OpenOrCreate))
            using (StreamReader enabledextensionsreader = new StreamReader(enabledextensionsstream))
                enabledextensions = new List<string>(enabledextensionsreader.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));

            ServerVisible = Settings.srvbrowser;
        }

        private static void Main(string[] args)
        {
            NativeMethods.SetConsoleCtrlHandler(ctrlhandler, true);
            Console.ForegroundColor = ConsoleColor.White;

            InitServer();
        }

        private static void miwc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Result.Split('.').Length == 4)
                    Print("Your public IP is: {0}", e.Result);
                else
                    throw new Exception();
            }
            catch
            {
                Print("Failed to get your public IP.");
            }
        }

        private static void MyIP()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                Print("You aren't connected to a network!");
                return;
            }

            try
            {
                IPHostEntry ips = Dns.GetHostEntry(Dns.GetHostName());
                try
                {
                    foreach (IPAddress ip in ips.AddressList.Reverse())
                    {
                        if (ip.AddressFamily == AddressFamily.InterNetwork && ip.ToString().Split('.')[0] == "192" && ip.ToString().Split('.')[1] == "168")
                        {
                            Print("Your local IP is: {0}", ip.ToString());
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    PrintError("Failed to get your local IP. Error message: {0}", ex.Message);
                }
            }
            catch (SocketException)
            {
                PrintError("Failed to get your local IP. Try running the server as administrator if you aren't already.");
            }

            WebClient miwc = new WebClient();
            miwc.DownloadStringCompleted += miwc_DownloadStringCompleted;
            miwc.DownloadStringAsync(new Uri("http://GDevMP.foxgamingservers.com/myip.php"));
        }

        private static void OnConnected(IWebSocketConnection context)
        {
            FireEvent(Events.BEFORECONNECT, context.ConnectionInfo.ClientIpAddress, context.ConnectionInfo.ClientPort);

            if (GetConnectedPlayersFromIP(context.ConnectionInfo.ClientIpAddress) > MAXIMUM_CONNECTIONS_PER_IP)
            {
                foreach (Client client in Clients)
                {
                    if (context.ConnectionInfo.ClientIpAddress == client.Context.ConnectionInfo.ClientIpAddress)
                    {
                        Kick(context, "Too many connections from this IP", "Too many connections from IP");
                        return;
                    }
                }
            }

            if (!BannedIPs.Contains(context.ConnectionInfo.ClientIpAddress))
            {
                Clients.Add(new Client(context));
                Print("Client {0} ({1}) connected", GetIP(context), ClientCount - 1);
                FireEvent(Events.AFTERCONNECT, context.ConnectionInfo.ClientIpAddress, context.ConnectionInfo.ClientPort, ClientCount - 1);
            }
            else
            {
                Print("Client {0} tried to connect but is banned", true, true, GetIP(context));
                Kick(context, "You are banned from this server", "Banned");
            }
        }

        private static void OnDisconnect(IWebSocketConnection context)
        {
            try
            {
                long id = GetID(context);
                if (GetIndex(id) > -1)
                {
                    FireEvent(Events.BEFOREDISCONNECT, context.ConnectionInfo.ClientIpAddress, context.ConnectionInfo.ClientPort, id);
                    Print("Client {0} ({1}) disconnected", GetIP(context), id);
                    SendOthers(context, "DISCONN" + SEPERATOR + id + SEPERATOR + "left" + SEPERATOR + SERVERID);
                    Print("Informed other clients about disconnection", false, true);
                    Clients[GetIndex(id)].Context.Close();
                    Clients.RemoveAt(GetIndex(id));
                    FireEvent(Events.AFTERDISCONNECT, context.ConnectionInfo.ClientIpAddress, context.ConnectionInfo.ClientPort);
                }
            }
            catch
            {
                return;
            }
        }

        private static void OnReceive(IWebSocketConnection context, string data)
        {
            try
            {
                long id = GetID(context);
                if (id < 0 || id.ToString() == SERVERID) return;

                try
                {
                    Print("Recieved data from client {0} ({1}): \"{2}\"", true, false, GetIP(context), id, data.Replace(SEPERATOR, PRINTSEPERATOR).Replace("\n", string.Empty));
                }
                catch
                {
                    return;
                }
                if (data.StartsWith("Connection") || data.StartsWith("User-Agent")) return;

                Clients[GetIndex(id)].timeoutstopwatch.Restart();

                int maxlength = !data.StartsWith("COGREQ") ? MAXIMUM_INPUT_LENGTH : MAXIMUM_INPUT_LENGTH * 3;

                if (data.Length > maxlength && (!(Settings.serversidesave && data.StartsWith("SAVESTRE"))))
                {
                    if (Clients[GetIndex(id)].floodcount < 5)
                    {
                        Print("Recieved data larger than {0} bytes from player with ID {1}, ignoring", maxlength, id);
                        Clients[GetIndex(id)].floodcount++;
                    }
                    else
                        Kick(context, "Flooding with large data", "Flooding with large data");

                    return;
                }

                data += SEPERATOR + id;
                string[] splitdata = data.Split(new string[] { SEPERATOR }, StringSplitOptions.None);

                if (splitdata[0].Length > 24)
                {
                    Print("Invalid status code \"{0}\"", false, true, splitdata[0]);
                    return;
                }

                if (FireEvent(Events.BEFORERECEIVE, splitdata))
                    return;

                switch (splitdata[0])
                {
                    case "BADLOSER":
                        if (!Settings.cheatmodallowed)
                            Kick(id, "Please disable the cheat mod", "Cheat mod enabled");

                        break;

                    case "COGANN":
                        if (splitdata.Length > 5)
                        {
                            int index = GetIndex(id);
                            int codevindex = GetIndex(ConvertToLong(splitdata[5]));
                            byte[] cogamedata = GenerateCoGameData(new string[] { splitdata[1], splitdata[2], splitdata[3], splitdata[4] });

                            if (Clients[index].cogameanndata != null && Clients[index].cogameanndata.SequenceEqual(cogamedata))
                            {
                                List<string> splitdatatosend = new List<string>(splitdata);
                                splitdatatosend.RemoveAt(6);
                                splitdatatosend.RemoveAt(4);
                                SendAll(string.Join(SEPERATOR, splitdatatosend.ToArray()), id.ToString());
                                Clients[index].cogameanndata = null;
                            }
                            else
                                Clients[codevindex].cogameanndata = cogamedata;
                        }
                        break;

                    case "COGREL":
                        if (splitdata.Length > 5)
                        {
                            int index = GetIndex(id);
                            int codevindex = GetIndex(ConvertToLong(splitdata[5]));
                            byte[] cogamedata = GenerateCoGameData(new string[] { splitdata[1], splitdata[2], splitdata[3], splitdata[4] });

                            if (Clients[index].cogamereldata != null && Clients[index].cogamereldata.SequenceEqual(cogamedata))
                            {
                                List<string> splitdatatosend = new List<string>(splitdata);
                                splitdatatosend.RemoveAt(6);
                                splitdatatosend.RemoveAt(4);
                                SendAll(string.Join(SEPERATOR, splitdatatosend.ToArray()), id.ToString());
                                Clients[index].cogamereldata = null;
                            }
                            else
                                Clients[codevindex].cogamereldata = cogamedata;
                        }
                        break;

                    case "COGREV":
                        if (splitdata.Length > 8)
                        {
                            int index = GetIndex(id);
                            int codevindex = GetIndex(ConvertToLong(splitdata[5]));
                            byte[] cogamedata = GenerateCoGameData(new string[] { splitdata[1], splitdata[2], splitdata[7] });

                            if (Clients[index].cogamerevdata != null && Clients[index].cogamerevdata.SequenceEqual(cogamedata))
                            {
                                List<string> splitdatatosend = new List<string>(splitdata);
                                splitdatatosend.RemoveAt(9);
                                splitdatatosend.RemoveAt(7);
                                SendAll(string.Join(SEPERATOR, splitdatatosend.ToArray()), id.ToString());
                                Clients[index].cogamerevdata = null;
                            }
                            else
                                Clients[codevindex].cogamerevdata = cogamedata;
                        }
                        break;

                    case "COGSOLD":
                        if (splitdata.Length > 5)
                        {
                            int index = GetIndex(id);
                            int codevindex = GetIndex(ConvertToLong(splitdata[5]));
                            byte[] cogamedata = GenerateCoGameData(new string[] { splitdata[1], splitdata[4] });

                            List<string> splitdatatosend = new List<string>(splitdata);
                            if (Clients[index].cogamesolddata != null && Clients[index].cogamesolddata.SequenceEqual(cogamedata))
                            {
                                try
                                {
                                    splitdatatosend[2] = (int.Parse(splitdatatosend[2]) + int.Parse(Clients[codevindex].cogamesoldunits)).ToString();
                                    splitdatatosend[3] = (int.Parse(splitdatatosend[3]) + int.Parse(Clients[codevindex].cogamesoldrev)).ToString();
                                }
                                catch
                                {
                                }

                                splitdatatosend.RemoveAt(6);
                                splitdatatosend.RemoveAt(4);
                                SendAll(string.Join(SEPERATOR, splitdatatosend.ToArray()), id.ToString());
                                Clients[index].cogamesolddata = null;
                            }
                            else
                            {
                                Clients[codevindex].cogamesolddata = cogamedata;
                                Clients[codevindex].cogamesoldunits = splitdatatosend[2];
                                Clients[codevindex].cogamesoldrev = splitdatatosend[3];
                            }
                        }
                        break;

                    case "MSG":
                        if (splitdata.Length == 3)
                        {
                            string opprefix = Clients[GetIndex(id)].Op ? "@" : string.Empty;
                            Print("<{0}> {1}", opprefix + Clients[GetIndex(id)].Name, splitdata[1]);

                            if (Clients[GetIndex(id)].Op && splitdata[1][0] == '/')
                            {
                                string command = data.Replace(SEPERATOR, " ").Substring(5);
                                Command(command.Remove(command.Length - 2), context);
                            }
                            else
                            {
                                string datatosend = data;
                                if (Clients[GetIndex(id)].Op)
                                {
                                    List<string> templist = splitdata.ToList();
                                    templist.Insert(2, "true");
                                    datatosend = string.Join(SEPERATOR, templist.ToArray());
                                }

                                SendOthers(context, datatosend);
                            }
                        }
                        break;

                    case "POLL":
                        PlayerCount--;
                        string description = Settings.description.ToLower() != "disabled" ? Settings.description : "GDTMP Server";
                        context.Send("POLLRES" + SEPERATOR + GetPlayerCount() + SEPERATOR + Settings.cheatmodallowed + SEPERATOR + MINVERSION + SEPERATOR + description + SEPERATOR + SERVERVERSION + SEPERATOR + SERVERID);
                        Kick(id, "poll", "poll");
                        break;

                    case "PRIVMSG":
                        try
                        {
                            if (splitdata.Length == 4)
                            {
                                string datatosend = data;
                                if (Clients[GetIndex(id)].Op)
                                {
                                    List<string> templist = splitdata.ToList();
                                    templist.Insert(3, "true");
                                    datatosend = string.Join(SEPERATOR, templist.ToArray());
                                }
                                Clients[GetIndex(ConvertToLong(splitdata[1]))].Context.Send(datatosend);
                            }
                        }
                        catch
                        {
                        }

                        break;

                    case "REQID":
                        bool oldversion = splitdata.Length < 5 || new Version(splitdata[2]).CompareTo(new Version(MINVERSION)) < 0;
                        bool lol = !oldversion && OFFICIAL && !GetIP(context).StartsWith("127.0.0.") && !GetIP(context).StartsWith("192.168.");

                        if (!oldversion && !lol)
                        {
                            string datatosend = "YOURID" + SEPERATOR + splitdata[1] + SEPERATOR + id + SEPERATOR + SERVERID;
                            context.Send(datatosend);
                            Print("Returned ID", false, true);

                            int index = GetIndex(id);
                            try
                            {
                                Clients[index].Code = long.Parse(splitdata[1]);
                            }
                            catch
                            {
                                Clients[index].Code = -1;
                                Kick(context, "Your client code is invalid", "Invalid client code");
                            }

                            SendSettings(context);

                            string[] mods = splitdata[3].Split(new string[] { SEPERATOR3 }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string mod in mods)
                            {
                                string[] splitmod = mod.Split(new string[] { SEPERATOR2 }, StringSplitOptions.RemoveEmptyEntries);
                                if (splitmod.Length == 2)
                                {
                                    if (Settings.cheatmodallowed || splitmod[0] != "CheatMod-kristof1104" && id > -1)
                                        Clients[index].Mods.Add(new ClientMod(splitmod[0], splitmod[1]));
                                    else
                                        Kick(context, "Please disable the cheat mod", "Cheat mod enabled");
                                }
                            }

                            if (Settings.permaopip && OppedIPs.Contains(context.ConnectionInfo.ClientIpAddress))
                            {
                                Clients[index].Op = true;
                                Print("Opped client {0} ({1}).", GetIP(context), id);
                                Clients[index].Context.Send("OPSTATUS" + SEPERATOR + "true" + SEPERATOR + SERVERID);
                            }

                            if (Settings.motd != null && Settings.motd.ToLower() != "disabled")
                            {
                                string motdtosend = "MSG" + SEPERATOR + Settings.motd + SEPERATOR + SERVERID;
                                context.Send(motdtosend);
                                Print("Sent MOTD", false, true);
                            }

                            foreach (Client client in Clients)
                                client.requestedmods = false;
                        }
                        else if (oldversion)
                        {
                            Print("Client {0} ({1}) is running an old version of GDTMP (disconnecting)", GetIP(context), id);

                            if (!OFFICIAL && !newerversion)
                                context.Send("MSG" + SEPERATOR + "You're running an old version of GDTMP. Please update. | You've been kicked from the server." + SEPERATOR + SERVERID);
                            else
                                context.Send("MSG" + SEPERATOR + "This is a testing server for the unreleased next update. | You've been kicked from the server." + SEPERATOR + SERVERID);

                            Clients[GetIndex(id)].Context.Close();
                            Clients.RemoveAt(GetIndex(id));
                            Print("Sent message and dropped client", false, true);
                        }
                        else if (lol)
                        {
                            context.Send("MSG" + SEPERATOR + "Nice try :) | You've been kicked from the server." + SEPERATOR + SERVERID);
                            Clients[GetIndex(id)].Context.Close();
                            Clients.RemoveAt(GetIndex(id));
                            Print("Sent trololo and dropped client", false, true);
                        }
                        break;

                    case "SAVELOAD":
                        if (Settings.serversidesave && !string.IsNullOrEmpty(Clients[GetIndex(id)].saveinfo) && !string.IsNullOrEmpty(Clients[GetIndex(id)].savedata))
                            context.Send("SAVEDATA" + SEPERATOR + Clients[GetIndex(id)].saveinfo + SEPERATOR + Clients[GetIndex(id)].savedata + SEPERATOR + SERVERID);
                        else if (Settings.serversidesave)
                            context.Send("SAVEFAIL" + SEPERATOR + SERVERID);

                        break;

                    case "SAVESTRE":
                        if (Settings.serversidesave && splitdata.Length > 3 && splitdata[2].StartsWith("{") && splitdata[2].EndsWith("}"))
                        {
                            if (splitdata[2].Length <= 512 * 1024)
                            {
                                int index = GetIndex(id);

                                Clients[index].saveinfo = splitdata[1];
                                Clients[index].savedata = splitdata[2];

                                Clients[index].savechanged = true;
                            }
                            else
                                Kick(context, "Your save data is too large", "Save data too large");
                        }
                        break;

                    default:
                        bool sendothers = true;

                        switch (splitdata[0])
                        {
                            case "COMPANY":
                                if (splitdata.Length > 15)
                                {
                                    Client client = Clients[GetIndex(id)];

                                    splitdata[1] = Regex.Replace(splitdata[1], @"(^\W+)", string.Empty);
                                    splitdata[2] = Regex.Replace(splitdata[2], @"(^\W+)", string.Empty);

                                    client.Name = splitdata[1];
                                    client.Boss = splitdata[2];

                                    float tempfloat;

                                    if (float.TryParse(splitdata[3], out tempfloat))
                                        client.Cash = Convert.ToInt64(tempfloat);
                                    else
                                        client.Cash = -1;

                                    if (float.TryParse(splitdata[4], out tempfloat))
                                        client.Fans = Convert.ToInt64(tempfloat);
                                    else
                                        client.Fans = -1;

                                    if (float.TryParse(splitdata[5], out tempfloat))
                                        client.ResearchPoints = Convert.ToInt64(tempfloat);
                                    else
                                        client.ResearchPoints = -1;

                                    if (float.TryParse(splitdata[6], out tempfloat))
                                        client.CurrentWeek = Convert.ToInt64(tempfloat);
                                    else
                                        client.CurrentWeek = -1;

                                    if (float.TryParse(splitdata[7], out tempfloat))
                                        client.Employees = Convert.ToInt64(tempfloat);
                                    else
                                        client.Employees = -1;

                                    if (float.TryParse(splitdata[8], out tempfloat))
                                        client.PlatformCount = Convert.ToInt64(tempfloat);
                                    else
                                        client.PlatformCount = -1;

                                    if (float.TryParse(splitdata[9], out tempfloat))
                                        client.GameCount = Convert.ToInt64(tempfloat);
                                    else
                                        client.GameCount = -1;

                                    client.FavouriteGenre = splitdata[10];

                                    if (float.TryParse(splitdata[11], out tempfloat))
                                        client.AvgCosts = Convert.ToInt64(tempfloat);
                                    else
                                        client.AvgCosts = -1;

                                    if (float.TryParse(splitdata[12], out tempfloat))
                                        client.AvgIncome = Convert.ToInt64(tempfloat);
                                    else
                                        client.AvgIncome = -1;

                                    if (float.TryParse(splitdata[13], out tempfloat))
                                        client.AvgScore = Convert.ToInt64(tempfloat);
                                    else
                                        client.AvgScore = -1;

                                    if (float.TryParse(splitdata[14], out tempfloat))
                                        client.HighScore = Convert.ToInt64(tempfloat);
                                    else
                                        client.HighScore = -1;

                                    Client newclient = client;
                                    foreach (Client otherclient in Clients)
                                    {
                                        if (newclient.Code == otherclient.Code && newclient.ID != otherclient.ID && otherclient.Context.ConnectionInfo.ClientIpAddress == otherclient.Context.ConnectionInfo.ClientIpAddress && newclient.Name == otherclient.Name && newclient.Boss == otherclient.Boss)
                                        {
                                            Kick(otherclient.ID, "You are a ghost", "Ghosting");
                                            break;
                                        }
                                    }

                                    if (Settings.maxmoney > -1 && client.Cash > Settings.maxmoney)
                                        Kick(client.Context, "You have too much money", "Having too much money");

                                    if (!client.requestedmods)
                                    {
                                        bool therearemods = false;
                                        string modlist = string.Empty;

                                        foreach (Client otherclient in Clients)
                                        {
                                            if (SEND_SELF || GetIP(otherclient.Context) != GetIP(context))
                                            {
                                                modlist += otherclient.ID + SEPERATOR2;
                                                foreach (ClientMod mod in otherclient.Mods)
                                                {
                                                    modlist += mod.Name + SEPERATOR3;
                                                    therearemods = true;
                                                }

                                                if (therearemods)
                                                    modlist = modlist.Substring(0, modlist.Length - 1);

                                                modlist += SEPERATOR4;
                                            }
                                        }

                                        if (therearemods)
                                            modlist = modlist.Substring(0, modlist.Length - 1);

                                        client.requestedmods = true;

                                        if (!string.IsNullOrEmpty(modlist))
                                            context.Send("MODLIST" + SEPERATOR + modlist + SEPERATOR + SERVERID);
                                    }
                                }
                                break;

                            case "ADVSPY":
                            case "ASPYDATA":
                            case "ASPYFAIL":
                            case "COGCONF":
                            case "COGFAIL":
                            case "COGFIN":
                            case "COGREQ":
                            case "COGRES":
                            case "COGSCORE":
                            case "MONEY":
                            case "PRIVMSG":
                            case "SABOTAGE":
                            case "SABODATA":
                            case "SABOFAIL":
                            case "TRADEREQ":
                            case "TRADERES":
                                sendothers = false;

                                if (splitdata.Length > 2 && (!(splitdata[0] == "TRADEREQ" && splitdata.Length < 6 && ConvertToLong(splitdata[4]) > Settings.maxmoney)))
                                    Clients[GetIndex(ConvertToLong(splitdata[1]))].Context.Send(data);

                                break;
                        }

                        if (sendothers)
                            SendOthers(context, data);

                        Print("Appended ID and forwarded data to all other clients", false, true);
                        break;
                }

                FireEvent(Events.AFTERRECEIVE, splitdata);
            }
            catch (Exception ex)
            {
                if (PRINT_STACK_TRACES)
                    PrintError("Error while processing message: {0}", ex.Message);
                else
                    PrintError("Error while processing message: {0}\nStack trace: {1}", ex.Message, ex.StackTrace);
            }
        }

        private static void PrintC(string text, IWebSocketConnection callback, params object[] args)
        {
            if (callback != null)
                callback.Send("MSG" + SEPERATOR + string.Format(text, args) + SEPERATOR + SERVERID);
            else
                Print(text, args);
        }

        private static void PrintConsoleOnlyCommand(IWebSocketConnection callback = null)
        {
            PrintC("This command can only be used in the console.", callback);
        }

        private static void RefreshClients()
        {
            try
            {
                foreach (Client client in Clients)
                {
                    if (Settings.serversidesave && client.savedata != null && client.savechanged)
                    {
                        try
                        {
                            string savepath = Path.Combine(CLIENTSAVEFOLDER, client.Code + ".data");
                            if (File.Exists(savepath))
                            {
                                using (StreamWriter savedatawriter = new StreamWriter(savepath, false))
                                {
                                    savedatawriter.Write(client.savedata);
                                    savedatawriter.Flush();
                                }
                            }

                            string infopath = Path.Combine(CLIENTSAVEFOLDER, client.Code + ".info");
                            if (File.Exists(infopath))
                            {
                                using (StreamWriter saveinfowriter = new StreamWriter(infopath, false))
                                {
                                    saveinfowriter.Write(client.saveinfo);
                                    saveinfowriter.Flush();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (PRINT_STACK_TRACES)
                                PrintError("Error while saving client data: {0}", ex.Message);
                            else
                                PrintError("Error while saving client data: {0}\nStack trace: {1}", ex.Message, ex.StackTrace);
                        }
                    }

                    if (client.timeoutstopwatch.ElapsedMilliseconds / 1000 > Settings.timeout)
                        Kick(client.Context, "Connection timed out, there could be an issue with your client", "Connection timed out");
                    if (client.timeoutstopwatch.ElapsedMilliseconds / 1000 > 30)
                        client.floodcount = 0;
                }
            }
            catch
            {
            }
        }

        private static void RegisterServer()
        {
            WebClient rewc = new WebClient();
            rewc.DownloadStringCompleted += rewc_DownloadStringCompleted;
            rewc.DownloadStringAsync(new Uri("http://gdevmp.foxgamingservers.com/srvbreg.php?port=" + CurrentPort));
        }

        private static void rewc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null && !e.Cancelled)
                {
                    Print("Registered server.");

                    PrivateSettings.serverregisterfail = false;
                    PrivateSettings.serverunregisterfail = false;
                }
                else
                    throw new Exception();
            }
            catch
            {
                PrivateSettings.serverregisterfail = true;
                PrivateSettings.serverunregisterfail = false;

                new Thread(() =>
                {
                    Thread.Sleep(1800000); //30 minutes
                    if (PrivateSettings.serverregisterfail)
                        RegisterServer();
                }).Start();

                Print("Failed to register server. Automatic retry in 30 minutes...");
            }
        }

        private static void rmwc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null && !e.Cancelled)
                    Print("Added your IP to the removal request list.");
                else
                    throw new Exception();
            }
            catch
            {
                Print("Failed to add server listing removal request.");
            }
        }

        private static void SendAll(string data)
        {
            SendAll(data, SERVERID);
        }

        private static void SendAll(string data, string alias)
        {
            foreach (Client client in Clients)
            {
                try
                {
                    if (client.Name != null)
                        client.Context.Send(data + SEPERATOR + alias);
                }
                catch
                {
                }
            }
        }

        private static void SendOthers(IWebSocketConnection context, string data)
        {
            if (data == null) return;
            if (context == null || context.ConnectionInfo == null || GetIP(context) == null)
            {
                SendAll(data);
                return;
            }

            foreach (Client client in Clients)
            {
                try
                {
                    if (SEND_SELF || (GetIP(context) != GetIP(client.Context) && client.Name != null))
                        client.Context.Send(data);
                }
                catch
                {
                }
            }
        }

        private static bool SetSettingByName(object settingclassobject, string settingname, object value, bool save = true)
        {
            string lowercasename = settingname.ToLower();

            FieldInfo field = settingclassobject.GetType().GetField(lowercasename);

            if (field != null)
            {
                field.SetValue(settingclassobject, Convert.ChangeType(value, field.FieldType));

                if (save)
                    SaveSettings();

                return true;
            }

            return false;
        }

        private static void ShowSetHelp(IWebSocketConnection callback)
        {
            PrintC("Syntax: set <variable> <value>", callback);
            PrintC("Available variables:", callback);
            PrintLine(callback);
            PrintC("autopf <true|false> - Experimental setting that allows automatic port forwarding. Requires server restart. Default: false", callback);
            PrintLine(callback);
            PrintC("cheatmodallowed <true|false> - Whether the use of the Cheat Mod is allowed on the server. Default: false", callback);
            PrintLine(callback);
            PrintC("extendedopprivs <true|false> - Whether ops have access to the more dangerous commands (exit, deop, deopip, myip, op, opip, set, site). Default: false", callback);
            PrintLine(callback);
            PrintC("description <message> - The message that is displayed in the server browser. Set to \"disabled\" to disable. Default: GDTMP Server", callback);
            PrintLine(callback);
            PrintC("maxmoney <cash> - Automatically kick a player if their cash exceeds this value. Set to -1 to disable. Default: -1", callback);
            PrintLine(callback);
            PrintC("motd <message> - Message of the day, displayed to clients when they join the server. Set to \"disabled\" to disable. Default: disabled", callback);
            PrintLine(callback);
            PrintC("offlineconsoles <true|false> - Whether consoles developed by other players will be available even when they are offline. Default: false", callback);
            PrintLine(callback);
            PrintC("permaopip <true|false> - Whether operators will be saved and whether everyone connecting from the operator's IP will be opped when they join. Default: false", callback);
            PrintLine(callback);
            PrintC("port <1-65535> - Which port the server runs on. Requires server restart. Default: 3966", callback);
            PrintLine(callback);
            PrintC("reviewbattle <true|false> - Whether players compete with each other's scores. Requires timesync to be set to true. Default: false", callback);
            PrintLine(callback);
            PrintC("serversidesave <true|false> - Whether to force players to store their save file on the server. Could allow malware! Default: false", callback);
            PrintLine(callback);
            PrintC("srvbrowser <true|false> - Whether the server is visible in the server browser. Default: false", callback);
            PrintLine(callback);
            PrintC("syncconsoles <true|false> - Whether consoles developed by other players will be available before the release date is reached. Default: true", callback);
            PrintLine(callback);
            PrintC("timeout <time> - How long, in seconds, a client can wait (not send any data) without being disconnected. Default: 300", callback);
            PrintLine(callback);
            PrintC("timesync <true|false> - Whether time is synchronized between players. Default: false", callback);
            PrintLine(callback);
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string message = ((Exception)e.ExceptionObject).Message;

            if (!closing && !message.EndsWith(") is normally permitted"))
                PrintError("ERROR! Unhandled exception: {0}", ((Exception)e.ExceptionObject).Message);

            while (true)
                Thread.Sleep(int.MaxValue);
        }

        private static void UnregisterServer(bool verbose = true, bool retry = true)
        {
            WebClient urwc = new WebClient();
            try
            {
                urwc.DownloadString(new Uri("http://GDevMP.foxgamingservers.com/srvbunreg.php"));

                PrivateSettings.serverunregisterfail = false;
                PrivateSettings.serverregisterfail = false;

                if (verbose)
                    Print("Unregistered server.");
            }
            catch
            {
                PrivateSettings.serverunregisterfail = true;
                PrivateSettings.serverregisterfail = false;

                if (retry)
                {
                    new Thread(() =>
                    {
                        Thread.Sleep(1800000); //30 minutes
                        if (PrivateSettings.serverunregisterfail)
                            UnregisterServer();
                    }).Start();
                }

                if (verbose && retry)
                    Print("Failed to unregister server. Automatic retry in 30 minutes...");
                else if (verbose)
                    Print("Failed to unregister server.");
            }
        }

        private static void UPnPForward()
        {
            try
            {
                upnpnat = new UPnPNATClass();
                portmappings = upnpnat.StaticPortMappingCollection;

                foreach (IStaticPortMapping mapping in portmappings)
                    if (mapping.ExternalPort == CurrentPort && mapping.Protocol == "TCP")
                        portmappings.Remove(CurrentPort, "TCP");

                string ip = string.Empty;
                foreach (IPAddress cip in Dns.GetHostEntry(Dns.GetHostName()).AddressList.Reverse())
                {
                    if (cip.AddressFamily == AddressFamily.InterNetwork && cip.ToString().Split('.')[0] == "192" && cip.ToString().Split('.')[1] == "168")
                    {
                        ip = cip.ToString();
                        break;
                    }
                }

                portmappings.Add(CurrentPort, "TCP", CurrentPort, ip, true, "GDTMP Server");
                autoportforwarded = true;

                Print("Configured UPnPNAT (automatic port forwarding).");
            }
            catch
            {
                PrintError("UPnPNAT ran into an error, UPnP may be disabled in your router.");
            }
        }

        private static void UPnPUnforward()
        {
            try
            {
                foreach (IStaticPortMapping mapping in portmappings)
                    if (mapping.ExternalPort == CurrentPort && mapping.Protocol == "TCP")
                        portmappings.Remove(CurrentPort, "TCP");

                autoportforwarded = false;
                Print("Disabled UPnP port forwarding.");
            }
            catch
            {
            }
        }

        private static void WaitForInput()
        {
            ConsoleKeyInfo keyinfo = Console.ReadKey();
            switch (keyinfo.Key)
            {
                case ConsoleKey.Enter:
                    try
                    {
                        Console.WriteLine();
                        Command(cmdbuffer, null);
                    }
                    catch (dMultiplayerException ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        PrintError("ERROR: {0}", ex.Message);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        PrintError("ERROR! Unhandled exception: {0}", ex.Message);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    finally
                    {
                        cmdbuffer = string.Empty;
                        Console.Write("> ");
                    }

                    break;

                case ConsoleKey.Backspace:
                    if (cmdbuffer.Length > 0)
                    {
                        cmdbuffer = cmdbuffer.Remove(cmdbuffer.Length - 1);
                        Console.Write(' ');

                        if (Console.CursorLeft > 1)
                            Console.CursorLeft--;
                        else if (!lineclear)
                        {
                            Console.CursorLeft--;
                            lineclear = true;
                        }
                        else
                        {
                            Console.CursorLeft = Console.BufferWidth - 1;
                            Console.CursorTop--;
                            Console.Write(' ');
                            Console.CursorLeft = Console.BufferWidth - 1;
                            Console.CursorTop--;
                            lineclear = false;
                        }
                    }
                    else
                        Console.CursorLeft++;

                    break;

                default:
                    if (keyinfo.KeyChar > 0)
                    {
                        cmdbuffer += keyinfo.KeyChar;
                        lineclear = false;
                    }
                    else
                        Console.CursorLeft--;

                    break;
            }
        }

        public static class Events
        {
            public const string AFTERCOMMAND = "aftercommand";
            public const string AFTERCONNECT = "afterconnect";
            public const string AFTERDISCONNECT = "afterdisconnect";
            public const string AFTERRECEIVE = "afterreceive";
            public const string BEFORECOMMAND = "beforecommand";
            public const string BEFORECONNECT = "beforeconnect";
            public const string BEFOREDISCONNECT = "beforedisconnect";
            public const string BEFORERECEIVE = "beforereceive";
            public const string ONPLAYERBAN = "onplayerban";
            public const string ONPLAYERKICK = "onplayerkick";
            public const string ONPLAYEROP = "onplayerop";
            public const string ONSERVERSTART = "onserverstart";
            public const string ONSERVERSTOP = "onserverstop";
            public const string ONUPDATEAVAILABLE = "onupdateavailable";
        }

        public class Client
        {
            public readonly long ID;
            public long AvgCosts = -1;
            public long AvgIncome = -1;
            public long AvgScore = -1;
            public string Boss = null;
            public long Cash = -1;
            public IWebSocketConnection Context;
            public long CurrentWeek = -1;
            public long Employees = -1;
            public long Fans = -1;
            public string FavouriteGenre = null;
            public long GameCount = -1;
            public long HighScore = -1;
            public List<ClientMod> Mods = new List<ClientMod>();
            public string Name = null;
            public bool Op = false;
            public long PlatformCount = -1;
            public long ResearchPoints = -1;

            internal readonly Stopwatch floodstopwatch = new Stopwatch();
            internal readonly Stopwatch timeoutstopwatch = new Stopwatch();
            internal byte[] cogameanndata = null;
            internal byte[] cogamereldata = null;
            internal byte[] cogamerevdata = null;
            internal byte[] cogamesolddata = null;
            internal string cogamesoldrev = null;
            internal string cogamesoldunits = null;
            internal long floodcount = 0;
            internal bool requestedmods = false;
            internal bool savechanged = false;
            internal string savedata = null;
            internal string saveinfo = null;
            internal bool saveprepared = false;

            private long _code = -1;

            public Client(IWebSocketConnection context)
            {
                ID = ClientCount;
                Context = context;

                ClientCount++;
                PlayerCount++;
            }

            public long Code
            {
                get
                {
                    return _code;
                }
                set
                {
                    _code = value;
                    if (Settings.serversidesave)
                        PrepareSaveFiles();
                }
            }

            internal void PrepareSaveFiles()
            {
                if (Code < 0 || saveprepared) return;

                string savedatapath = Path.Combine(CLIENTSAVEFOLDER, Code + ".data");
                using (FileStream savedatastream = new FileStream(savedatapath, FileMode.OpenOrCreate))
                using (StreamReader savedatareader = new StreamReader(savedatastream))
                    savedata = savedatareader.ReadToEnd();

                string saveinfopath = Path.Combine(CLIENTSAVEFOLDER, Code + ".info");
                using (FileStream saveinfostream = new FileStream(saveinfopath, FileMode.OpenOrCreate))
                using (StreamReader saveinforeader = new StreamReader(saveinfostream))
                    saveinfo = saveinforeader.ReadToEnd();

                saveprepared = true;
            }
        }

        public class ClientMod
        {
            public readonly string ID;
            public readonly string Name;

            public ClientMod(string id, string name)
            {
                ID = id;
                Name = name;
            }
        }

        public class CustomSetting
        {
            public readonly string SettingName;
            public bool Private;
            internal readonly bool addedonload;
            private object _value;

            public CustomSetting(string name, bool isprivate, object defaultvalue) : this(name, isprivate, defaultvalue, false)
            {
            }

            internal CustomSetting(string name, bool isprivate, object defaultvalue, bool addedonload)
            {
                SettingName = name.ToLower();
                Private = isprivate;
                Value = defaultvalue;
                this.addedonload = addedonload;
            }

            public object Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    BinaryFormatter binaryformatter = new BinaryFormatter();
                    MemoryStream memorystream = new MemoryStream();
                    binaryformatter.Serialize(memorystream, value);

                    memorystream.Seek(0, SeekOrigin.Begin);
                    while (memorystream.Position < memorystream.Length)
                        if (Convert.ToByte(memorystream.ReadByte()) == '\r' && memorystream.Position < memorystream.Length && Convert.ToByte(memorystream.ReadByte()) == '\n')
                            throw new dMultiplayerException("No carriage return + line feed allowed in setting value! (Setting: " + SettingName + ")");

                    _value = value;
                }
            }
        }

        public class PrivSettings
        {
            public bool serverregisterfail = false;
            public bool serverunregisterfail = false;
        }

        public class ServerSettings
        {
            public bool autopf = false;
            public bool cheatmodallowed = false;
            public string description = "GDTMP Server";
            public bool extendedopprivs = false;
            public long maxmoney = -1;
            public string motd = "disabled";
            public bool offlineconsoles = false;
            public bool permaopip = false;
            public ushort port = 3966;
            public bool reviewbattle = false;
            public bool serversidesave = false;
            public bool srvbrowser = false;
            public bool syncconsoles = true;
            public int timeout = 300;
            public bool timesync = false;
        }

        public class UpdateEventArgs : EventArgs
        {
            public string version;

            public UpdateEventArgs(string version)
            {
                this.version = version;
            }
        }

        private class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
        }
    }
}