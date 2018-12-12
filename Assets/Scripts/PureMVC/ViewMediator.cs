using System.Collections.Generic;
using PureMVC.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace MyPureMVC
{
    public class ViewMediator : PureMVC.Patterns.Mediator
    {
        public new const string NAME = "ViewMediator";

        private Text ViewText;
        private Button DataButtton;

        public override void OnRegister()
        {
            base.OnRegister();
            Debug.Log("Register ViewMediator");
            ViewText = GameObject.Find("ViewText").GetComponent<Text>();
            DataButtton = GameObject.Find("DataButton").GetComponent<Button>();
            DataButtton.onClick.AddListener(() => SendNotification(MyCommandEvent.UPDATEDATA));
        }

        public override void OnRemove()
        {
            base.OnRemove();
            DataButtton.onClick.RemoveAllListeners();
        }

        public override IList<string> ListNotificationInterests()
        {
            IList<string> list = new List<string>();
            list.Add(MyCommandEvent.DATAUPDATED);
            return list;
        }

        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case MyCommandEvent.DATAUPDATED:
                    PureData data = (PureData)notification.Body;
                    ViewText.text = data.Num.ToString();
                    break;
            }
        }
    }
}
