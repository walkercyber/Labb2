namespace Labb2

{
    // Klass för produkter
    public class Product
    {
        // egenskaper namn & pris
        public string Name { get; private set; }
        public double Price { get; private set; }

        // Skapar en ny produkt : egenskaper
        public Product(string name, double price)
        {
            Name = name;
            Price = price;
        }

        public override string ToString() //Method som returnerar produktnamn och pris
        {
            return $"{Name} - {Price:C}";
        }
    }

    // Klass för smink som ärver från Product
    public class Makeup : Product
    {
        // Konstruktor som skickar namn och pris till basklassens konstruktor
        public Makeup(string name, double price) : base(name, price) { }
    }

    // Klass för kundens med namn, lösen och kundkorg
    public class Customer
    {
        public string Name { get; private set; }
        private string Password { get; set; }

        // Kundens varukorg med produkter
        public List<Product> Cart { get; private set; }

        // Skapar en ny kund med namn, lösenord och en tom korg
        public Customer(string name, string password)
        {
            Name = name;
            Password = password;
            Cart = new List<Product>();
        }

        // Kontrollerar lösen som kund angett
        public bool VerifyPassword(string password)
        {
            return Password == password;
        }

        // Visar information om kunden och korgen
        public override string ToString()
        {
            if (Cart.Count == 0) // Ifsats som retunerar om korgen är tom eller ej
            {
                return $"Customer: {Name}\nPassword: {Password}\n\nYour cart is empty.";
            }

            // Räknar ut antal och totala pris av alla produkter i kundens korg
            var cartSummary = Cart
                .GroupBy(p => p.Name)
                .Select(group => new
                {
                    ProductName = group.Key,
                    Quantity = group.Count(),
                    TotalPrice = group.Sum(p => p.Price)
                });

            // Lista som visar produkterna i korgen
            string cartContents = string.Join("\n", cartSummary.Select(item =>
                $"{item.Quantity} {item.ProductName}, total: {item.TotalPrice:C}"));

            // Returnerar kundens info och produkterna kunden valt
            return $"Customer: {Name}\nPassword: {Password}\n\nCart:\n{cartContents}\nTotal: {GetCartTotal():C}";
        }

        // Räknar ut totalpriset
        public double GetCartTotal()
        {
            return Cart.Sum(p => p.Price);
        }

        // Kundinformation
        public void PrintCustomerInfo()
        {
            Console.WriteLine("Your account information: ");
            Console.WriteLine($"Your name: {Name}");
        }
    }

    // Klass för kunder och deras inloggning
    public static class Store
    {
        // Lista av registrerade kunder
        public static List<Customer> Customers { get; private set; } = new List<Customer>();

        // Fördefinerade kunder
        static Store()
        {
            Customers.Add(new Customer("Knatte", "123"));
            Customers.Add(new Customer("Fnatte", "321"));
            Customers.Add(new Customer("Tjatte", "213"));
        }

        // Matchar fördefinarede kundernas namn och lösenord med det man matar in
        public static Customer LogIn(string name, string password)
        {
            return Customers.FirstOrDefault(c => c.Name == name && c.VerifyPassword(password));
        }

        // Registrerar ny kund
        public static void Register(string name, string password)
        {
            Customers.Add(new Customer(name, password));
            Console.WriteLine($"Customer {name} registered.");
        }
    }

    //programmet startar här
    internal class Program
    {
        private static void Main()
        {
            // Vilken kund som är inloggad
            Customer loggedInCustomer = null;

            while (true)
            {
                // Visar menyn för att registrera eller logga in om man ej är det
                if (loggedInCustomer == null)
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.WriteLine("Welcome to KBEAUTY!\n");
                    Console.WriteLine("Click anywhere with your mouse to access the menu.\n");
                    Console.ResetColor();

                    Console.WriteLine("1. Register new customer");
                    Console.WriteLine("2. Log In");
                    Console.WriteLine("3. Exit");

                    string choice = Console.ReadLine();

                    if (choice == "1") // Registrera ny kund
                    {
                        Console.Clear();
                        Console.Write("Enter your username: ");
                        string name = Console.ReadLine();
                        Console.Write("Enter your password: ");
                        string password = Console.ReadLine();
                        Store.Register(name, password);
                        Console.Clear();
                    }
                    else if (choice == "2") // Logga in
                    {
                        Console.Clear();
                        Console.Write("Enter your username: ");
                        string name = Console.ReadLine();
                        Console.Write("Enter your password: ");
                        string password = ReadPassword();

                        Console.Clear();
                        loggedInCustomer = Store.LogIn(name, password);  // Kollar om inloggning fungerar

                        if (loggedInCustomer == null)
                        {
                            // Ifsats för att kolla om lösen är fel eller att användaren inte finns
                            if (Store.Customers.Any(c => c.Name == name))
                            {
                                Console.WriteLine("Incorrect password. Try again.");
                            }
                            else // Annars, registrera dig ja/nej
                            {
                                Console.WriteLine("Customer not found. Would you like to register? (y/n)");
                                if (Console.ReadLine()?.ToLower() == "y")
                                {
                                    Store.Register(name, password);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Welcome {loggedInCustomer.Name}!");
                        }
                    }
                    else if (choice == "3")
                    {
                        break;
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Invalid choice. Please try again.");
                    }
                }
                else
                {
                    // Visar meny för att shoppa, se varukorgen eller logga ut för inloggad kund
                    Console.WriteLine("\n1. Shop");
                    Console.WriteLine("2. View Cart");
                    Console.WriteLine("3. Checkout");
                    Console.WriteLine("4. Log Out");

                    string customerChoice = Console.ReadLine();

                    switch (customerChoice)
                    {
                        case "1":
                            Console.Clear();
                            Shop(loggedInCustomer);
                            break;
                        case "2":
                            Console.Clear();
                            Console.WriteLine(loggedInCustomer);
                            break;
                        case "3":
                            Console.Clear();
                            Console.WriteLine($"Your total is: {loggedInCustomer.GetCartTotal():C}");
                            loggedInCustomer.Cart.Clear();
                            Console.WriteLine("Thank you! Welcome back!\n");
                            break;
                        case "4":
                            loggedInCustomer = null;
                            Console.Clear();
                            break;
                        default:
                            Console.Clear();
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
            }
        }


        private static string ReadPassword() // Läser in lösenord dolt
        {
            string password = string.Empty;
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);

            return password;
        }

        //  När kunden handlar och lägger produkter i sin korg
        private static void Shop(Customer customer)
        {
            // Produkterna i affären
            Product product1 = new Makeup("Concealer", 129.95);
            Product product2 = new Makeup("Bronzer", 299.95);
            Product product3 = new Makeup("Foundation", 599.95);

            bool continueShopping = true;

            while (continueShopping)  // Huvudloopen som fortsätter så länge användaren vill shoppa
            {
                Console.Clear();  
                Console.WriteLine("Available products:");  
                Console.WriteLine($"1. {product1}");  
                Console.WriteLine($"2. {product2}");  
                Console.WriteLine($"3. {product3}");  
                Console.WriteLine("Choose a product to add to your cart, or press 0 to return to the menu:"); 

                string productChoice = Console.ReadLine();  


                // Om inmatningen inte är ett nummer returneras false med switch-case
                if (int.TryParse(productChoice, out int choice))
                {
                    // Beroende på användarens val läggs rätt produkt till i korgen
                    switch (choice)
                    {
                        case 1:
                            customer.Cart.Add(product1);  
                            Console.WriteLine("Product added to cart.");  
                            break;
                        case 2:
                            customer.Cart.Add(product2); 
                            Console.WriteLine("Product added to cart.");
                            break;
                        case 3:
                            customer.Cart.Add(product3);
                            Console.WriteLine("Product added to cart.");
                            break;
                        case 0:
                            continueShopping = false; 
                            Console.Clear();  
                            break;
                        default:
                            Console.Clear();  
                            Console.WriteLine("Invalid choice. Please try again."); 
                            break;
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input. Please enter a valid number."); 
                }
                if (continueShopping)
                {
                    Console.WriteLine("\nPress any key to proceed with your shopping"); 
                    Console.ReadKey();  
                }
            }
        }
    }
}