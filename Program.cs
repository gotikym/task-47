using System;
using System.Collections.Generic;

internal class Program
{
    static void Main(string[] args)
    {
        Supermarket supermarket = new Supermarket();

        supermarket.Work();
    }
}

class Supermarket
{
    private List<Product> _products = new List<Product>();
    private Queue<Client> _clients = new Queue<Client>();
    private Cashier _cashier = new Cashier();

    public Supermarket()
    {
        AddProducts();
    }

    public void Work()
    {
        AddClients();

        while (_clients.Count > 0)
        {
            Serve(_clients.Dequeue());
        }
    }

    private void Serve(Client client)
    {
        int costCount = 0;
        bool isSolvent = false;

        FillBasket(client);

        while (isSolvent == false)
        {
            costCount = _cashier.Calculate(client.Products);

            if (client.CanPay(costCount))
            {
                client.Pay(costCount);
                _cashier.TakeMoney(costCount);
                isSolvent = true;
                Console.WriteLine("Клиент оплатил и ушел в закат");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("У клиента не достаточно денег, выкинем пожалуй что-нибудь из его корзины");
                client.RemoveProduct();
                Console.ReadKey();
            }
        }
    }

    private void FillBasket(Client client)
    {
        const string CommandPay = "pay";
        bool isExit = false;

        while (isExit == false)
        {
            Console.WriteLine("Чтобы взять продукт, нажмите любую клавишу, чтобы оплатить напишите - " + CommandPay);
            string userChoice = Console.ReadLine();

            if (userChoice == CommandPay)
            {
                client.TakeProduct(ChooseProduct());
            }
            else
            {
                isExit = true;
            }
        }
    }

    private Product ChooseProduct()
    {
        Console.WriteLine("Выберите продукт: ");
        ShowProducts();
        int productIndex = GetNumber();
        int defoltIndex = 0;

        if(productIndex >= _products.Count || productIndex < 0)
        {
            Console.WriteLine("Нет такого продукта, но думаю вы хотели яблоко.");
            return _products[defoltIndex];
        }

        return _products[productIndex];
    }

    private void ShowProducts()
    {
        int numberProduct = 0;

        foreach (Product product in _products)
        {
            Console.WriteLine(numberProduct + " " + product.Name + ": " + product.Description + " - " + product.Cost + " серебрянных");
            numberProduct++;
        }
    }

    private void AddProducts()
    {
        _products.Add(new Product("яблоко", "красное, спелое, вкусное", 50));
        _products.Add(new Product("мороженое", "Бурёнка, крем-брюле", 40));
        _products.Add(new Product("какао", "собрано в Мордоре", 70));
        _products.Add(new Product("мяско", "свежее, не замороженное", 170));
        _products.Add(new Product("шампанское", "Mondoro Asti", 250));
        _products.Add(new Product("хлеб", "черный как тьма, невидим ночью", 300));
    }

    private void AddClients()
    {
        int clientCount = 5;

        for (int i = 0; i < clientCount; i++)
        {
            _clients.Enqueue(new Client());
        }
    }

    private int GetNumber()
    {
        bool isParse = false;
        int numberForReturn = 0;

        while (isParse == false)
        {
            string userNumber = Console.ReadLine();
            isParse = int.TryParse(userNumber, out numberForReturn);

            if (isParse == false)
            {
                Console.WriteLine("Вы не корректно ввели число.");
            }
        }

        return numberForReturn;
    }
}

class Cashier
{
    public int Money { get; protected set; }

    public Cashier()
    {
        Money = 0;
    }

    public int Calculate(IReadOnlyList<Product> inventory)
    {
        int costCount = 0;

        foreach (Product product in inventory)
        {
            costCount += product.Cost;
        }

        return costCount;
    }

    public void TakeMoney(int money)
    {
        Money += money;
    }
}

class Client
{
    private static Random _random = new Random();
    public IReadOnlyList<Product> Products => _inventory;
    private List<Product> _inventory = new List<Product>();
    public int Money { get; protected set; }

    public Client() : base()
    {
        int minMoney = 100;
        int maxMoney = 901;
        Money = _random.Next(minMoney, maxMoney);
    }

    public void RemoveProduct()
    {
        int minProduct = 0;
        int maxProduct = _inventory.Count;

        _inventory.RemoveAt(_random.Next(minProduct, maxProduct));
    }

    public bool CanPay(int costCount)
    {
        return Money >= costCount;
    }

    public int Pay(int costCount)
    {
        Money -= costCount;
        return costCount;
    }

    public void TakeProduct(Product product)
    {
        _inventory.Add(product);
    }
}

class Product
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int Cost { get; private set; }

    public Product(string name, string description, int cost)
    {
        Name = name;
        Description = description;
        Cost = cost;
    }
}
