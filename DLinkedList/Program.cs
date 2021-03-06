using System;
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

            public ListRandom()
            {

            }
            public ListRandom(Stream s)
            {
                Deserialize(s);
            }
            public ListRandom(string data)
            {
                AddNode(data);
            }
            public void AddNode(string data)
            {
                ListNode node = new ListNode();
                node.Data = data;

                if (Head==null)
                {
                    Head = Tail = node;
                    node.Previous = null;
                }
                else
                {
                    node.Previous = Tail;
                    Tail.Next = node;
                    Tail = node;
                }
                node.Next = null;                
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
                if (Head != null)
                {
                    Dictionary<ListNode, int> indexList = new Dictionary<ListNode, int>();
                    int i = 0;
                    for (ListNode node = Head; node != null; node = node.Next,i++)
                        indexList.Add(node,i);

                    using (StreamWriter writer = new StreamWriter(s))
                    {
                        for (ListNode node = Head; node!=null; node = node.Next)
                        {
                            writer.WriteLine($"{node.Data}.{indexList[node.Random]}");
                        }
                            
                    }
                }
            }
            public void Deserialize(Stream s)
            {
                Count = 0;
                List<ListNode> indexList = new List<ListNode>();

                using (StreamReader reader = new StreamReader(s))
                {
                    
                    string line;
                    string temp=String.Empty;
                    
                    while ((line = reader.ReadLine())!=null)
                    {
                        if (line=="")
                        {
                            continue;
                        }
                        else
                        if (!line.Contains('.')&&temp==String.Empty)
                        {
                            temp = line;
                        }
                        else
                        {
                            if (temp==string.Empty)
                                AddNode(line);
                            else
                            {
                                AddNode($"{temp}.{line}");
                                temp = String.Empty;
                            }
                            indexList.Add(Tail);
                        }
                    }
                }
                if (Head != null)
                {
                    foreach (ListNode item in indexList)
                    {
                        var tempString = item.Data.Split('.');
                        item.Random = indexList[int.Parse(tempString[1])];
                        item.Data = tempString[0];
                    }
                }
            }
        }

       
        static void Main(string[] args)
        {
            ListRandom listRandom = new ListRandom();
            Random random = new Random();
            for (int i = 0; i < 5/*random.Next(5,10)*/; i++)
            {
                listRandom.AddNode(i.ToString());
            }
            listRandom.SetRandom();

            FileStream file = new FileStream("list.dat", FileMode.Create); 
            listRandom.Serialize(file);
            file.Close();
            Console.WriteLine("Замените в файле \'.\' на \'\\n\' или \'\\n\\n\' и нажмите Enter");
            Console.ReadKey();
            file = new FileStream("list.dat", FileMode.Open);
            ListRandom listRandom2 = new ListRandom(file);
            file.Close();

            try
            {
                if (listRandom.Count != listRandom2.Count)
                    throw new Exception("Wrong length");

                ListNode first = listRandom.Head;
                ListNode second = listRandom2.Head;

                for (; first != null && second != null; first = first.Next, second = second.Next)
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
