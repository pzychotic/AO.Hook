using System;

namespace pzy.AO.Hook
{
    public class HookInterface : IHookInterface
    {
        public override void IsInstalled( Int32 processId )
        {
            Console.WriteLine( "Hook has been installed in target: {0}", processId );
        }

        public override void OnReceiveMessage( Int32 processId, Byte[] message )
        {
            Console.WriteLine( "Received a message from target: {0}", processId );
        }

        public override void ReportException( Int32 processId, Exception e )
        {
            Console.WriteLine( "The target process [{0}] has reported an error: {1}", processId, e );
        }

        public override bool Ping( Int32 processId )
        {
            return true;
        }
    }
}
