using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Authentication;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DemoK.Module.BusinessObjects
{
    [DomainComponent()]
    public class CustomLogonParameters : INotifyPropertyChanged, ICustomObjectSerialize, ISupportClearPassword, IAuthenticationStandardLogonParameters, ILogonParameters
    {

        bool showPassword;

        private string userName;

        private string password;

        [NonSerialized]
        [IgnoreDataMember]
        private PropertyChangedEventHandler propertyChanged;

        //
        // Summary:
        //     Specifies the login name.
        //
        // Value:
        //     A string which is the login name.
        [DataMember]
        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                userName = value;
                RaisePropertyChanged("UserName");
            }
        }

        //
        // Summary:
        //     Specifies the password.
        //
        // Value:
        //     A string which is the password.
        [ModelDefault("IsPassword", "True")]
        [DataMember]
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                RaisePropertyChanged("Password");
            }
        }

        //
        // Summary:
        //     Occurs when a DevExpress.ExpressApp.Security.AuthenticationStandardLogonParameters‘s
        //     property value changes.
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                propertyChanged = (PropertyChangedEventHandler)Delegate.Combine(propertyChanged, value);
            }
            remove
            {
                propertyChanged = (PropertyChangedEventHandler)Delegate.Remove(propertyChanged, value);
            }
        }

        //
        // Summary:
        //     Initializes a new instance of the DevExpress.ExpressApp.Security.AuthenticationStandardLogonParameters
        //     class.
        //
        // Parameters:
        //   userName:
        //     A string which is the login name.
        //
        //   password:
        //     A string which is the password.
        [JsonConstructor]
        public CustomLogonParameters(string userName, string password)
        {
            this.userName = userName;
            this.password = password;
        }

        //
        // Summary:
        //     Initializes a new instance of the DevExpress.ExpressApp.Security.AuthenticationStandardLogonParameters
        //     class.
        public CustomLogonParameters()
        {
        }

        //
        // Summary:
        //     Reads the AuthenticationStandardLogonParameters.UserName and AuthenticationStandardLogonParameters.Password
        //     values from the settings storage object.
        //
        // Parameters:
        //   storage:
        //     A SettingsStorage object storing values to be read.
        public void ReadPropertyValues(SettingsStorage storage)
        {
            userName = storage.LoadOption("", "UserName");
        }

        //
        // Summary:
        //     Writes the AuthenticationStandardLogonParameters.UserName and AuthenticationStandardLogonParameters.Password
        //     values to the settings storage object.
        //
        // Parameters:
        //   storage:
        //     A SettingsStorage object storing values.
        public void WritePropertyValues(SettingsStorage storage)
        {
            storage.SaveOption("", "UserName", userName);
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void ClearPassword()
        {
            Password = string.Empty;
        }

        [ImmediatePostData()]
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