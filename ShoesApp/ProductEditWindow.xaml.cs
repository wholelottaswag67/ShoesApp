using System;
using System.Data;
using System.Windows;

namespace ShoesApp;

public partial class ProductEditWindow : Window
{
    private readonly DataRow? _row;
    private readonly int _id;

    public ProductEditWindow(DataRow? row)
    {
        InitializeComponent();
        cmbCategory.ItemsSource = new[] { "Женская обувь", "Мужская обувь" };

        _row = row;
        if (row != null)
        {
            Title = "Редактирование товара";
            _id = Convert.ToInt32(row["Id"]);
            txtArticle.Text = row["Артикул"].ToString();
            txtName.Text = row["Наименование"].ToString();
            txtUnit.Text = row["Ед.изм."].ToString();
            txtPrice.Text = row["Цена"].ToString();
            txtSupplier.Text = row["Поставщик"].ToString();
            txtManufacturer.Text = row["Производитель"].ToString();
            cmbCategory.SelectedItem = row["Категория"].ToString();
            txtDiscount.Text = row["Скидка %"].ToString();
            txtStock.Text = row["На складе"].ToString();
            txtDescription.Text = row["Описание"].ToString();
            txtPhoto.Text = row["Photo"].ToString();
        }
        else
        {
            Title = "Новый товар";
            cmbCategory.SelectedIndex = 0;
        }
    }

    private void BtnOk_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtArticle.Text) || string.IsNullOrWhiteSpace(txtName.Text))
        {
            MessageBox.Show("Заполните артикул и наименование");
            return;
        }
        if (!decimal.TryParse(txtPrice.Text, out var price) ||
            !int.TryParse(txtDiscount.Text, out var disc) ||
            !int.TryParse(txtStock.Text, out var stock))
        {
            MessageBox.Show("Проверьте числовые поля (цена, скидка, кол-во)");
            return;
        }

        try
        {
            if (_row == null)
            {
                Db.Exec(@"INSERT INTO Products
                    (Article,Name,Unit,Price,Supplier,Manufacturer,Category,Discount,Stock,Description,Photo)
                    VALUES(@a,@n,@u,@p,@s,@m,@c,@d,@st,@des,@ph)",
                    ("@a", txtArticle.Text), ("@n", txtName.Text), ("@u", txtUnit.Text),
                    ("@p", price), ("@s", txtSupplier.Text), ("@m", txtManufacturer.Text),
                    ("@c", cmbCategory.SelectedItem?.ToString() ?? ""),
                    ("@d", disc), ("@st", stock),
                    ("@des", txtDescription.Text), ("@ph", txtPhoto.Text));
            }
            else
            {
                Db.Exec(@"UPDATE Products SET
                    Article=@a, Name=@n, Unit=@u, Price=@p, Supplier=@s,
                    Manufacturer=@m, Category=@c, Discount=@d, Stock=@st,
                    Description=@des, Photo=@ph
                    WHERE Id=@id",
                    ("@a", txtArticle.Text), ("@n", txtName.Text), ("@u", txtUnit.Text),
                    ("@p", price), ("@s", txtSupplier.Text), ("@m", txtManufacturer.Text),
                    ("@c", cmbCategory.SelectedItem?.ToString() ?? ""),
                    ("@d", disc), ("@st", stock),
                    ("@des", txtDescription.Text), ("@ph", txtPhoto.Text),
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