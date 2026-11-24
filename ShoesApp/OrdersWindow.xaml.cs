using System;
using System.Data;
using System.Windows;

namespace ShoesApp;

public partial class OrdersWindow : Window
{
    public OrdersWindow()
    {
        InitializeComponent();

        string role = Db.CurrentUser?["Role"]?.ToString() ?? "Гость";
        bool isAdmin = role == "Администратор";

        btnAdd.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
        btnEdit.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
        btnDelete.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;

        LoadData();
    }

    private void LoadData()
    {
        var dt = Db.Query(@"SELECT o.Id, o.Articles AS 'Артикул',
                                   o.OrderDate AS 'Дата заказа',
                                   o.DeliveryDate AS 'Дата доставки',
                                   o.PickupPointId AS 'Пункт выдачи',
                                   o.ClientName AS 'Клиент',
                                   o.Code AS 'Код',
                                   o.Status AS 'Статус'
                            FROM Orders o ORDER BY o.Id");
        dgOrders.ItemsSource = dt.DefaultView;

        if (dgOrders.Columns.Count > 0)
        {
            var col = dgOrders.Columns.FirstOrDefault(c => (string)c.Header == "Id");
            if (col != null) col.Visibility = Visibility.Collapsed;
        }
    }

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        new OrderEditWindow(null).ShowDialog();
        LoadData();
    }

    private void BtnEdit_Click(object sender, RoutedEventArgs e)
    {
        if (dgOrders.SelectedItem is not DataRowView rv) return;
        new OrderEditWindow(rv.Row).ShowDialog();
        LoadData();
    }

    private void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (dgOrders.SelectedItem is not DataRowView rv) return;
        if (MessageBox.Show("Удалить заказ?", "Подтверждение",
                MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
        Db.Exec("DELETE FROM Orders WHERE Id=@id", ("@id", rv["Id"]));
        LoadData();
    }

    private void BtnBack_Click(object sender, RoutedEventArgs e)
    {
        new ProductsWindow().Show();
        Close();
    }
}