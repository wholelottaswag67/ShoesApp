using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace ShoesApp;

public static class Db
{
    public static string ConnStr =
        "Server=.\\SQLEXPRESS;Database=ShoesDB;Trusted_Connection=True;TrustServerCertificate=True;";

    public static DataRow? CurrentUser = null;     // null = гость

    public static DataTable Query(string sql, params (string, object)[] prms)
    {
        using var conn = new SqlConnection(ConnStr);
        conn.Open();
        using var cmd = new SqlCommand(sql, conn);
        foreach (var p in prms) cmd.Parameters.AddWithValue(p.Item1, p.Item2 ?? DBNull.Value);
        using var ad = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        ad.Fill(dt);
        return dt;
    }

    public static void Exec(string sql, params (string, object)[] prms)
    {
        using var conn = new SqlConnection(ConnStr);
        conn.Open();
        using var cmd = new SqlCommand(sql, conn);
        foreach (var p in prms) cmd.Parameters.AddWithValue(p.Item1, p.Item2 ?? DBNull.Value);
        cmd.ExecuteNonQuery();
    }

    public static void Init()
    {
        Exec(@"
IF OBJECT_ID('Users','U') IS NULL
CREATE TABLE Users(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Role NVARCHAR(50),
    FullName NVARCHAR(150),
    Login NVARCHAR(100) UNIQUE,
    Password NVARCHAR(100));

IF OBJECT_ID('PickupPoints','U') IS NULL
CREATE TABLE PickupPoints(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Address NVARCHAR(300));

IF OBJECT_ID('Products','U') IS NULL
CREATE TABLE Products(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Article NVARCHAR(50) UNIQUE,
    Name NVARCHAR(100),
    Unit NVARCHAR(20),
    Price DECIMAL(10,2),
    Supplier NVARCHAR(100),
    Manufacturer NVARCHAR(100),
    Category NVARCHAR(50),
    Discount INT,
    Stock INT,
    Description NVARCHAR(500),
    Photo NVARCHAR(200));

IF OBJECT_ID('Orders','U') IS NULL
CREATE TABLE Orders(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Articles NVARCHAR(300),
    OrderDate DATE,
    DeliveryDate DATE,
    PickupPointId INT,
    ClientName NVARCHAR(150),
    Code INT,
    Status NVARCHAR(50));
");

        if (Convert.ToInt32(Query("SELECT COUNT(*) AS C FROM Users").Rows[0]["C"]) > 0)
            return;

        // --- пользователи ---
        var users = new[]{
            ("Администратор","Никифорова Весения Николаевна","94d5ous@gmail.com","uzWC67"),
            ("Администратор","Сазонов Руслан Германович","uth4iz@mail.com","2L6KZG"),
            ("Администратор","Одинцов Серафим Артёмович","yzls62@outlook.com","JlFRCZ"),
            ("Менеджер","Степанов Михаил Артёмович","1diph5e@tutanota.com","8ntwUp"),
            ("Менеджер","Ворсин Петр Евгеньевич","tjde7c@yahoo.com","YOyhfR"),
            ("Менеджер","Старикова Елена Павловна","wpmrc3do@tutanota.com","RSbvHv"),
            ("Авторизированный клиент","Михайлюк Анна Вячеславовна","5d4zbu@tutanota.com","rwVDh9"),
            ("Авторизированный клиент","Ситдикова Елена Анатольевна","ptec8ym@yahoo.com","LdNyos"),
            ("Авторизированный клиент","Ворсин Петр Евгеньевич","1qz4kw@mail.com","gynQMT"),
            ("Авторизированный клиент","Старикова Елена Павловна","4np6se@mail.com","AtnDjr"),
        };
        foreach (var u in users)
            Exec("INSERT INTO Users(Role,FullName,Login,Password) VALUES(@r,@f,@l,@p)",
                ("@r", u.Item1), ("@f", u.Item2), ("@l", u.Item3), ("@p", u.Item4));

        // --- пункты выдачи ---
        var pts = new[]{
            "420151, г. Лесной, ул. Вишневая, 32","125061, г. Лесной, ул. Подгорная, 8",
            "630370, г. Лесной, ул. Шоссейная, 24","400562, г. Лесной, ул. Зеленая, 32",
            "614510, г. Лесной, ул. Маяковского, 47","410542, г. Лесной, ул. Светлая, 46",
            "620839, г. Лесной, ул. Цветочная, 8","443890, г. Лесной, ул. Коммунистическая, 1",
            "603379, г. Лесной, ул. Спортивная, 46","603721, г. Лесной, ул. Гоголя, 41",
            "410172, г. Лесной, ул. Северная, 13","614611, г. Лесной, ул. Молодежная, 50",
            "454311, г. Лесной, ул. Новая, 19","660007, г. Лесной, ул. Октябрьская, 19",
            "603036, г. Лесной, ул. Садовая, 4","394060, г. Лесной, ул. Фрунзе, 43",
            "410661, г. Лесной, ул. Школьная, 50","625590, г. Лесной, ул. Коммунистическая, 20",
            "625683, г. Лесной, ул. 8 Марта","450983, г. Лесной, ул. Комсомольская, 26",
            "394782, г. Лесной, ул. Чехова, 3","603002, г. Лесной, ул. Дзержинского, 28",
            "450558, г. Лесной, ул. Набережная, 30","344288, г. Лесной, ул. Чехова, 1",
            "614164, г. Лесной, ул. Степная, 30","394242, г. Лесной, ул. Коммунистическая, 43",
            "660540, г. Лесной, ул. Солнечная, 25","125837, г. Лесной, ул. Шоссейная, 40",
            "125703, г. Лесной, ул. Партизанская, 49","625283, г. Лесной, ул. Победы, 46",
            "614753, г. Лесной, ул. Полевая, 35","426030, г. Лесной, ул. Маяковского, 44",
            "450375, г. Лесной ул. Клубная, 44","625560, г. Лесной, ул. Некрасова, 12",
            "630201, г. Лесной, ул. Комсомольская, 17","190949, г. Лесной, ул. Мичурина, 26"
        };
        foreach (var a in pts)
            Exec("INSERT INTO PickupPoints(Address) VALUES(@a)", ("@a", a));

        // --- товары ---
        var products = new object[,] {
            {"А112Т4","Ботинки",4990,"Kari","Kari","Женская обувь",3,6,"Женские Ботинки демисезонные kari","1.jpg"},
            {"F635R4","Ботинки",3244,"Обувь для вас","Marco Tozzi","Женская обувь",2,13,"Ботинки Marco Tozzi женские демисезонные","2.jpg"},
            {"H782T5","Туфли",4499,"Kari","Kari","Мужская обувь",4,5,"Туфли kari мужские классика MYZ21AW-450A","3.jpg"},
            {"G783F5","Ботинки",5900,"Kari","Рос","Мужская обувь",2,8,"Мужские ботинки Рос-Обувь кожаные с натуральным мехом","4.jpg"},
            {"J384T6","Ботинки",3800,"Обувь для вас","Rieker","Мужская обувь",2,16,"B3430/14 Полуботинки мужские Rieker","5.jpg"},
            {"D572U8","Кроссовки",4100,"Обувь для вас","Рос","Мужская обувь",3,6,"129615-4 Кроссовки мужские","6.jpg"},
            {"F572H7","Туфли",2700,"Kari","Marco Tozzi","Женская обувь",2,14,"Туфли Marco Tozzi женские летние","7.jpg"},
            {"D329H3","Полуботинки",1890,"Обувь для вас","Alessio Nesca","Женская обувь",4,4,"Полуботинки Alessio Nesca женские","8.jpg"},
            {"B320R5","Туфли",4300,"Kari","Rieker","Женская обувь",2,6,"Туфли Rieker женские демисезонные","9.jpg"},
            {"G432E4","Туфли",2800,"Kari","Kari","Женская обувь",3,15,"Туфли kari женские TR-YR-413017","10.jpg"},
            {"S213E3","Полуботинки",2156,"Обувь для вас","CROSBY","Мужская обувь",3,6,"407700/01-01 Полуботинки мужские CROSBY",""},
            {"E482R4","Полуботинки",1800,"Kari","Kari","Женская обувь",2,14,"Полуботинки kari женские MYZ20S-149",""},
            {"S634B5","Кеды",5500,"Обувь для вас","CROSBY","Мужская обувь",3,0,"Кеды Caprice мужские демисезонные",""},
            {"K345R4","Полуботинки",2100,"Обувь для вас","CROSBY","Мужская обувь",2,3,"407700/01-02 Полуботинки мужские CROSBY",""},
            {"O754F4","Туфли",5400,"Обувь для вас","Rieker","Женская обувь",4,18,"Туфли женские демисезонные Rieker",""},
            {"G531F4","Ботинки",6600,"Kari","Kari","Женская обувь",12,9,"Ботинки женские зимние ROMER",""},
            {"J542F5","Тапочки",500,"Kari","Kari","Мужская обувь",13,0,"Тапочки мужские Арт.70701-55-67син р.41",""},
            {"B431R5","Ботинки",2700,"Обувь для вас","Rieker","Мужская обувь",2,5,"Мужские кожаные ботинки",""},
            {"P764G4","Туфли",6800,"Kari","CROSBY","Женская обувь",15,15,"Туфли женские, ARGO, размер 38",""},
            {"C436G5","Ботинки",10200,"Kari","Alessio Nesca","Женская обувь",15,9,"Ботинки женские, ARGO, размер 40",""},
            {"F427R5","Ботинки",11800,"Обувь для вас","Rieker","Женская обувь",15,11,"Ботинки на молнии с пряжкой FRAU",""},
            {"N457T5","Полуботинки",4600,"Kari","CROSBY","Женская обувь",3,13,"Полуботинки черные зимние, мех",""},
            {"D364R4","Туфли",12400,"Kari","Kari","Женская обувь",16,5,"Туфли Luiza Belly женские Kate-lazo",""},
            {"S326R5","Тапочки",9900,"Обувь для вас","CROSBY","Мужская обувь",17,15,"Мужские кожаные тапочки Профиль С.Дали",""},
            {"L754R4","Полуботинки",1700,"Kari","Kari","Женская обувь",2,7,"Полуботинки kari женские WB2020SS-26",""},
            {"M542T5","Кроссовки",2800,"Обувь для вас","Rieker","Мужская обувь",18,3,"Кроссовки мужские TOFA",""},
            {"D268G5","Туфли",4399,"Обувь для вас","Rieker","Женская обувь",3,12,"Туфли Rieker женские демисезонные",""},
            {"T324F5","Сапоги",4699,"Kari","CROSBY","Женская обувь",2,5,"Сапоги замша Цвет: синий",""},
            {"K358H6","Тапочки",599,"Kari","Rieker","Мужская обувь",20,2,"Тапочки мужские син р.41",""},
            {"H535R5","Ботинки",2300,"Обувь для вас","Rieker","Женская обувь",2,7,"Женские Ботинки демисезонные",""}
        };
        for (int i = 0; i < products.GetLength(0); i++)
        {
            Exec(@"INSERT INTO Products(Article,Name,Unit,Price,Supplier,Manufacturer,Category,Discount,Stock,Description,Photo)
                   VALUES(@a,@n,'шт.',@p,@s,@m,@c,@d,@st,@des,@ph)",
                ("@a", products[i, 0]), ("@n", products[i, 1]), ("@p", products[i, 2]),
                ("@s", products[i, 3]), ("@m", products[i, 4]), ("@c", products[i, 5]),
                ("@d", products[i, 6]), ("@st", products[i, 7]), ("@des", products[i, 8]),
                ("@ph", products[i, 9]));
        }

        // --- заказы ---
        var orders = new object[,] {
            {"А112Т4, 2, F635R4, 2","2025-02-27","2025-04-20",1,"Степанов Михаил Артёмович",901,"Завершен"},
            {"H782T5, 1, G783F5, 1","2022-09-28","2025-04-21",11,"Никифорова Весения Николаевна",902,"Завершен"},
            {"J384T6, 10, D572U8, 10","2025-03-21","2025-04-22",2,"Сазонов Руслан Германович",903,"Завершен"},
            {"F572H7, 5, D329H3, 4","2025-02-20","2025-04-23",11,"Одинцов Серафим Артёмович",904,"Завершен"},
            {"А112Т4, 2, F635R4, 2","2025-03-17","2025-04-24",2,"Степанов Михаил Артёмович",905,"Завершен"},
            {"H782T5, 1, G783F5, 1","2025-03-01","2025-04-25",15,"Никифорова Весения Николаевна",906,"Завершен"},
            {"J384T6, 10, D572U8, 10","2025-03-21","2025-04-26",3,"Сазонов Руслан Германович",907,"Завершен"},
            {"F572H7, 5, D329H3, 4","2025-03-31","2025-04-27",19,"Одинцов Серафим Артёмович",908,"Новый"},
            {"B320R5, 5, G432E4, 1","2025-04-02","2025-04-28",5,"Степанов Михаил Артёмович",909,"Новый"},
            {"S213E3, 5, E482R4, 5","2025-04-03","2025-04-29",19,"Степанов Михаил Артёмович",910,"Новый"}
        };
        for (int i = 0; i < orders.GetLength(0); i++)
            Exec(@"INSERT INTO Orders(Articles,OrderDate,DeliveryDate,PickupPointId,ClientName,Code,Status)
                   VALUES(@a,@od,@dd,@pp,@cn,@c,@s)",
                ("@a", orders[i, 0]), ("@od", orders[i, 1]), ("@dd", orders[i, 2]),
                ("@pp", orders[i, 3]), ("@cn", orders[i, 4]), ("@c", orders[i, 5]),
                ("@s", orders[i, 6]));
    }
}