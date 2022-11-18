namespace Gbookv2
{
    public class FailedReturnData : BC_ReturnData
    {
        public FailedReturnData (int retCode, string retMsg) : base(retCode, retMsg, "") {}
    }
}
