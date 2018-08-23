namespace DBRestorer.Ctrl.Domain
{
    public class Message
    {
        private readonly string _Msg;

        public Message(string msg)
        {
            _Msg = msg;
        }

        public override string ToString()
        {
            return _Msg;
        }
    }

    public class ErrorMsg : Message
    {
        public ErrorMsg(string msg) : base(msg)
        {
        }
    }

    public class SucceedMsg : Message
    {
        public SucceedMsg(string msg)
            : base(msg)
        {
        }
    }

    public class CallPostRestorePlugins : Message
    {
        public CallPostRestorePlugins(string msg) : base(msg)
        {

        }
    }
}