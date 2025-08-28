using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoK.Module.Controllers
{
    public class ShowPasswordViewController : ViewController
    {
        public ShowPasswordViewController() : base()
        {
            // Target required Views (use the TargetXXX properties) and create their Actions.
            this.TargetObjectType = typeof(DemoK.Module.BusinessObjects.CustomLogonParameters);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
    }
}
