using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using EasyHook;

namespace pzy.AO.Hook
{
    public class Hook : EasyHook.IEntryPoint
    {
        IHookInterface  _hookInterface  = null;
        LocalHook       _messageHook    = null;
        Stack<Byte[]>   _messageQueue   = new Stack<Byte[]>();

        public Hook( RemoteHooking.IContext context, String channelName )
        {
            _hookInterface = RemoteHooking.IpcConnectClient<IHookInterface>( channelName );
            _hookInterface.Ping( RemoteHooking.GetCurrentProcessId() );
        }

        public void Run( RemoteHooking.IContext context, String channelName )
        {
            // install hook
            try
            {
                _messageHook = LocalHook.Create(
                    LocalHook.GetProcAddress( "MessageProtocol.dll", "?DataBlockToMessage@@YAPAVMessage_t@@IPAX@Z" ),
                    new DataBlockToMessageHook( DataBlockToMessageHooked ),
                    this );

                // all hooks will start deactivated...
                // the following ensures that all threads are intercepted
                _messageHook.ThreadACL.SetExclusiveACL( new Int32[] { 0 } );
            }
            catch( Exception e )
            {
                // report error to host process
                _hookInterface.ReportException( e );

                return;
            }

            // notify host process about installed hook
            _hookInterface.IsInstalled( RemoteHooking.GetCurrentProcessId() );

            // wait for host process termination
            try
            {
                while( _hookInterface.Ping( RemoteHooking.GetCurrentProcessId() ) )
                {
                    Thread.Sleep( 50 );

                    // transmit monitored messages to host process
                    lock( _messageQueue )
                    {
                        while( _messageQueue.Count > 0 )
                        {
                            _hookInterface.OnReceiveMessage( RemoteHooking.GetCurrentProcessId(), _messageQueue.Pop() );
                        }
                    }
                }
            }
            catch
            {
                // Ping() will raise an exception if host is unreachable
            }
        }

        [DllImport( "MessageProtocol.dll", EntryPoint = "?DataBlockToMessage@@YAPAVMessage_t@@IPAX@Z", CallingConvention = CallingConvention.Cdecl )]
        static extern IntPtr DataBlockToMessage( Int32 size, IntPtr pDataBlock );

        [UnmanagedFunctionPointer( CallingConvention.Cdecl, CharSet = CharSet.Unicode, SetLastError = true )]
        delegate IntPtr DataBlockToMessageHook( Int32 size, IntPtr pDataBlock );

        // injected method for intercepting messages
        static IntPtr DataBlockToMessageHooked( Int32 size, IntPtr pDataBlock )
        {
            try
            {
                Hook hook = (Hook)HookRuntimeInfo.Callback;

                lock( hook._messageQueue )
                {
                    Byte[] message = new Byte[ size ];
                    Marshal.Copy( pDataBlock, message, 0, size );
                    hook._messageQueue.Push( message );
                }
            }
            catch
            {
            }

            // call original method
            return DataBlockToMessage( size, pDataBlock );
        }
    }
}
