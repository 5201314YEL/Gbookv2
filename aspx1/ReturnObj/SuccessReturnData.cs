namespace Gbookv2
{
    public class SuccessReturnData : BC_ReturnData
    {
        public SuccessReturnData (object data) : base(1, "请求成功", data) {}
    }
}
