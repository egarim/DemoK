using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Authentication;

namespace DemoK.Module.BusinessObjects
{
    public class CustomLogonParameters : AuthenticationStandardLogonParameters
    {
    
        public bool ShowPassword { get; set; }
    }
}