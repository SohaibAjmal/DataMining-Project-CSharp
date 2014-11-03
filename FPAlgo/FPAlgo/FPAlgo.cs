using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPAlgo
{

    public class Node 
    {
        private int count;
        private bool visited;
        
        public Dictionary<string, Node> item = new Dictionary<string, Node>();

        public Node()
        {
            count = 0;
            visited = false;
        }


        /*Public properties*/
        public int Count { get; set; }
        public bool Visited { get; set; }


        
    }

  

    /*This is the Node class which represents each node. Every node has its its children and all nodes*/
    /* have ItemValue which represent attribute-value pair and count for the count it appears in database*/
    public class Tree
    {
        public Node root;
        private int minSupportCount;

        public Tree()
        {
            root = new Node();
            root.Count = 0;
            root.item = null;

            /*i.e. the minimum support count 40% of total number of items*/
            minSupportCount = 13024;
        }


        /*This method builds the tree of freuqent item sets*/
        public void BuildTree(ref List<string> elemsRecvd)
        {
            if (root.item == null)
            {
                Node crawler = root;
                foreach (string element in elemsRecvd)
                {
                    Node node = new Node();

                    node.Count = 1;

                    crawler.item = new Dictionary<string, Node>();
                    crawler.item[element] = node;
                    crawler = node;
                }
           
            }
            else 
            {
                Node crawler = root;
                if (crawler.item != null)
                {
                    foreach (string element in elemsRecvd)
                    {
                        if (crawler.item.ContainsKey(element))
                        {
                            Node node = crawler.item[element];
                            node.Count = node.Count + 1;
                            crawler.item[element] = node;
                            crawler = node;
                        }
                        else
                        {
                            Node node = new Node();
                            node.Count = 1;
                            crawler.item.Add(element, node);
                            crawler = node;
                        }
                    }
                }

                
            }
           
           
        }

       
        /*This method geenrtaes most frequent itemset by mining the tree created*/
        /*It receives root node and starts mining from there*/
        public List<string> generateFrequentItemset(Node rootNode, List<string> frequentPattern)
        {
            
            


            foreach (KeyValuePair<string, Node> item in rootNode.item)
            {

                if (!item.Value.Visited && item.Value.Count > minSupportCount)
                {
                    item.Value.Visited = true;
                    frequentPattern.Add(item.Key);
                    generateFrequentItemset(item.Value, frequentPattern);
                }
            }


            return frequentPattern;
        }

    }

    class FPGrowth
    {
        private const int minSupportCount = 6512;


        /*This method generates the count of all attribute-value pair present in database*/
        private static List<List<Object>> returnC1(ref List<String> Database)
        {
            List<List<Object>> itemSets = new List<List<Object>>();
            /*Minimum support count calculated by using minimum support to be 40%*/

            /*Scan all the database transactions*/
            foreach (String transaction in Database)
            {
                /*Get all the Items in transaction*/
                String[] transItems = transaction.Split(',');


                int countOfTable = 0;


                /*See for each transaction item, add in list if not present otherwise increment count, each transaction will have 14 item*/
                foreach (String itemName in transItems)
                {

                    string itemNameTemp = itemName.Trim();
                    Boolean itemInTable = false;

                    /*Search if itemset is already present in the list C1*/
                    for (int itemCount = 0; itemCount < itemSets.Count; itemCount++)
                    {
                        if (itemSets[itemCount][0].ToString() == itemNameTemp)
                        {
                            int countOfItemName = Convert.ToInt32(itemSets[itemCount][1]);
                            itemSets[itemCount][1] = ++countOfItemName;
                            itemInTable = true;
                        }

                    }

                    /*If item is not already present in list C1, then add it with count 1*/
                    if (itemInTable == false)
                    {
                        List<Object> newItemSet = new List<Object>();
                        newItemSet.Add(itemNameTemp);
                        newItemSet.Add(1);

                        itemSets.Add(newItemSet);
                    }

                    countOfTable++;
                }
            }

            return itemSets;
        }

        /*This method prunes all those itemstes from C1 with count below minimum support count*/
        private static List<List<Object>> returnL1(ref List<List<Object>> Ck)
        {
            List<List<Object>> L1Items = new List<List<Object>>();
            for (int count = 0; count < Ck.Count; count++)
            {
                if (Convert.ToInt32(Ck[count][1]) > minSupportCount)
                {
                    L1Items.Add(Ck[count]);
                }
            }
            return L1Items;
        }


        /*This method sorts L1, containing all attribute-values with count greater than minimum support count*/
        private static void sortL1Descending(ref List<List<Object>> L1)
        {
            List<List<Object>> decendingSortedL1 = new List<List<Object>>();

            for (int i = 0; i < L1.Count; i++)
            {
                for (int j = 0; j < L1.Count -1; j++)
                {

                    if (Convert.ToInt32(L1[j][1]) < Convert.ToInt32(L1[j + 1][1]))
                    {
                        List<Object> temp = new List<Object>();
                        temp = L1[j];
                        L1[j] = L1[j + 1];
                        L1[j + 1] = temp;
                    }
                }
            }               

        }

        /*This is preprocessing method which simply concatenates the name of attribute before each value */
        /*and it discretize continous values such as age, capital gain etc.*/
        private static List<String> PreProcessing(string[] Database)
        {
            List<String> preProcessedDB = new List<String>();
            int lineNum = 0;
            foreach (String trans in Database)
            {
                String[] items = trans.Split(',');
                int index = 0;
                String newTrans = "";
                foreach (String item in items)
                {
                    String itemTemp = item.Trim();
                    if (itemTemp != "")
                    {
                        if (index == 0)
                        {
                            int age = Convert.ToInt32(itemTemp);
                            if (age > 0 && age <= 10)
                            {
                                newTrans = newTrans + "Age_1-10";
                            }
                            else if (age > 10 && age <= 20)
                            {
                                newTrans = newTrans + "Age_11-20";
                            }
                            else if (age > 20 && age <= 30)
                            {
                                newTrans = newTrans + "Age_21-30";
                            }
                            else if (age > 30 && age <= 40)
                            {
                                newTrans = newTrans + "Age_31-40";
                            }
                            else if (age > 40 && age <= 50)
                            {
                                newTrans = newTrans + "Age_41-50";
                            }
                            else if (age > 50 && age <= 60)
                            {
                                newTrans = newTrans + "Age_51-60";
                            }
                            else if (age > 60 && age <= 70)
                            {
                                newTrans = newTrans + "Age_61-70";
                            }
                            else if (age > 70 && age <= 80)
                            {
                                newTrans = newTrans + "Age_71-80";
                            }
                            else
                                newTrans = newTrans + "Age_>80";
                        }
                        else if (index == 1)
                        {
                            String workClass = itemTemp.ToString();
                            newTrans = newTrans + ", WorkClass_" + workClass;
                        }
                        else if (index == 2)
                        {
                            int fnlwgt = Convert.ToInt32(itemTemp);
                            if (fnlwgt > 0 && fnlwgt <= 50000)
                            {
                                newTrans = newTrans + ", FNLWGT_1-50000";
                            }
                            else if (fnlwgt > 50001 && fnlwgt <= 100000)
                            {
                                newTrans = newTrans + ", FNLWGT_500001-100000";
                            }
                            else if (fnlwgt > 100001 && fnlwgt <= 150000)
                            {
                                newTrans = newTrans + ", FNLWGT_100001-150000";
                            }
                            else if (fnlwgt > 150001 && fnlwgt <= 200000)
                            {
                                newTrans = newTrans + ", FNLWGT_150001-200000";
                            }
                            else if (fnlwgt > 200001 && fnlwgt <= 250000)
                            {
                                newTrans = newTrans + ", FNLWGT_200001-250000";
                            }
                            else if (fnlwgt > 250001 && fnlwgt <= 300000)
                            {
                                newTrans = newTrans + ", FNLWGT_250001-300000";
                            }
                            else
                                newTrans = newTrans + ", FNLWGT_>300000";
                        }
                        else if (index == 3)
                        {
                            String education = itemTemp.ToString();
                            newTrans = newTrans + ", education_" + education;
                        }
                        else if (index == 4)
                        {
                            int educationNum = Convert.ToInt32(itemTemp);
                            if (educationNum > 0 && educationNum <= 5)
                            {
                                newTrans = newTrans + ", educationNum_1-5";
                            }
                            else if (educationNum > 5 && educationNum <= 10)
                            {
                                newTrans = newTrans + ", educationNum_6-10";
                            }
                            else if (educationNum > 10 && educationNum <= 15)
                            {
                                newTrans = newTrans + ", educationNum_11-15";
                            }
                            else if (educationNum > 15 && educationNum <= 20)
                            {
                                newTrans = newTrans + ", educationNum_16-20";
                            }
                            else
                                newTrans = newTrans + ", educationNum_>20";

                        }
                        else if (index == 5)
                        {
                            String maritStatus = itemTemp.ToString();
                            newTrans = newTrans + ", MaritalStatus_" + maritStatus;
                        }
                        else if (index == 6)
                        {
                            String occupation = itemTemp.ToString();
                            newTrans = newTrans + ", Occupation_" + occupation;
                        }
                        else if (index == 7)
                        {
                            String relationship = itemTemp.ToString();
                            newTrans = newTrans + ", Relationship_" + relationship;
                        }
                        else if (index == 8)
                        {
                            String race = itemTemp.ToString();
                            newTrans = newTrans + ", Race_" + race;
                        }
                        else if (index == 9)
                        {
                            String sex = itemTemp.ToString();
                            newTrans = newTrans + ", Sex_" + sex;
                        }
                        else if (index == 10 || index == 11)
                        {
                            int capGainLoss = Convert.ToInt32(itemTemp);
                            if (capGainLoss >= 0 && capGainLoss <= 10000)
                            {
                                if (index == 10)
                                    newTrans = newTrans + ", CapitalGain_0-10000";
                                else if (index == 11)
                                    newTrans = newTrans + ", CapitalLoss_0-10000";
                            }
                            else if (capGainLoss > 10001 && capGainLoss <= 20000)
                            {
                                if (index == 10)
                                    newTrans = newTrans + ", CapitalGain_10001-20000";
                                else if (index == 11)
                                    newTrans = newTrans + ", CapitalLoss_10001-20000";

                            }
                            else if (capGainLoss > 20001 && capGainLoss <= 30000)
                            {
                                if (index == 10)
                                    newTrans = newTrans + ", CapitalGain_20001-30000";
                                else if (index == 11)
                                    newTrans = newTrans + ", CapitalLoss_20001-30000";
                            }
                            else if (capGainLoss > 30001 && capGainLoss <= 40000)
                            {
                                if (index == 10)
                                    newTrans = newTrans + ", CapitalGain_10001-20000";
                                else if (index == 11)
                                    newTrans = newTrans + ", CapitalLoss_10001-20000";
                            }
                            else if (capGainLoss > 40001 && capGainLoss <= 50000)
                            {
                                if (index == 10)
                                    newTrans = newTrans + ", CapitalGain_40001-50000";
                                else if (index == 11)
                                    newTrans = newTrans + ", CapitalLoss_40001-50000";
                            }
                            else
                                if (index == 10)
                                    newTrans = newTrans + ", CapitalGain_>50000";
                                else if (index == 11)
                                    newTrans = newTrans + ", CapitalLoss_>50000";

                        }
                        else if (index == 12)
                        {
                            int hoursPerWeek = Convert.ToInt32(itemTemp);
                            if (hoursPerWeek >= 0 && hoursPerWeek <= 10)
                            {
                                newTrans = newTrans + ", HoursPerWeek_0-10";
                            }
                            else if (hoursPerWeek > 10 && hoursPerWeek <= 20)
                            {
                                newTrans = newTrans + ", HoursPerWeek_11-20";
                            }
                            else if (hoursPerWeek > 30 && hoursPerWeek <= 40)
                            {
                                newTrans = newTrans + ", 3HoursPerWeek_1-40";
                            }
                            else if (hoursPerWeek > 40 && hoursPerWeek <= 50)
                            {
                                newTrans = newTrans + ", HoursPerWeek_41-50";
                            }
                            else if (hoursPerWeek > 50 && hoursPerWeek <= 60)
                            {
                                newTrans = newTrans + ", HoursPerWeek_51-60";
                            }
                            else if (hoursPerWeek > 60 && hoursPerWeek <= 70)
                            {
                                newTrans = newTrans + ", HoursPerWeek_61-70";
                            }
                            else if (hoursPerWeek > 70 && hoursPerWeek <= 80)
                            {
                                newTrans = newTrans + ", HoursPerWeek_71-80";
                            }
                            else if (hoursPerWeek > 80 && hoursPerWeek <= 90)
                            {
                                newTrans = newTrans + ", HoursPerWeek_81-90";
                            }
                            else
                                newTrans = newTrans + ", HoursPerWeek_>90";

                        }
                        else if (index == 13)
                        {
                            String country = itemTemp.ToString();
                            newTrans = newTrans + ", Country_" + country;
                        }
                        else
                            newTrans = newTrans + ", " + item;
                        index++;
                    }
                }
                lineNum++;

                preProcessedDB.Add(newTrans);
            }

            return preProcessedDB;

        }

        static void Main(string[] args)
        {
            string[] Database = System.IO.File.ReadAllLines(@"..\..\..\adult.txt");
            
            /*********************************/
            /*****Preprocessing on Data  *****/
            /*********************************/

            List<String> processedDatabase = new List<String>();
            processedDatabase = PreProcessing(Database);

            /*List of all Items for C1 so far*/
            List<List<Object>> C1ItemList = new List<List<Object>>();


            List<List<Object>> L1ItemList = new List<List<Object>>();

            /*Start the stopwatch to calculate time of execution of code*/
            System.Diagnostics.Stopwatch calcTime = System.Diagnostics.Stopwatch.StartNew();
        
            /*C1 and L1 Computation peformed. L1 will have all frequent attribute-value pairs*/
            C1ItemList = returnC1(ref processedDatabase);
            L1ItemList = returnL1(ref C1ItemList);

            /*L1Itemset is now sorted*/
            sortL1Descending(ref L1ItemList);


            /*Create Tree class object*/
            Tree tree = new Tree();

            /*This list will contain elements in each transaction that are frequent*/
            List<string> elementsFoundList = new List<string>();

            /*Traverse database and create the tree based on list C1*/
            foreach (String trans in processedDatabase)
            {
                for(int i = 0; i< L1ItemList.Count; i++)
                {     
                    /*If transaction contains frequent itemset add it*/
                    if (trans.Contains(L1ItemList[i][0].ToString()))
                    {
                        elementsFoundList.Add(L1ItemList[i][0].ToString());
                    }
                }
                tree.BuildTree(ref elementsFoundList);
                elementsFoundList.Clear();

            }


            List<string> freqItems = new List<string>();

            /*Generate frequent itemset*/
            freqItems = tree.generateFrequentItemset(tree.root, freqItems);


            Console.WriteLine("The Most Frequent Item Set in Data is: \n");
            /*Print frequent Itemset*/
            foreach (string item in freqItems)
            {
                Console.WriteLine(" "+item);
            }

            /*Stop calculating time after algorithm finishes*/
            Console.WriteLine("\nTotal Execution Time is  " + ((calcTime.ElapsedMilliseconds)/1000) + " seconds \n");

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }

        }
    }
}
