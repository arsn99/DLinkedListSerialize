﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLinkedList
{
    class Program
    {
        class ListNode
        {
            public ListNode Previous;
            public ListNode Next;
            public ListNode Random;
            public string Data;
        }

        class ListRandom
        {
            public ListNode Head;
            public ListNode Tail;
            public int Count;

            public ListRandom(Stream s)
            {
                Deserialize(s);
            }
            public ListRandom(string data)
            {
                Head = Tail = new ListNode();
                Head.Data = data;
                Head.Next = null;
                Count++;
            }
            public void AddNode(string data)
            {
                ListNode node = new ListNode();
                node.Data = data;
                node.Previous = Tail;
                node.Next = null;
                Tail.Next = node;
                Tail = node;
                Count++;
            }
            public void SetRandom()
            {
                Random random = new Random();
       
                for (ListNode node = Head; node!=null; node=node.Next)
                {
                    ListNode temp = Head;
                    for (int i = 0; i < random.Next(0,Count-1); i++)
                    {
                        temp = temp.Next;
                    }
                    node.Random = temp;
                 }
                
            }
            public void Serialize(Stream s)
            {
                List<ListNode> indexList = new List<ListNode>();
                for (ListNode node = Head; node != null; node = node.Next)
                    indexList.Add(node);
                
                using (StreamWriter writer = new StreamWriter(s))
                {
                    foreach (ListNode node in indexList)
                        writer.WriteLine(node.Data+"."+indexList.IndexOf(node.Random).ToString());
                }
            }
            public void Deserialize(Stream s)
            {
                Count = 0;
                ListNode node = new ListNode();
                Head = node;
                List<ListNode> indexList = new List<ListNode>();

                using (StreamReader reader = new StreamReader(s))
                {
                    string line;
                    while ((line = reader.ReadLine())!=null)
                    {
                        if (!line.Equals(""))
                        {
                            node.Data = line;
                            ListNode next = new ListNode();
                            node.Next = next;
                            next.Previous = node;
                            indexList.Add(node);
                            node = next;
                            Count++;
                        }
                    }
                }
                Tail = node.Previous;
                Tail.Next = null;
                foreach (ListNode item in indexList)
                {
                    var tempString = item.Data.Split('.');
                    item.Random = indexList[int.Parse(tempString[1])];
                    item.Data = tempString[0];
                }
            }
        }

       
        static void Main(string[] args)
        {
            ListRandom listRandom = new ListRandom("Head Test");
            Random random = new Random();
            for (int i = 0; i < 5/*random.Next(5,10)*/; i++)
            {
                listRandom.AddNode(i.ToString());
            }
            listRandom.SetRandom();

            FileStream file = new FileStream("list.dat", FileMode.OpenOrCreate); 
            listRandom.Serialize(file);
            file.Close();

            file = new FileStream("list.dat", FileMode.Open);
            ListRandom listRandom2 = new ListRandom(file);
            file.Close();

            try
            {
                if (listRandom.Count != listRandom2.Count)
                    throw new Exception("Wrong length");

                ListNode first = listRandom.Head;
                ListNode second = listRandom2.Head;

                for (; first != null || second != null; first = first.Next, second = second.Next)
                {
                    if ((first.Data != second.Data) || (first.Random.Data != second.Random.Data))
                        throw new Exception("Wrong Data");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}"); 
            }
            finally
            {
                Console.WriteLine("Completed");
            }

            Console.ReadKey();
        }
    }
}