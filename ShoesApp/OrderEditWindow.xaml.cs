using System;
using System.Data;
using System.Windows;

namespace ShoesApp;

public partial class OrderEditWindow : Window
{
    private readonly DataRow? _row;
    private readonly int _id;

    public OrderEditWindow(DataRow? row)
    {
        InitializeComponent();
        cmbStatus.ItemsSource = new[] { "Новый", "Завершен" };

        
        var pts = Db.Query("SELECT Id, Address FROM PickupPoints ORDER BY Id");
        cmbPickup.ItemsSource = pts.DefaultView;
        cmbPickup.DisplayMemberPath = "Address";
        cmbPickup.SelectedValuePath = "Id";

        _row = row;
        if (row != null)
        {
            Title = "Редактирование заказа";
            _id = Convert.ToInt32(row["Id"]);
            txtArticles.Text = row["Артикул"].ToString();
            dpOrderDate.SelectedDate = Convert.ToDateTime(row["Дата заказа"]);
            dpDeliveryDate.SelectedDate = Convert.ToDateTime(row["Дата доставки"]);
            cmbPickup.SelectedValue = Convert.ToInt32(row["Пункт выдачи"]);
            txtClient.Text = row["Клиент"].ToString();
            txtCode.Text = row["Код"].ToString();
            cmbStatus.SelectedItem = row["Статус"].ToString();
        }
        else
        {
            Title = "Новый заказ";
            cmbStatus.SelectedIndex = 0;
            dpOrderDate.SelectedDate = DateTime.Today;
            dpDeliveryDate.SelectedDate = DateTime.Today.AddDays(3);
        }
    }

    private void BtnOk_Click(object sender, RoutedEventArgs e)
    {
        if (cmbPickup.SelectedValue == null || cmbStatus.SelectedItem == null)
        {
            MessageBox.Show("Заполните пункт выдачи и статус");
            return;
        }
        if (!int.TryParse(txtCode.Text, out var code))
        {
            MessageBox.Show("Код получения должен быть числом");
            return;
        }

        try
        {
            if (_row == null)
            {
                Db.Exec(@"INSERT INTO Orders
                    (Articles,OrderDate,DeliveryDate,PickupPointId,ClientName,Code,Status)
                    VALUES(@a,@od,@dd,@pp,@cn,@c,@s)",
                    ("@a", txtArticles.Text),
                    ("@od", dpOrderDate.SelectedDate ?? DateTime.Today),
                    ("@dd", dpDeliveryDate.SelectedDate ?? DateTime.Today),
                    ("@pp", cmbPickup.SelectedValue),
                    ("@cn", txtClient.Text),
                    ("@c", code),
                    ("@s", cmbStatus.SelectedItem.ToString())!);
            }
            else
            {
                Db.Exec(@"UPDATE Orders SET
                    Articles=@a, OrderDate=@od, DeliveryDate=@dd,
                    PickupPointId=@pp, ClientName=@cn, Code=@c, Status=@s
                    WHERE Id=@id",
                    ("@a", txtArticles.Text),
                    ("@od", dpOrderDate.SelectedDate ?? DateTime.Today),
                    ("@dd", dpDeliveryDate.SelectedDate ?? DateTime.Today),
                    ("@pp", cmbPickup.SelectedValue),
                    ("@cn", txtClient.Text),
                    ("@c", code),
                    ("@s", cmbStatus.SelectedItem.ToString()!),
                    ("@id", _id));
            }
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ошибка сохранения: " + ex.Message);
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e) => Close();
}