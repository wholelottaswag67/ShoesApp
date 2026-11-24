using System.Windows;

namespace ShoesApp;

public partial class LoginWindow : Window
{
    public LoginWindow() => InitializeComponent();

    private void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        lblError.Text = "";
        var dt = Db.Query("SELECT * FROM Users WHERE Login=@l AND Password=@p",
            ("@l", txtLogin.Text.Trim()), ("@p", txtPass.Password.Trim()));
        if (dt.Rows.Count == 0)
        {
            lblError.Text = "Неверный логин или пароль";
            return;
        }
        Db.CurrentUser = dt.Rows[0];
        new ProductsWindow().Show();
        Close();
    }

    private void BtnGuest_Click(object sender, RoutedEventArgs e)
    {
        Db.CurrentUser = null;
        new ProductsWindow().Show();
        Close();
    }
}