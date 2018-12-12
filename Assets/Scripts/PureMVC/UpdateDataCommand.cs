using PureMVC.Interfaces;

namespace MyPureMVC
{
    class UpdateDataCommand : PureMVC.Patterns.SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);
            DataProxy proxy = Facade.RetrieveProxy(DataProxy.NAME) as DataProxy;
            proxy.UpdateData();
        }
    }
}
