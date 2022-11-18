namespace Gbookv2
{
    public abstract class BC_ReturnData
    {
        public int retCode;
        public string retMsg;
        public object data;        

        public BC_ReturnData (int retCode, string retMsg, object data) {
            this.retCode = retCode;
            this.retMsg = retMsg;
            this.data = data;
        }
    }
}
