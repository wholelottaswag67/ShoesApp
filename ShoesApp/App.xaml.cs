using System.Windows;

namespace ShoesApp;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Db.Init();     // создаём таблицы и заливаем данные
    }
}