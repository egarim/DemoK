using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Authentication;
using System.Text.Json.Serialization;

namespace DemoK.Module.BusinessObjects
{
    [DomainComponent()]
    public class CustomLogonParameters : AuthenticationStandardLogonParameters
    {

        bool showPassword;

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="CustomLogonParameters"/> class.</para>
        /// </summary>
        /// <param name="userName">A string which is the login name.</param>
        /// <param name="password">A string which is the password.</param>
        [JsonConstructor]
        public CustomLogonParameters(string userName, string password) : base(userName, password)
        {

        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="CustomLogonParameters"/> class.</para>
        /// </summary>
        public CustomLogonParameters()
        {

        }

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