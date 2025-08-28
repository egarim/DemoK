using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Authentication;

namespace DemoK.Module.BusinessObjects
{
    public class CustomLogonParameters : AuthenticationStandardLogonParameters
    {

        bool showPassword;
        public bool ShowPassword
        {
            get => showPassword;
            set
            {
                if (showPassword == value)
                {
                    return;
                }

                showPassword = value;
                RaisePropertyChanged(nameof(ShowPassword));
            }
        }
    }
}