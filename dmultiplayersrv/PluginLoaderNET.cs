using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Darkly.GDTMP
{
    public abstract class Extension
    {
        public abstract void Start();

        public abstract void Stop();
    }

    internal class PluginLoaderNET
    {
        public List<PluginNET> Plugins = new List<PluginNET>();

        public void RunPlugin(string filename)
        {
            if (Path.GetExtension(filename) != dmultiplayersrv.NETPLUGIN_EXTENSION)
                throw new dMultiplayerException("Extension must be of filetype \"" + dmultiplayersrv.NETPLUGIN_EXTENSION + "\".");

            foreach (PluginNET plugin in Plugins)
                if (plugin.Filename == filename)
                    throw new dMultiplayerException("The specified extension is already running.");

            string pluginname = Path.GetFileNameWithoutExtension(filename);
            Plugins.Add(new PluginNET(filename));

            int index = Plugins.Count - 1;
            if (pluginname.ToLower().StartsWith("async_"))
            {
                new Thread(() =>
                {
                    Plugins[index].Start();
                }).Start();
            }
            else
                Plugins[index].Start();
        }

        public void StopPlugin(string filename)
        {
            foreach (PluginNET plugin in Plugins)
            {
                if (plugin.Filename == filename)
                {
                    plugin.Stop();
                    Plugins.Remove(plugin);
                    dmultiplayersrv.Print("Stopped " + Path.GetFileNameWithoutExtension(filename) + ".");
                    return;
                }
            }

            dmultiplayersrv.Print("The specified extension isn't running.");
        }
    }

    internal class PluginNET : IDisposable
    {
        public readonly string Filename;
        public readonly string PluginName;
        private bool _initialized = false;
        private Assembly assembly;
        private Extension extensionclass;

        public PluginNET(string filename)
        {
            Filename = filename;
            PluginName = Path.GetFileNameWithoutExtension(Filename);
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

        public void Dispose()
        {
            extensionclass = null;
            assembly = null;
        }

        public void Start()
        {
            try
            {
                assembly = Assembly.LoadFrom(Filename);

                int classesfound = 0;
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.BaseType == typeof(Extension))
                    {
                        if (classesfound == 0)
                            extensionclass = (Extension)Activator.CreateInstance(type);

                        classesfound++;
                    }
                }

                if (classesfound > 1)
                    throw new Exception("Found too many (" + classesfound + ") classes with base class Extension. There must only be one.");
                else if (classesfound < 1)
                    throw new Exception("Couldn't find any class with base class Extension.");

                try
                {
                    extensionclass.Start();
                    dmultiplayersrv.Print("Loaded " + PluginName + ".");
                }
                catch (Exception ex)
                {
                    dmultiplayersrv.PrintError("EXTENSION \"" + PluginName + "\" RUN ERROR: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                dmultiplayersrv.PrintError("EXTENSION \"" + PluginName + "\" LOAD ERROR: " + ex.Message);
            }

            Initialized = true;
        }

        public void Stop()
        {
            try
            {
                extensionclass.Stop();
                Dispose();
            }
            catch (Exception ex)
            {
                dmultiplayersrv.PrintError("EXTENSION \"" + PluginName + "\" STOP ERROR: " + ex.Message);
            }
        }
    }
}