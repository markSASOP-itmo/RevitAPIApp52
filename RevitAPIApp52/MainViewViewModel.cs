using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Prism.Commands;
using Prism.Mvvm;
using RevitAPIApp52;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RevitAPIApp52
{
    internal class MainViewViewModel: BindableBase
    {
        private readonly Document _doc;
        private readonly List<ElementId> _wallIds;

        private ObservableCollection<WallType> _wallTypes;
        public ObservableCollection<WallType> WallTypes
        {
            get => _wallTypes;
            set => SetProperty(ref _wallTypes, value);
        }

        private WallType _selectedWallType;
        public WallType SelectedWallType
        {
            get => _selectedWallType;
            set => SetProperty(ref _selectedWallType, value);
        }

        public DelegateCommand ChangeTypeCommand { get; }
        public Action CloseAction { get; set; }

        public MainViewViewModel(Document doc, List<ElementId> wallIds)
        {
            _doc = doc;
            _wallIds = wallIds;

            LoadWallTypes();

            // Команда активна только тогда, когда выбран тип стены
            ChangeTypeCommand = new DelegateCommand(OnChangeType, CanChangeType)
                .ObservesProperty(() => SelectedWallType);
        }

        private void LoadWallTypes()
        {
            var types = new FilteredElementCollector(_doc)
                .OfClass(typeof(WallType))
                .Cast<WallType>()
                .OrderBy(wt => wt.Name)
                .ToList();

            WallTypes = new ObservableCollection<WallType>(types);
        }

        private bool CanChangeType() => SelectedWallType != null;

        private void OnChangeType()
        {
            try
            {
                using (Transaction tx = new Transaction(_doc, "Изменение типа стен"))
                {
                    tx.Start();
                    foreach (var id in _wallIds)
                    {
                        if (_doc.GetElement(id) is Wall wall)
                        {
                            wall.WallType = SelectedWallType;
                        }
                    }
                    tx.Commit();
                }

                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ошибка", $"Не удалось изменить тип стен: {ex.Message}");
            }
        }
    }
}
