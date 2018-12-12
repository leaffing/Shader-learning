using PureMVC.Interfaces;

namespace MyPureMVC
{
    public class StartUpCommand : PureMVC.Patterns.SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);
            Facade.RegisterProxy(new DataProxy(DataProxy.NAME));
            Facade.RegisterMediator(new ViewMediator());
        }
    }

    public class MyCommandEvent
    {
        public const string STARTUP = "StartUp";

        public const string UPDATEDATA = "UpdateData";

        public const string DATAUPDATED = "DataUpdated";
    }
}
