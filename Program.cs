using System;
using System.Collections.Generic;

internal class Program
{
    static void Main(string[] args)
    {
        SuperMarket superMarket = new SuperMarket();

        superMarket.Work();
    }
}

class SuperMarket
{
    private Queue<Client> _clients = new Queue<Client>();
    private Cashier _cashier = new Cashier();

    public void Work()
    {
        AddClient();

        while(_clients.Count > 0)
        {
            Console.WriteLine("Свободная касса");
            Serve(_clients.Peek());
            _clients.Dequeue();
        }
    }

    private void Serve(Client client)
    {
        int costCount = 0;
        bool isSolvent = false;

        while (isSolvent == false)
        {
            costCount = _cashier.Calculate(client.GiveProduct());

            if (client.CanPay(costCount))
            {
                client.Pay(costCount);
                _cashier.GetMoney(costCount);
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

    private void AddClient()
    {
        int clientCount = 7;

        for (int i = 0; i < clientCount; i++)
        {
            _clients.Enqueue(new Client());
        }
    }
}

abstract class Man
{
    protected List<Product> Inventory = new List<Product>();
    public int Money { get; protected set; }

    public Man()
    {
        Money = 0;
        Inventory = new List<Product>();
    }
}

class Cashier : Man
{
    public Cashier() : base() { }

    public int Calculate(IReadOnlyList<Product> inventory)
    {
        int costCount = 0;

        foreach (Product product in inventory)
        {
            costCount += product.Cost;
        }

        return costCount;
    }

    public void GetMoney(int money)
    {
        Money += money;
    }
}

class Client : Man
{
    private static Random _random = new Random();
    public IReadOnlyList<Product> Products => Inventory;

    public Client() : base()
    {
        int minMoney = 100;
        int maxMoney = 901;
        Money = _random.Next(minMoney, maxMoney);
        AddProducts();
    }

    public IReadOnlyList<Product> GiveProduct()
    {
        return Products;
    }

    public void RemoveProduct()
    {
        int minProduct = 0;
        int maxProduct = Inventory.Count;

        Inventory.RemoveAt(_random.Next(minProduct, maxProduct));
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

    private void AddProducts()
    {
        Inventory.Add(new Product("яблоко", "красное, спелое, вкусное", 50));
        Inventory.Add(new Product("мороженое", "Бурёнка, крем-брюле", 40));
        Inventory.Add(new Product("какао", "собрано в Мордоре", 70));
        Inventory.Add(new Product("мяско", "свежее, не замороженное", 170));
        Inventory.Add(new Product("шампанское", "Mondoro Asti", 250));
        Inventory.Add(new Product("хлеб", "черный как тьма, невидим ночью", 300));
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
