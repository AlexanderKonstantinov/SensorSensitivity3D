using SensorSensitivity3D.Domain.Entities;
using SensorSensitivity3D.Domain.Models;
using SensorSensitivity3D.Infrastructure;
using SensorSensitivity3D.Services;
using System;
using System.Windows.Input;

namespace SensorSensitivity3D.ViewModels.GeophoneViewModels
{
    public class GeophoneViewModel : BaseViewModel
    {
        /// <summary>
        /// Вызывается при возвращении к панели с геофонами
        /// </summary>
        public event Action<Geophone> Back;

        public bool IsGeophonePanel { get; set; }

        public Geophone OriginalGeophone { get; private set; }
        public Geophone EditedGeophone { get; set; }

        public string PanelTitle { get; }


        public GeophoneViewModel() { }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="originalGeophone">Редактируемый геофон</param>
        public GeophoneViewModel(Geophone originalGeophone, string panelTitle)
        {
            PanelTitle = panelTitle;

            OriginalGeophone = originalGeophone;

            EditedGeophone = OriginalGeophone is null
                    ? new Geophone()
                    : GeophoneConversionService.CopyConstructor(OriginalGeophone);

            IsGeophonePanel = true;
        }


        private RelayCommand _backCommand;
        public ICommand BackCommand
            => _backCommand
               ?? (_backCommand = new RelayCommand(ExecuteBackCommand, null));

        private void ExecuteBackCommand(object obj)
        {
            Back?.Invoke(null);

            IsGeophonePanel = false;
        }


        private RelayCommand _saveGeophoneCommand;
        public ICommand SaveGeophoneCommand
            => _saveGeophoneCommand
               ?? (_saveGeophoneCommand = new RelayCommand(ExecuteSaveGeophoneCommand, CanExecuteSaveGeophoneCommand));

        private void ExecuteSaveGeophoneCommand(object obj)
        {
            OriginalGeophone = GeophoneConversionService.Copy(OriginalGeophone, EditedGeophone);

            Back?.Invoke(OriginalGeophone);

            IsGeophonePanel = false;
        }

        private bool CanExecuteSaveGeophoneCommand(object obj) 
            => OriginalGeophone != null 
            && OriginalGeophone.Name != EditedGeophone.Name 
            && !OriginalGeophone.Equals(EditedGeophone);


        private RelayCommand _addGeophoneCommand;
        public ICommand AddGeophoneCommand
            => _addGeophoneCommand
               ?? (_addGeophoneCommand = new RelayCommand(ExecuteAddGeophoneCommand, CanExecuteAddGeophoneCommand));

        private void ExecuteAddGeophoneCommand(object obj)
        {
            Back?.Invoke(EditedGeophone);

            OriginalGeophone = EditedGeophone;

            EditedGeophone = GeophoneConversionService.CopyConstructor(OriginalGeophone);

            IsGeophonePanel = (bool) obj;
        }

        private bool CanExecuteAddGeophoneCommand(object obj)
            => OriginalGeophone is null || !OriginalGeophone.Equals(EditedGeophone);


        protected override void OnDispose()
        {
            throw new NotImplementedException();
        }
    }
}
