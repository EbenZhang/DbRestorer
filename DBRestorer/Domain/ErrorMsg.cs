namespace DBRestorer.Domain
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
}