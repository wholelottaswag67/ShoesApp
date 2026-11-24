using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace ShoesApp;
public partial class ProductsWindow : Window
{
    public ProductsWindow()
    {
        InitializeComponent();
        Title = "Товары — ООО «Обувь»";

        string role = Db.CurrentUser?["Role"]?.ToString() ?? "Гость";
        lblRole.Text = "Роль: " + role;

        bool isAdmin = role == "Администратор";
        bool isManager = role == "Менеджер" || isAdmin;

        txtSearch.Visibility = cmbSort.Visibility = cmbFilter.Visibility =
            isManager ? Visibility.Visible : Visibility.Collapsed;

        btnAdd.Visibility = btnEdit.Visibility = btnDelete.Visibility =
            isAdmin ? Visibility.Visible : Visibility.Collapsed;

        btnOrders.Visibility = isManager ? Visibility.Visible : Visibility.Collapsed;

        cmbSort.ItemsSource = new[] { "Без сортировки", "Цена ↑", "Цена ↓", "Скидка ↑", "Скидка ↓" };
        cmbSort.SelectedIndex = 0;

        cmbFilter.ItemsSource = new[] { "Все категории", "Женская обувь", "Мужская обувь" };
        cmbFilter.SelectedIndex = 0;

        LoadData();
    }

    private void Filter_Changed(object sender, RoutedEventArgs e) => LoadData();

    private void LoadData()
    {
        string sql = @"SELECT Id, Article, Name, Unit, Price, Supplier, Manufacturer, Category, 
                              Discount, Stock, Description, Photo
                       FROM Products WHERE 1=1";
        var prms = new System.Collections.Generic.List<(string, object?)>();

        if (cmbFilter != null && cmbFilter.Visibility == Visibility.Visible && cmbFilter.SelectedIndex > 0)
        {
            sql += " AND Category=@c";
            prms.Add(("@c", cmbFilter.SelectedItem.ToString()));
        }
        if (txtSearch != null && txtSearch.Visibility == Visibility.Visible && !string.IsNullOrWhiteSpace(txtSearch.Text))
        {
            sql += " AND (Name LIKE @q OR Description LIKE @q OR Article LIKE @q)";
            prms.Add(("@q", "%" + txtSearch.Text.Trim() + "%"));
        }
        if (cmbSort != null && cmbSort.Visibility == Visibility.Visible)
        {
            sql += cmbSort.SelectedIndex switch
            {
                1 => " ORDER BY Price ASC",
                2 => " ORDER BY Price DESC",
                3 => " ORDER BY Discount ASC",
                4 => " ORDER BY Discount DESC",
                _ => " ORDER BY Id"
            };
        }
        else sql += " ORDER BY Id";

        var dt = Db.Query(sql, prms.ToArray()!);

       
        if (!dt.Columns.Contains("PhotoPath"))
        {
            dt.Columns.Add("PhotoPath", typeof(string));
        }
        foreach (DataRow row in dt.Rows)
        {
            string fileName = row["Photo"].ToString() ?? "";
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                
                row["PhotoPath"] = $"pack://siteoforigin:,,,/Resources/{fileName}";
            }
        }

        dgProducts.ItemsSource = dt.DefaultView;
    }

    private void dgProducts_LoadingRow(object? sender, DataGridRowEventArgs e)
    {
        if (e.Row.Item is DataRowView rv)
        {
            
            int disc = Convert.ToInt32(rv["Discount"]);
            e.Row.Background = disc > 15 ? new SolidColorBrush(Color.FromRgb(0x2E, 0x8B, 0x57)) : Brushes.White;
        }
    }

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        new ProductEditWindow(null).ShowDialog();
        LoadData();
    }

    private void BtnEdit_Click(object sender, RoutedEventArgs e)
    {
        if (dgProducts.SelectedItem is not DataRowView rv) return;
        new ProductEditWindow(rv.Row).ShowDialog();
        LoadData();
    }

    private void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (dgProducts.SelectedItem is not DataRowView rv) return;
        if (MessageBox.Show("Удалить товар?", "Подтверждение",
                MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

        int used = Convert.ToInt32(Db.Query(
            "SELECT COUNT(*) AS C FROM Orders WHERE Articles LIKE @q",
            ("@q", "%" + rv["Артикул"] + "%")).Rows[0]["C"]);
        if (used > 0)
        {
            MessageBox.Show("Этот товар присутствует в заказах — удалить нельзя.");
            return;
        }
        Db.Exec("DELETE FROM Products WHERE Id=@id", ("@id", rv["Id"]));
        LoadData();
    }

    private void BtnOrders_Click(object sender, RoutedEventArgs e)
    {
        new OrdersWindow().Show();
        Close();
    }

    private void BtnExit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
}