using UnityEngine;

namespace MyPureMVC
{
    public class DataProxy : PureMVC.Patterns.Proxy
    {
        public new const string NAME = "DataProxy";

        private PureData data;

        public DataProxy(string proxyName) : base(proxyName, null)
        {

        }

        public override void OnRegister()
        {
            base.OnRegister();
            Debug.Log("Register DataProxy");
        }

        public void UpdateData()
        {
            data.UpdataData();
            SendNotification(MyCommandEvent.DATAUPDATED, data);
        }
    }
}
