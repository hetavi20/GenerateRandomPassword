using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GeneratePassword
{
    public class Program
    {
        const int NUMBER_OF_RECORDES = 30000000;
        const int NUMBER_OF_TASK = 7;
        const int SIZE = (NUMBER_OF_RECORDES / NUMBER_OF_TASK);
        const int EXTRA = (NUMBER_OF_RECORDES % NUMBER_OF_TASK);
        const string CONNECTION_STRING = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=assignment1;Integrated Security=True";
        static Random random = new Random();
        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            CreateTask();
            watch.Stop();
            Console.WriteLine("Total time:" + watch.Elapsed.ToString());
            Console.ReadLine();
        }

        private static async void CreateTask()
        {

            List<Task> tasks = new List<Task>();

            for (int i = 0; i < NUMBER_OF_TASK; i++)
            {
                Task t = InsertRecord(SIZE);
                tasks.Add(t);
            }
            Task task = Task.WhenAll(tasks);
            if (EXTRA > 0)
            {
                Task.Run(() => InsertRecord(EXTRA));
            }
            try
            {
                task.Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static async Task InsertRecord(int SIZE)
        {
            SqlConnection connection = new SqlConnection(CONNECTION_STRING);

            string password;
            string query = "insert into PasswordTable values (@val)";
            connection.Open();
            for (int i = 0; i < SIZE; i++)
            {
                Console.WriteLine();
                password = RandomString(20);
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@val", password);
                    try
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex) { i--; }
                }
            }
            connection.Close();


        }
        private static string RandomString(int length)
        {

            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "1234567890";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string spacialChar = "~`!@#$%^&*()_-+={[}]|\\:;\"'<,>.?/";
            int a = random.Next(1, 9);
            int b = random.Next(1, 4);
            int c = random.Next(1, 8);
            int d = length - a - b - c;
            string answer1 = GetString(upper, d);
            string answer2 = GetString(numbers, b);
            string answer3 = GetString(lower, c);
            string answer4 = GetString(spacialChar, a);

            return (answer1 + answer2 + answer3 + answer4);

        }

        private static string GetString(string str, int length)
        {
            string result = "";
            for (int i = 0; i < length; i++)
            {
                int x = random.Next(0, str.Length);
                result += str[x];
            }
            return result;

        }


    }
}
