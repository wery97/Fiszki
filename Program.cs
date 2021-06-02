using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Fiszki
{
    class Program
    {
        static void Main(string[] args)
        {
            //Data used in programm to connect with database and to operate on data
            MySqlConnection polaczenie = new MySqlConnection("server=localhost; user=root; database=slowka; SslMode=none");
            MySqlCommand komenda = new MySqlCommand();
            MySqlCommand komenda2 = new MySqlCommand();

            string wybor_tabeli="";
            string[,] bledy = new string[2, 400];
            string[] dzialy = new string[50];
            int index = 0, licznik_ilosci_bledow = 0, wykonanie_petli, liczba_dzialow = 1;
            //Open the connection
            try
            {
                polaczenie.Open();

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {

                Console.WriteLine("Error" + ex.Message.ToString());
                Console.ReadKey();
            }
            //sql query
            komenda.Connection = polaczenie;
            Console.WriteLine("Wybierz dział:\n");
            komenda.CommandText = "show tables";
            MySqlDataReader dzial = komenda.ExecuteReader();
            while (dzial.Read()) 
            {
                Console.WriteLine("{0}. {1}", liczba_dzialow, dzial.GetString(0));
                dzialy[liczba_dzialow] = dzial.GetString(0);
                liczba_dzialow++;

            }
            Console.WriteLine("Wybieram dzial nr:\n");
            int numer_dzialu = Int16.Parse(Console.ReadLine());
            polaczenie.Close();
            wybor_tabeli = dzialy[numer_dzialu];
            polaczenie.Open();
            komenda.Connection = polaczenie;
            
            komenda.CommandText = "Select * FROM "+wybor_tabeli;
 
           
            MySqlDataReader rdr = komenda.ExecuteReader();
            //read data
            while (rdr.Read())
            {
                for (; ; )
                {
                    Console.Clear();
                    Console.WriteLine("Przetlumacz : {0}\n", rdr.GetString(2));
                    if (Console.ReadLine() == rdr.GetString(1))
                    {
                        Console.WriteLine("DObrze!\n");
                        break;
                    }
                    else
                    {
                        licznik_ilosci_bledow++;
                        bledy[0, index] = rdr.GetString(2);
                        bledy[1, index] = rdr.GetString(1);
                        index++;
                        Console.WriteLine(rdr.GetString(1));
                        Console.ReadKey();
                        break;

                    }
                }

            }
            //management of faults
            if (licznik_ilosci_bledow == 0) goto koniec;

            for (; ; )
            {
                if (licznik_ilosci_bledow == 0) break;
                wykonanie_petli= licznik_ilosci_bledow;
                licznik_ilosci_bledow = 0;
                for (int i = 0; i < wykonanie_petli; i++)
                {
                    Console.Clear();
                    Console.WriteLine("Przetłumacz: {0}\n", bledy[0, i]);
                    if (Console.ReadLine() == bledy[1, i])
                    {
                        Console.WriteLine("DObrze!\n");
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("Prawidlowa odp: {0}\n", bledy[1, i]);
                        bledy[1, licznik_ilosci_bledow] = bledy[1, i];
                        bledy[0, licznik_ilosci_bledow] = bledy[0, i];
                        licznik_ilosci_bledow++;
                        Console.ReadKey();
                        continue;
                    }
                }
            }


            //zakonczenie polaczenia
            koniec:
            Console.ReadKey();
            polaczenie.Close();
        }

        
    }
}
