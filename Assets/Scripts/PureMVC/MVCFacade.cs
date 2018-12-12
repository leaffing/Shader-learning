namespace MyPureMVC
{
    public class MVCFacad : PureMVC.Patterns.Facade
    {
        protected override void InitializeFacade()
        {
            base.InitializeFacade();
        }

        protected override void InitializeController()
        {
            base.InitializeController();
            RegisterCommand(MyCommandEvent.STARTUP, typeof(StartUpCommand));
            RegisterCommand(MyCommandEvent.UPDATEDATA, typeof(UpdateDataCommand));
        }

        public void StartUp()
        {
            SendNotification(MyCommandEvent.STARTUP);
        }
    }
}
