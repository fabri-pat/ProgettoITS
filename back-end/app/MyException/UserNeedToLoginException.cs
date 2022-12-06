namespace app.MyException
{
    public class UserNeedToLoginException : Exception
    {
        public UserNeedToLoginException()
           : base()
        { }

        public UserNeedToLoginException(string msg) : base(msg)
        {
        }
    }
}
