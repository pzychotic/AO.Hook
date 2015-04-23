using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using EasyHook;

namespace pzy.AO.Hook
{
    class Program
    {
        static String _channelName = null;

        static void Main( string[] args )
        {
            try
            {
                // only needed if we want to register our binaries in the GAC
                // since version 3.7 EasyHook can do it without
                //Config.Register( "Message hook for AO.", "AO.Hook.Example.exe", "AO.Hook.dll" );

                RemoteHooking.IpcCreateServer<HookInterface>( ref _channelName, WellKnownObjectMode.SingleCall );

                string hookLib = Path.Combine( Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location ), "AO.Hook.dll" );

                Process[] processes = Process.GetProcessesByName( "AnarchyOnline" );

                foreach( Process process in processes )
                {
                    RemoteHooking.Inject( process.Id, InjectionOptions.DoNotRequireStrongName, hookLib, hookLib, _channelName );
                }

                Console.ReadLine();
            }
            catch( Exception e )
            {
                Console.WriteLine( "Error: Can't connect to target: " + e.ToString() );
                Console.ReadLine();
            }
        }
    }
}
