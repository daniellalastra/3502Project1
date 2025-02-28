using System;
using System.Threading;
class BankAccount
{
    public int Id {get;}
    public int Balance { get; private set; } = 1000; // Starting balance
    private object balanceLock = new object();

    public BankAccount(int id)
    {
        Id = id;
    }

    public void Deposit(int amount)
    {
        lock(balanceLock)
        {
            Balance += amount;
            Console.WriteLine($"Deposited {amount} to Account {Id}. New Balance: {Balance}");
        }  
    }

    public void Withdraw(int amount)
    {
        lock (balanceLock)
        {
            Balance -= amount;
            Console.WriteLine($"Withdrew {amount} from Account {Id}. New balance: {Balance}");
        }
        if (Balance < amount){
        Console.WriteLine($"Insufficient balance in Account {Id} for withdrawal of {amount}.");
        return;
        }
    }

    public void Transfer(BankAccount target, int amount)
    {
        Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} attempting to transer {amount} from Account {Id} to Account {target.Id}");

        if(Id < target.Id)
        {
            lock(balanceLock)
            {
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} locked Account {Id}");
                Thread.Sleep(100);
                lock (target.balanceLock)
                {
                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} locked Account {target.Id}");
                    Withdraw(amount);
                    target.Deposit(amount);
                }
            }
        }
        else
        {
            lock (target.balanceLock)
            {
                Console.WriteLine($"Thread{Thread.CurrentThread.ManagedThreadId} locked Account {target.Id}");
                Thread.Sleep(100);
                lock(balanceLock)
                {
                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} locked Account {Id}");
                    Withdraw(amount);
                    target.Deposit(amount);
                }
            }
        }
         Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} completed transfer from Account {Id} to Account {target.Id}");
        }
        
    }


class Program
{
    static void Main(string[] args)
    {
        // Create two bank accounts
        BankAccount account1 = new BankAccount(1);
        BankAccount account2 = new BankAccount(2);

        // Thread 1: Transfer from account1 to account2
        Thread thread1 = new Thread(() => account1.Transfer(account2, 100));

        // Thread 2: Transfer from account2 to account1
        Thread thread2 = new Thread(() => account2.Transfer(account1, 100));

        // Start both threads
        thread1.Start();
        thread2.Start();

        // Wait for threads to complete
        thread1.Join();
        thread2.Join();

        // Display final balances
        Console.WriteLine($"Account 1 balance: {account1.Balance}");
        Console.WriteLine($"Account 2 balance: {account2.Balance}");
        }
    }

