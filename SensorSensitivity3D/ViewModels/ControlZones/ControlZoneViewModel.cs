using System;
using System.Windows.Input;
using SensorSensitivity3D.Domain.Enums;
using SensorSensitivity3D.Domain.Models;
using SensorSensitivity3D.Infrastructure;
using SensorSensitivity3D.ViewModels.Base;
using static SensorSensitivity3D.Services.ModelInteractionService;

namespace SensorSensitivity3D.ViewModels.ControlZones
{
    public class ControlZoneViewModel : BaseViewModel
    {
        /// <summary>
        /// Вызывается при возвращении к панели с зонами контроля
        /// Сообщает подписчику, стоит ли оставить зону контроля коллекции
        /// </summary>
        public event Action<ModelOperation, ControlZoneModel> Back;
        public bool IsControlZonePanel { get; set; }
        public string PanelTitle { get; private set; }
        public bool IsEditedMode { get; set; }

        public ControlZoneModel EditedControlZone { get; set; }
        
        public ControlZoneViewModel() { }

        /// <summary>
        /// Активировать панель редактирования или добавления новой зоны контроля,
        /// сделав её видимой и инициализировав привязываемые объекты
        /// </summary>
        /// <param name="editedControlZone">Зона контроля для редактирования</param>
        public void ActivateControlZoneViewModel(ControlZoneModel editedControlZone)
        {
            IsEditedMode = editedControlZone != null;

            if (IsEditedMode)
            {
                EditedControlZone = editedControlZone;
                PanelTitle = "Редактировать зону контроля";
            }
            else
            {
                EditedControlZone = new ControlZoneModel();
                PanelTitle = "Добавить зону контроля";
            }

            IsControlZonePanel = true;
        }


        private RelayCommand _changeControlZoneCommand;
        public ICommand ChangeControlZoneCommand
            => _changeControlZoneCommand ??= new RelayCommand(ExecuteChangeControlZoneCommand);

        private void ExecuteChangeControlZoneCommand(object o)
        {
            if (!IsControlZonePanel) return;

            RemoveEntities(new [] {EditedControlZone.Entity});

            EditedControlZone.InitEntity();

            AddEntities(new[] { EditedControlZone.Entity });
        }


        private RelayCommand _backCommand;
        public ICommand BackCommand
            => _backCommand ??= new RelayCommand(ExecuteBackCommand, null);

        private void ExecuteBackCommand(object obj)
        {
            IsControlZonePanel = false;

            RemoveEntities(new[] { EditedControlZone.Entity });

            if (IsEditedMode)
            {
                EditedControlZone.ResetControlZoneSettings();
                AddEntities(new[] { EditedControlZone.Entity });
            }

            Back?.Invoke(ModelOperation.None, null);
        }


        private RelayCommand _saveControlZoneCommand;
        public ICommand SaveControlZoneCommand
            => _saveControlZoneCommand ??= new RelayCommand(ExecuteSaveControlZoneCommand, CanExecuteSaveControlZoneCommand);

        private void ExecuteSaveControlZoneCommand(object obj)
        {
            IsControlZonePanel = false;

            Back?.Invoke(ModelOperation.Edit, EditedControlZone);
        }

        private bool CanExecuteSaveControlZoneCommand(object obj)
            => IsEditedMode
            && (EditedControlZone?.IsChanged ?? false)
            && !string.IsNullOrEmpty(EditedControlZone.Name);


        private RelayCommand _addControlZoneCommand;
        public ICommand AddControlZoneCommand
            => _addControlZoneCommand ??= new RelayCommand(ExecuteAddControlZoneCommand, CanExecuteAddControlZoneCommand);

        private void ExecuteAddControlZoneCommand(object obj)
        {
            var needToContinue = (bool)obj;

            IsControlZonePanel = false;

            if (IsEditedMode)
            {
                var newControlZone = new ControlZoneModel(EditedControlZone);

                EditedControlZone.ResetControlZoneSettings();
                AddEntities(new []{EditedControlZone.Entity});

                EditedControlZone = newControlZone;
            }

            Back?.Invoke(needToContinue
                ? ModelOperation.AddAndContinueAdding
                : ModelOperation.Add,
                EditedControlZone);
        }

        private bool CanExecuteAddControlZoneCommand(object obj)
            => EditedControlZone != null 
               && (!IsEditedMode || (EditedControlZone.IsChanged))
               && EditedControlZone.GeometricZone.Volume > 0;


        private RelayCommand _resetControlZoneCommand;
        public ICommand ResetControlZoneCommand
            => _resetControlZoneCommand ??= new RelayCommand(ExecuteResetControlZoneCommand, CanExecuteResetControlZoneCommand);

        private void ExecuteResetControlZoneCommand(object obj)
        {
            RemoveEntities(new []{EditedControlZone.Entity});

            EditedControlZone.ResetControlZoneSettings();

            AddEntities(new []{EditedControlZone.Entity});
        }

        private bool CanExecuteResetControlZoneCommand(object obj)
            => IsEditedMode && (EditedControlZone?.IsChanged ?? false);


        protected override void OnDispose()
        {
            throw new NotImplementedException();
        }
    }
}
