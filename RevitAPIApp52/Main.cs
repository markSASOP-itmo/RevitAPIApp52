using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPIApp52
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                // 1. Запрос на выбор стен в модели
                IList<Reference> selectedRefs = uidoc.Selection.PickObjects(
                    ObjectType.Element,
                    new WallSelectionFilter(),
                    "Выберите стены для изменения типа"
                );

                if (selectedRefs == null || selectedRefs.Count == 0)
                    return Result.Cancelled;

                List<ElementId> wallIds = selectedRefs.Select(r => r.ElementId).ToList();

                // 2. Создание ViewModel и передача зависимостей
                var viewModel = new MainViewViewModel(doc, wallIds);

                // 3. Создание и открытие окна
                var window = new MainWindow
                {
                    DataContext = viewModel
                };

                // Лямбда для закрытия окна из ViewModel
                viewModel.CloseAction = () => window.Close();

                window.ShowDialog();

                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled; // Пользователь нажал Esc
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
    public class WallSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem) => elem is Wall;

        public bool AllowReference(Reference reference, XYZ position) => true;
    }
}
