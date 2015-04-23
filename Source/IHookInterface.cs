using System;

namespace pzy.AO.Hook
{
    public abstract class IHookInterface : MarshalByRefObject
    {
        public abstract void IsInstalled( Int32 processId );

        public abstract void OnReceiveMessage( Int32 processId, Byte[] message );

        public abstract void ReportException( Int32 processId, Exception e );

        public abstract bool Ping( Int32 processId );
    }
}
