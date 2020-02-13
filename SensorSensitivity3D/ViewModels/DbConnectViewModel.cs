using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using SensorSensitivity3D.Infrastructure;
using SensorSensitivity3D.Services;
using SensorSensitivity3D.ViewModels.Base;

namespace SensorSensitivity3D.ViewModels
{
    public class DbConnectViewModel : BaseViewModel
    {
        private GCSDbReadService _gcsDbReadService;

        public string ServerName { get; set; }
        public string  DbName { get; set; }
        public string  UserName { get; set; }
        public string  Password { get; set; }

        public DbConnectViewModel()
        {
            ServerName = "(local)";
            DbName = "Apatit";
            UserName = "sa";
            Password = "2006";

            _gcsDbReadService = new GCSDbReadService();
        }


        private RelayCommand _connectDBCommand;
        public ICommand ConnectDBCommand
            => _connectDBCommand ??= new RelayCommand(ExecuteConnectDBCommand, CanExecuteConnectDBCommand);

        private void ExecuteConnectDBCommand(object obj)
        {
            string connectionString = $@"Data Source={ServerName};Initial Catalog={DbName};User ID={UserName};Password={Password}";

            var geophones = _gcsDbReadService.GetGeophonesFromGCSDb(connectionString, out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(
                    "Подключение к базе данных не выполнено. Проверьте достоверность введенных данных.",
                    "Ошибка подключения",
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                return;
            }


        }

        private bool CanExecuteConnectDBCommand(object obj)
            => true;

        protected override void OnDispose()
        {
            throw new NotImplementedException();
        }
    }
}
