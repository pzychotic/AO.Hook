using System;

namespace pzy.AO.Hook
{
    public class HookInterface : IHookInterface
    {
        public override void IsInstalled( Int32 processId )
        {
            Console.WriteLine( "Hook has been installed in target: " + processId.ToString() );
        }

        public override void OnReceiveMessage( Int32 processId, Byte[] message )
        {
            Console.WriteLine( "Received a message!" );
        }

        public override void ReportException( Exception e )
        {
            Console.WriteLine( "The target process has reported an error: " + e.ToString() );
        }

        public override bool Ping( Int32 processId )
        {
            return true;
        }
    }
}
